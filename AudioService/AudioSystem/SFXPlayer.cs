using System;
using System.Collections;
using System.Collections.Generic;
using Services.Injection;
using UnityEngine;
using UnityEngine.Audio;

namespace Services.AudioSystem
{
    public class SFXPlayer : ScriptableObject, IInjectionObject
    {
        [SerializeField] private AudioMixer m_MasterMixer;
        [SerializeField] private SFXConfig[] m_SoundsConfigs;

        private LinkedList<AudioSource> sources;
        private Dictionary<int, SFX> activeSfxs;
        private GameObject sourcesHolder;
        private MonoBehaviour routineRunner;

        public SFXConfig[] SoundsConfigs => m_SoundsConfigs;

        public void Setup(MonoBehaviour routineRunner)
        {
            this.routineRunner = routineRunner;
            sources = new LinkedList<AudioSource>();
            activeSfxs = new Dictionary<int, SFX>();
            
            sourcesHolder = new GameObject("AudioSourcesHolder");
            DontDestroyOnLoad(sourcesHolder);
            AudioSource source = sourcesHolder.AddComponent<AudioSource>();

            sources.AddLast(source);
        }

        public bool IsSourcePlaying(SFXConfig config)
        {
            return activeSfxs.ContainsKey(config.GetInstanceID());
        }

        public bool Play(SFXConfig config, Action onEnd = null)
        {
            if (config == null)
            {
                Debug.LogError("Config is null!");
                return false;
            }

            int id = config.GetInstanceID();
            AudioSource source = PlaySource(config);
            
            if (activeSfxs.ContainsKey(id) && activeSfxs[id].Routine != null)
            {
                Stop(config);
            }
            
            activeSfxs[id] = new SFX(source, SourcePlayer(source, config, () =>
            {
                onEnd?.Invoke();
            }));

            routineRunner.StartCoroutine(activeSfxs[id].Routine);
            return true;
        }

        public void StopAll()
        {
            foreach (var sfx in activeSfxs)
            {
                activeSfxs.Remove(sfx.Key);
                ConsumeSource(sfx.Value.Source);
            }
            
            activeSfxs.Clear();
        }

        public void Stop(SFXConfig config)
        {
            int id = config.GetInstanceID();
            if (!activeSfxs.TryGetValue(id, out var sfx))
            {
                Debug.LogWarning($"There is not any source with config ID : {id}");
                return;
            }

            activeSfxs.Remove(id);
            routineRunner.StopCoroutine(sfx.Routine);
            ConsumeSource(sfx.Source);
        }

        public void ConsumeSource(AudioSource source)
        {
            StopSource(source);
            sources.AddLast(source);
        }

        public AudioSource GetFreeSource()
        {
            AudioSource source;
            if (sources.Count > 0)
            {
                source = sources.First.Value;
                sources.RemoveFirst();
                return source;
            }

            source = sourcesHolder.AddComponent<AudioSource>();
            return source;
        }

        private AudioSource PlaySource(SFXConfig config)
        {
            AudioSource source = GetFreeSource();

            source.clip = config.Clip;
            source.outputAudioMixerGroup = config.MixerGroup;
            source.volume = config.Volume;
            source.pitch = config.Pitch;
            source.loop = config.Loop;

            source.Play();

            return source;
        }

        private bool StopSource(AudioSource source)
        {
            if (!source.isPlaying) return false;
            source.Stop();
            return true;
        }

        public void MuteSounds(bool isOn)
        {
            if (isOn)
                m_MasterMixer.SetFloat("effects_vol", 0);
            else m_MasterMixer.SetFloat("effects_vol", -80);
        }

        public void MuteMusic(bool isOn)
        {
            if (isOn)
                m_MasterMixer.SetFloat("music_vol", 0);
            else m_MasterMixer.SetFloat("music_vol", -80);
        }


        private IEnumerator SourcePlayer(AudioSource source, SFXConfig config, Action onEnd)
        {
            while (source.isPlaying)
            {
                yield return Yielders.Get(0.1f);
            }

            Stop(config);
            onEnd?.Invoke();
        }
    }
}