using System;
using CowRun;
using Firing;
using Meta.WitAi;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CowInteraction : MonoBehaviour
{
    public CowControls cowControls;
    public LabelPool labelPool;
    public ScoreCounter scoreCounter;
    public SfxPool sfxPool;
    
    
    [Header("Round Configuration")]
    public float roundTime = 105;
    public TextMeshPro timeLabel;
    public PanelManager endGamePanel;
    private float _timeCounter = 0;
    private bool _gameEnded = false;

    private int __lastSecondShown = -1;
    
    private void Update()
    {
        if(_gameEnded)return;
        _timeCounter += Time.deltaTime;
        if (_timeCounter >= roundTime )
        {
            EndGame();
            return;
        }
        int timer = Mathf.FloorToInt(roundTime - _timeCounter);
        if (__lastSecondShown != timer)
        {
            timeLabel.SetText("{0:00}:{1:00}", (int)(timer / 60), timer % 60);
            __lastSecondShown = timer;
        }
        
    }

    private void OnTriggerEnter(Collider triggerObject)
    {
        ////// Trigger con un Oggetto collider //////////////
        if (triggerObject.CompareTag("CR_Collider"))
        {
            Debug.Log("Trigger collider.");
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
            
            return;
        }

        ////// Trigger della fine della strada, fine partita ////////////
        if (triggerObject.CompareTag("CR_Trigger_EndRoad"))
        {
            EndGame();
            
            return;
        }
    }

    private void EndGame()
    {
        _gameEnded = true;
        this.cowControls.GameEnded();
        // show endpanel
    }
}
