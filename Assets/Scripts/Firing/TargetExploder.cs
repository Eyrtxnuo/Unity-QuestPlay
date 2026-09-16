using System.Collections;
using PingPong;
using UnityEngine;


public class TargetExploder : MonoBehaviour
{
    public delegate void Callback();
    
    public float forza = 500f;
    public float raggio = 5f;
    private float vanishTime = 0.6f;

    public void Explode(Vector3 explosionPosition, Callback callback)
    {
        this.gameObject.SetActive(true);
        Rigidbody[] fragments = GetComponentsInChildren<Rigidbody>();
        

        foreach (Rigidbody rb in fragments)
        {
            rb.isKinematic = false;
            rb.AddExplosionForce(forza, explosionPosition, raggio);
        }
        
        StartCoroutine(VanishFragments(fragments, callback));
    }
    
    private IEnumerator VanishFragments(Rigidbody[] fragments, Callback callback)
    {
        yield return WaitsCache.GetWaitForSeconds(0.3f);
        
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
        callback();
    }
}
