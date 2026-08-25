using PingPong;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanelManager : PanelManager
{
    public PauseManager pauseManager;

    private bool reloading = false;

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
        if(!reloading) base.ShowPanel();
    }

    public new void HidePanel()
    {
        if(!reloading) base.HidePanel();
    }
    
}
