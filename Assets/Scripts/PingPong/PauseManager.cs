using System;
using UnityEngine;

namespace PingPong
{
    public class PauseManager : MonoBehaviour
    {
        public PausePanelManager _pausePanelManager;
        
        public Action OnPause;
        public Action OnResume;

        private bool _is_paused = false;
        
        public void PauseInput()
        {
            if (_is_paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            _is_paused = true;
            Time.timeScale = 0;
            OnPause?.Invoke();
            _pausePanelManager.ShowPanel();
        }

        public void Resume()
        {
            _is_paused = false;
            Time.timeScale = 1;
            OnResume?.Invoke();
            _pausePanelManager.HidePanel();
        }

        public bool IsPaused()
        {
            return _is_paused;   
        }
        
    }
}