using UnityEngine;

public class TargetExploder : MonoBehaviour
{
    public float forza = 500f;
    public float raggio = 5f;
    // Sposta il punto di innesco leggermente in basso per far saltare i pezzi verso l'alto
    public float offsetVerticale = -0.5f; 

    public void Explode(Vector3 puntoEsplosione)
    {
        // Trova tutti i Rigidbody nei 27 frammenti figli
        Rigidbody[] frammenti = GetComponentsInChildren<Rigidbody>();
        
        // Calcola il centro dell'esplosione
        //Vector3 puntoEsplosione = transform.position + new Vector3(0, offsetVerticale, 0);

        foreach (Rigidbody rb in frammenti)
        {
            // Attiva la fisica (fa cadere i pezzi)
            rb.isKinematic = false;
            
            // Applica la forza d'urto per farli schizzare via
            rb.AddExplosionForce(forza, puntoEsplosione, raggio);
        }
    }
}
