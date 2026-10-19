using System;
using System.Collections.Generic;
using PingPong;
using UnityEngine;

public class StopHandTracking : MonoBehaviour
{
    public GameObject[] disableObjects;
    public PanelManager alertPanel;
    public PauseManager pauseManager;
    [Range(1, 120)]
    public int framesBetweenChecks;
    private int _counter;
    private bool _lostTracking = false;
    
    private List<GameObject> _toReenableObjects;
    private float _originalTimeScale;

    private void Start()
    {
        _toReenableObjects = new List<GameObject>();
    }

    void Update()
    {
        _counter++;
        if (_counter >= framesBetweenChecks)
        {
            _counter = 0;
            OVRInput.Controller activeController = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.RightHanded);
            if (!_lostTracking && activeController != OVRInput.Controller.RTouch)
            {
                // non è il controller destro che controlla la mano, blocca gioco.
                _lostTracking = true;
                alertPanel.ShowPanel();
                pauseManager.PauseLock(true);
                _originalTimeScale = Time.timeScale;
                Time.timeScale = 0;
                // saves the active objects in a list, to re-enable only the one that were enabled before the tracking loss
                foreach (var obj in disableObjects)
                {
                    if (obj.activeSelf)
                    {
                        obj.SetActive(false);
                        _toReenableObjects.Add(obj);
                    }
                }
            }

            if (_lostTracking && activeController == OVRInput.Controller.RTouch)
            {
                _lostTracking = false;
                alertPanel.HidePanel();
                pauseManager.PauseLock(false);
                Time.timeScale = _originalTimeScale;
                //re-enables only the disabled objects
                foreach (var obj in _toReenableObjects)
                {
                    obj.SetActive(true);
                }
                _toReenableObjects.Clear();
                
            }
            
            
        }
        
       

        // if (activeController == OVRInput.Controller.RHand)
        // {
        //     Debug.Log("L'utente sta usando la MANO destra.");
        // }
        // else if (activeController == OVRInput.Controller.RTouch)
        // {
        //     Debug.Log("L'utente sta usando il CONTROLLER destro.");
        // }
        // else
        // {
        //     Debug.Log("Nessun input attivo o rilevato.");
        // }
    }
}
