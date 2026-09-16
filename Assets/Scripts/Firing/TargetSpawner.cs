using UnityEngine;
using UnityEngine.Serialization;

public class TargetSpawner : MonoBehaviour
{
    [FormerlySerializedAs("prefabToSpawn")] [Header("Impostazioni Prefab")]
    public GameObject[] prefabsToSpawn; // Trascina qui il tuo prefab dall'Inspector

    [Header("Controllo Quantità")]
    public int maxObjectsInScene = 1; // Il numero N di oggetti da tenere attivi

    // Usiamo questo tag per contare quanti oggetti ci sono in scena
    private string targetTag = "AimTarget";

    void Update()
    {
        // Trova quanti oggetti con il tag "AimTarget" esistono attualmente nella scena
        int currentObjectCount = GameObject.FindGameObjectsWithTag(targetTag).Length;
        
        // Se sono meno di N, spawna fino a raggiungere il limite
        if (currentObjectCount < maxObjectsInScene)
        {
            SpawnPrefabRandom();
        }
    }

    void SpawnPrefabRandom()
    {
        if (prefabsToSpawn.Length == 0)
        {
            Debug.LogWarning("Manca il prefab! Trascinalo nell'Inspector.");
            return;
        }

        // Coordinate random richieste
        float randomX = Random.Range(-2.5f, 2.5f);
        float randomY = Random.Range(0.5f, 3.0f);
        float fixedZ = 3.5f;

        Vector3 spawnPosition = new Vector3(randomX, randomY, fixedZ);

        // Genera l'oggetto
        var prefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];
        GameObject spawnedObject = Instantiate(prefab, spawnPosition, prefab.transform.rotation);

        // Forziamo il tag corretto così il conteggio (e il laser) funzionano al 100%
        spawnedObject.tag = targetTag;
    }
}
