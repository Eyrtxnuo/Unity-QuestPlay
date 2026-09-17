using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrassInstancer : MonoBehaviour
{
    public Mesh grassMesh;
    public Material grassMaterial;
    public float meshScale = 5.0f;
    
    [Range(10, 1023)]
    public int instanceCount = 150;
    public float spawnRadius = 5f;
    

    private Matrix4x4[] matrices;

    void Start()
    {
        matrices = new Matrix4x4[instanceCount];
        for (int i = 0; i < instanceCount; i++)
        {
            // Generazione casuale in un raggio attorno a questo GameObject
            Vector2 randPos = Random.insideUnitCircle * spawnRadius;
            
            // Mantiene l'erba a quota 0 (o usa l'altezza del transform)
            Vector3 pos = transform.position + new Vector3(randPos.x, 0, randPos.y);
            
            // Rotazione casuale solo sull'asse Y
            Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            
            // Variazione casuale della scala (es. dal 70% al 120%)
            Vector3 scale = Vector3.one * Random.Range(0.7f, 1.2f) * meshScale;

            // Crea la matrice TRS (Traslazione, Rotazione, Scala)
            matrices[i] = Matrix4x4.TRS(pos, rot, scale);
        }
    }

    void Update()
    {
        // Invia tutte le 150 mesh alla GPU in un colpo solo
        Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, matrices, instanceCount);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Handles.color = new Color(1f, 0.4f, 0.4f, 0.5f);
        // Disegna un disco vuoto (cerchio) rivolto verso l'alto (asse Y) con il raggio specificato
        Handles.DrawWireDisc(transform.position, Vector3.up, spawnRadius);
    }
#endif
}