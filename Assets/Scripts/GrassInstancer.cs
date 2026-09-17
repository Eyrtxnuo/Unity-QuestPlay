using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrassInstancer : MonoBehaviour
{
    public Mesh grassMesh;
    public Material grassMaterial;
    public float meshScale = 5.0f;
    
    [Range(1, 1023)]
    public int instanceCount = 150;
    public float spawnRadius = 5f;

    [Tooltip("Aggiunge un offset casuale per rendere l'erba meno geometrica. 0 = spirale perfetta.")]
    [Range(0f, 1f)]
    public float jitterLevel = 0.3f; 
    
    [Tooltip("Cambia questo valore per ottenere una disposizione diversa in modo stabile e replicabile.")]
    public int seed = 42;
    
    private Matrix4x4[] _matrices;

    void Start()
    {
        GenerateMatrices();
    }

    void OnValidate()
    {
        GenerateMatrices();
    }

    void GenerateMatrices()
    {
        if (instanceCount <= 0) return;

        _matrices = new Matrix4x4[instanceCount];
        
        // Salva lo stato globale attuale della casualità
        Random.State oldState = Random.state;
        
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
            Vector3 scale = Vector3.one * Random.Range(0.9f, 1.1f) * meshScale;

            _matrices[i] = Matrix4x4.TRS(pos, rot, scale);
        }

        // Ripristina lo stato globale originale
        Random.state = oldState;
    }

    void Update()
    {
        if (_matrices is { Length: > 0 } && grassMesh && grassMaterial)
        {
            Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, _matrices, instanceCount);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Handles.color = new Color(1f, 0.4f, 0.4f, 0.5f);
        Handles.DrawWireDisc(transform.position, Vector3.up, spawnRadius);

        if (_matrices != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _matrices.Length; i++)
            {
                Vector3 pos = _matrices[i].GetPosition();
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }
    }
#endif
}