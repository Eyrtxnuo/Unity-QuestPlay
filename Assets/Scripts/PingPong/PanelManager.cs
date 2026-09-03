using PingPong;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject OVRController;

    private static int _instances = 0; // open panels (Andrebbero calcolati i panel aperti all'inizio!)
    

    public void ChangeGameButtonHandler()
    {
        StartCoroutine(MainMenu.LoadSceneAsyncRoutine("MainMenu"));
    }
    
    public void PlayAgainButtonHandler()
    {
        StartCoroutine(MainMenu.LoadSceneAsyncRoutine(SceneManager.GetActiveScene().name));
    }

    public void ShowPanel()
    {
        var wasActive = gameObject.activeSelf;
        gameObject.SetActive(true);
        if(_instances == 0) OVRController.SetActive(true);
        if (!wasActive) _instances++;
        Debug.Log("PanelInstances " + _instances);
    }



    public void HidePanel()
    {
        var wasActive = gameObject.activeSelf;
        gameObject.SetActive(false);
        if (wasActive && _instances > 0) _instances--;
        if(_instances == 0) OVRController.SetActive(false);
        
        Debug.Log("PanelInstances " + _instances);
    }

    public static void ResetInstanceCounter()
    {
        _instances = 0;
    }
}
