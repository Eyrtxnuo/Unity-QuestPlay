using System;
using UnityEngine;

namespace PingPong
{
    public class PausePanelManager : PanelManager
    {
        public PauseManager pauseManager;
        public AudioClip invokeClip;

        private AudioSource _audioSource;
        private bool reloading = false;

        public void Awake()
        {
            
            TryGetComponent(out _audioSource);
        }

        public void Start()
        {
            pauseManager.OnPause += ShowPanel;
            pauseManager.OnResume += HidePanel;
            HidePanel();
        }
        public void ResumeButtonHandler()
        {
            pauseManager.Resume();
            HidePanel();
        }

        public new void PlayAgainButtonHandler()
        {
            reloading = true;
            pauseManager.Resume();
            base.PlayAgainButtonHandler();
        }

        public new void ChangeGameButtonHandler()
        {
            reloading = true;
            pauseManager.Resume();
            base.ChangeGameButtonHandler();
        }

        public new void ShowPanel()
        {
            if(reloading)return;
            base.ShowPanel();
            PlaySfx(invokeClip);
        }

        public new void HidePanel()
        {
            if(reloading)return;
            base.HidePanel();
        }
        
        private void PlaySfx(AudioClip audioClip)
        {
            if(!_audioSource) return;
            _audioSource.PlayOneShot(audioClip);
        }
    
    }
}
