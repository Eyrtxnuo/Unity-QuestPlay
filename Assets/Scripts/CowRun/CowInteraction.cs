using CowRun;
using Firing;
using Meta.WitAi;
using UnityEngine;

public class CowInteraction : MonoBehaviour
{
    public CowControls cowControls;
    public LabelPool labelPool;
    public ScoreCounter scoreCounter;
    public SfxPool sfxPool;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider triggerObject)
    {
        if (triggerObject.CompareTag("CR_Collider"))
        {
            Debug.Log("Trigger collider .");
            var collided = triggerObject.gameObject.GetComponent<CollidingElement>();
            if (collided == null) return;
            var points = collided.PointsValue;
            labelPool.ShowPointLabel(points, this.transform.position + (this.transform.rotation*new Vector3(0,0.8f,1.3f)));
            CollisionEffect effect = collided.Hit();
            if (cowControls)
            {
                cowControls.ApplyEffect(effect);
            }
            scoreCounter.Score += points;
            sfxPool.PlayClipAtPosition(collided.GetHitSound(), collided.transform.position, 0.2f, Random.Range(0.9f,1.1f), 1);
        }
    }
}
