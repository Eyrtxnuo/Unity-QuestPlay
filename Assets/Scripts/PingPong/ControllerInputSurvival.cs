using UnityEngine;

namespace PingPong
{
    public class ControllerInputSurvival : MonoBehaviour
    {
        public SurvivalPingPongMaster gameMaster;
        public PauseManager pauseManager;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
            bool gamePaused = pauseManager.IsPaused(); 
            
        
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch) && !gamePaused)
            {
                Debug.Log("Pulsante A premuto, tasto 'Pronto'.");
                gameMaster.Ready_Button();
            }


            if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch))
            {
                // Bottone Pausa
                pauseManager.PauseInput();
            }
        
        }
    }
}
