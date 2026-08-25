using PingPong;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject OVRController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        OVRController.SetActive(true);
        gameObject.SetActive(true);
    }



    public void HidePanel()
    {
        OVRController.SetActive(false);
        gameObject.SetActive(false);
    }
}
