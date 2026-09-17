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

    [Tooltip("Aggiunge un offset casuale per rendere l'erba meno geometrica. 0 = spirale perfetta.")]
    [Range(0f, 1f)]
    public float jitterLevel = 0.3f; 
    
    [Tooltip("Cambia questo valore per ottenere una disposizione diversa in modo stabile e replicabile.")]
    public int seed = 42;
    
    private Matrix4x4[] matrices;

    void Start()
    {
        GenerateMatrices();
    }

    // Viene chiamato automaticamente in Editor ogni volta che modifichi un parametro nell'Inspector
    void OnValidate()
    {
        GenerateMatrices();
    }

    void GenerateMatrices()
    {
        if (instanceCount <= 0) return;

        matrices = new Matrix4x4[instanceCount];
        
        // 1. Salva lo stato globale attuale della casualità
        Random.State oldState = Random.state;
        
        // 2. Inizializza con il tuo seed specifico per l'erba
        Random.InitState(seed);

        float goldenAngle = 137.508f * Mathf.Deg2Rad;

        for (int i = 0; i < instanceCount; i++)
        {
            float t = (float)i / (instanceCount > 1 ? instanceCount - 1 : 1);
            float r = spawnRadius * Mathf.Sqrt(t);
            float theta = i * goldenAngle;

            float x = r * Mathf.Cos(theta);
            float z = r * Mathf.Sin(theta);

            Vector2 jitter = Random.insideUnitCircle * (spawnRadius * 0.1f * jitterLevel);
            Vector3 pos = transform.position + new Vector3(x + jitter.x, 0, z + jitter.y);
            
            Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            Vector3 scale = Vector3.one * Random.Range(0.7f, 1.2f) * meshScale;

            matrices[i] = Matrix4x4.TRS(pos, rot, scale);
        }

        // 3. Ripristina lo stato globale originale
        Random.state = oldState;
    }

    void Update()
    {
        // Controllo di sicurezza prima di renderizzare
        if (matrices != null && matrices.Length > 0 && grassMesh != null && grassMaterial != null)
        {
            Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, matrices, instanceCount);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Disegna il disco di confine
        Handles.color = new Color(1f, 0.4f, 0.4f, 0.5f);
        Handles.DrawWireDisc(transform.position, Vector3.up, spawnRadius);

        // Disegna la preview dei punti in Editor
        if (matrices != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < matrices.Length; i++)
            {
                // Estrae la posizione esatta calcolata dalla matrice
                Vector3 pos = matrices[i].GetPosition();
                
                // Disegna una piccola sfera per rappresentare l'istanza di erba
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }
    }
#endif
}