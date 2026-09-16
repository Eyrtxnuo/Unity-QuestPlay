using System.Collections;
using PingPong;
using UnityEngine;

public class TargetExploder : MonoBehaviour
{
    public float forza = 500f;
    public float raggio = 5f;
    // Sposta il punto di innesco leggermente in basso per far saltare i pezzi verso l'alto
    public float offsetVerticale = -0.5f;


    private float vanishTime = 0.4f;

    public void Explode(Vector3 explosionPosition)
    {
        // Trova tutti i Rigidbody nei 27 frammenti figli
        Rigidbody[] fragments = GetComponentsInChildren<Rigidbody>();
        
        // Calcola il centro dell'esplosione
        //Vector3 puntoEsplosione = transform.position + new Vector3(0, offsetVerticale, 0);

        foreach (Rigidbody rb in fragments)
        {
            // Attiva la fisica (fa cadere i pezzi)
            rb.isKinematic = false;
            
            // Applica la forza d'urto per farli schizzare via
            rb.AddExplosionForce(forza, explosionPosition, raggio);
        }
        
        StartCoroutine(VanishFragments(fragments));
    }
    
    private IEnumerator VanishFragments(Rigidbody[] fragments)
    {
        float time = 0f;
        Vector3[] initialScales = new Vector3[fragments.Length];
        for (int i = 0; i < fragments.Length; i++) 
        {
            if (fragments[i]) initialScales[i] = fragments[i].transform.localScale;
        }
        while (time < vanishTime)
        {
            time += Time.deltaTime;
            float percentage = 1f - (time / vanishTime);
            for (int i = 0; i < fragments.Length; i++)
            {
                if (fragments[i])
                {
                    fragments[i].transform.localScale = initialScales[i] * percentage;
                }
            }
            yield return null; 
        }
        Destroy(gameObject);
    }
}
