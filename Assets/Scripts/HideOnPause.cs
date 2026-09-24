using PingPong;
using UnityEngine;

public class HideOnPause : MonoBehaviour
{
    public Renderer[] renderers;
    
    
    private void OnEnable()
    {
        PauseManager.OnPause += _hPause;
        PauseManager.OnResume += _hResume;
    }

    private void OnDisable()
    {
        PauseManager.OnPause -= _hPause;
        PauseManager.OnResume -= _hResume;
    }

    private void _hPause()
    {
        HandlePause(true);
    }
    private void _hResume()
    {
        HandlePause(false);
    }


    private void HandlePause(bool paused)
    {
        foreach (Renderer r in renderers)
        {
            r.enabled = !paused;
        }
    }
}
