using System.Collections;
using PingPong;
using UnityEngine;

public class HaloHandler : MonoBehaviour
{
    public float haloDuration = 0.6f;
    public float haloStartScale = 0.05f;
    public float haloEndScale = 0.4f;
    public Color haloBlue = new Color(0.2f, 0.6f, 1f, 0.8f);
    public Color haloRed = new Color(1f, 0.2f, 0.2f, 0.8f);
    
    public void SpawnHalo(Vector3 position, Vector3 normal, Team scoringTeam)
    {
        // leggero offset lungo la normale per evitare z-fighting col tavolo
        Vector3 pos = position + normal * 0.001f;
    
        this.transform.position = pos;
        this.transform.rotation = Quaternion.LookRotation(-normal);
        this.transform.localScale = Vector3.one * haloStartScale;

        Renderer rend = this.GetComponent<Renderer>();
        Material mat = rend.material; // istanza runtime, non l'asset condiviso

        Color startColor = scoringTeam == Team.Opponent ? haloRed : haloBlue;
        mat.color = startColor;

        StartCoroutine(AnimateHalo(mat, startColor));
    }
    
    IEnumerator AnimateHalo(Material mat, Color startColor)
    {
        float elapsed = 0f;
        while (elapsed < haloDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / haloDuration;

            float scale = Mathf.Lerp(haloStartScale, haloEndScale, t);
            this.transform.localScale = new Vector3(scale, scale, scale);

            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, t);
            mat.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }
}
