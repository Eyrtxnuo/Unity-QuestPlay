using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
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
        fpsText.SetText("{0} FPS", Mathf.Ceil(fps));
        
    }
}
