using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TreeInstancer : MonoBehaviour
{
    public Mesh[] meshes;
    public Material material;
    public float meshScale = 5.0f;
    
    [Range(1, 1023)]
    public int instanceCount = 150;
    public float endRadius = 10f;
    public float startRadius = 5f;

    [Tooltip("Aggiunge un offset casuale per rendere l'erba meno geometrica. 0 = spirale perfetta.")]
    [Range(0f, 1f)]
    public float jitterLevel = 0.3f; 
    
    [Tooltip("Cambia questo valore per ottenere una disposizione diversa in modo stabile e replicabile.")]
    public int seed = 42;
    
    private Matrix4x4[] _matrices;
    private int[] _meshesChosen;

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
            float r = startRadius +((endRadius-startRadius) * Mathf.Sqrt(t));
            float theta = i * goldenAngle;

            float x =  r * Mathf.Cos(theta);
            float z = r * Mathf.Sin(theta);

            Vector2 jitter = Random.insideUnitCircle * (endRadius * 0.1f * jitterLevel);
            Vector3 pos = transform.position + new Vector3(x + jitter.x, 0, z + jitter.y);
            
            Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            Vector3 scale = Vector3.one * Random.Range(0.9f, 1.1f) * meshScale;

            _matrices[i] = Matrix4x4.TRS(pos, rot, scale);
        }

        _meshesChosen = new int[instanceCount];
        for (int i = 0; i < instanceCount; i++)
        {
            _meshesChosen[i] = Random.Range(0, meshes.Length);
        }

        // Ripristina lo stato globale originale
        Random.state = oldState;
    }

    private void Update()
    {
        for(int meshIdx = 0; meshIdx < meshes.Length; meshIdx++)
        {
            var mesh = meshes[meshIdx];
            var meshMatrices = _matrices.Where((x4, i) => _meshesChosen[i] == meshIdx).ToArray();
            if (meshMatrices is { Length: > 0 } && mesh && material)
            {
                Graphics.DrawMeshInstanced(mesh, 0, material, meshMatrices, meshMatrices.Length);
            }
        }
        
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if(!this.enabled)return;
        Handles.color = new Color(1f, 0.4f, 0.4f, 0.5f);
        Handles.DrawWireDisc(transform.position, Vector3.up, endRadius);
        Handles.color = new Color(0.5f, 0.4f, 0.4f, 0.5f);
        Handles.DrawWireDisc(transform.position, Vector3.up, startRadius);

        if (_matrices != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _matrices.Length; i++)
            {
                Vector3 pos = _matrices[i].GetPosition();
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }
        //Update(); //preview degli alberi, molto lampeggiante
    }
#endif
}