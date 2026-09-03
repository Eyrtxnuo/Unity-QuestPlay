using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GoToWorld(string worldName)
    {
        StartCoroutine(LoadSceneAsyncRoutine(worldName));
    }
    
    public static IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        PanelManager.ResetInstanceCounter();
        //TODO: disattiva i click per il loading degli altri mondi
        // e mangari aggiungere uno schermo di caricamento
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (asyncLoad is { isDone: false })
        {
            yield return null; 
        }
    }
}
