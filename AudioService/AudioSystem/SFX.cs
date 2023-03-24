using System.Collections;
using UnityEngine;

namespace Services.AudioSystem
{
    public struct SFX
    {
        public SFX(AudioSource source, IEnumerator routine)
        {
            Source = source;
            Routine = routine;
        }
        
        public AudioSource Source { get; set; }
        public IEnumerator Routine { get; set; }
    }
}