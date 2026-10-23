using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
