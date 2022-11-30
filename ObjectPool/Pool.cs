public class Pool<T> where T : IPoolObject
    {
        private Queue<T> _pendingQueue;
        private Func<T> _creator;
        private Action<T> _destroyer;

        public Pool(Func<T> creator, Action<T> destroyer, int capacity = 0)
        {
            _creator = creator;
            _destroyer = destroyer;
            _pendingQueue = new Queue<T>(CreateRange(capacity));
        }

        public T Get()
        {
            if (_pendingQueue.Count == 0)
            {
                return Create();
            }

            var item = _pendingQueue.Dequeue();
            item.Activate();
            return item;
        }

        public void Release(T item)
        {
            item.Deactivate();
            _pendingQueue.Enqueue(item);
        }

        public void Clean()
        {
            foreach (T poolObject in _pendingQueue)
            {
                _destroyer?.Invoke(poolObject);
            }
            
            _pendingQueue.Clear();
        }
        
        private IEnumerable<T> CreateRange(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var item = Create();
                item?.Deactivate();
                yield return item;
            }
        }

        private T Create()
        {
            T item = _creator.Invoke();
            if (item == null)
            {
                Debug.LogError("Creator or creating item is null");
            }
            return item;
        }
    }
