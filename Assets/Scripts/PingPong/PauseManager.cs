using System;
using UnityEngine;

namespace PingPong
{
    public class PauseManager : MonoBehaviour
    {
        public PausePanelManager _pausePanelManager;
        
        public static Action OnPause;
        public static Action OnResume;

        public static bool is_paused = false;
        
        public bool lockedPause { get; private set; } = false ;
        
        public void PauseInput()
        {
            if (is_paused)
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
            if(lockedPause)return;
            is_paused = true;
            Time.timeScale = 0;
            OnPause?.Invoke();
            _pausePanelManager.ShowPanel();
        }

        public void Resume()
        {
            if(lockedPause)return;
            is_paused = false;
            Time.timeScale = 1;
            OnResume?.Invoke();
            _pausePanelManager.HidePanel();
        }

        public bool IsPaused()
        {
            return is_paused;   
        }
        
        public void PauseLock(bool value) // disables pause and resume
        {
            lockedPause = value;
        }
        
    }
}