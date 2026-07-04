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


    public void GoToWorld()
    {
        //StartCoroutine(LoadSceneAsyncRoutine("Antiaerea"));
    }
    
    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        //TODO: disattiva i click per il loading degli altri mondi
        // e mangari aggiungere uno schermo di caricamento
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null; 
        }
    }
}
