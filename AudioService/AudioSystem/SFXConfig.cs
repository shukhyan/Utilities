using UnityEngine;
using UnityEngine.Audio;

namespace Services.AudioSystem
{
    [CreateAssetMenu(fileName = "Sound", menuName = "Effects/SFX Config")]
    public class SFXConfig : ScriptableObject
    {
        [SerializeField] private SFXPlayer m_Player;
        
        [Space, Header("Sound Parameters"), SerializeField]
        private AudioClip m_Clip;
        [SerializeField] private AudioMixerGroup m_MixerGroup;

        [Range(0, 2f), SerializeField] private float m_Volume = 1f;
        [Range(-3f, 3f), SerializeField] private float m_Pitch = 1f;
        [SerializeField] private bool m_Loop;

        public AudioClip Clip => m_Clip;

        public AudioMixerGroup MixerGroup => m_MixerGroup;
        public float Volume => m_Volume;
        public float Pitch => m_Pitch;
        public bool Loop => m_Loop;
        

        public void SetupPlayer(SFXPlayer player)
        {
            m_Player = player;
        }

        public void Play()
        {
            if (m_Player == null)
            {
                Debug.LogError($"Add SFX Player to {name} config, please!");
                return;
            }

            m_Player.Play(this);
        }

        public void Stop()
        {
            if (m_Player == null)
            {
                Debug.LogError($"Add SFX Player to {name} config, please!");
                return;
            }
            
            m_Player.Stop(this);
        }
    }
}