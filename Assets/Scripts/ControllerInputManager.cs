using System;
using System.Collections.Generic;
using PingPong;
using UnityEngine;

public class ControllerInputManager : MonoBehaviour
{

    public GameObject ball;
    public PingPongGameMaster gameMaster;
    public PauseManager pauseManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        bool gamePaused = pauseManager.IsPaused(); 
            
            
        // DEBUG INPUTS //////////////////////////////
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch) && !gamePaused)
        {
            Debug.Log("Pulsante Y premuto, batte il player.");
            //ball.GetComponent<Ball3D>().Relaunch();
            gameMaster.Serve(Team.Player);
        }
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch) && !gamePaused)
        {
            Debug.Log("Pulsante B premuto, batte il bot.");
            gameMaster.Serve(Team.Opponent);
        }
        //////////////////////////////////////////////
        
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) && !gamePaused)
        {
            Debug.Log("Pulsante A premuto, tasto 'Pronto'.");
            gameMaster.Ready_Button();
        }
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch) && !gamePaused)
        {
            Debug.Log("Pulsante X premuto, player alza la pallina.");
            gameMaster.BallTossInput();
        }


        if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch))
        {
            pauseManager.PauseInput();
        }
        
    }
}
