using TMPro;
using UnityEngine;

public class FPSCounterUpdater : MonoBehaviour
{
    private TextMeshPro fpsText;
    private float deltaTime = 0.0f;
    
    void Start()
    {
        fpsText = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        // Calcola il tempo tra un frame e l'altro (unscaled evita problemi se metti il gioco in pausa)
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
        // Converte in FPS
        float fps = 1.0f / deltaTime;

        // Aggiorna il testo (Arrotondato per intero)
        if (fpsText != null)
        {
            fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
        }
        
    }
}
