using PingPong;
using UnityEngine;

public class StopHandTracking : MonoBehaviour
{
    public GameObject paddle;
    public PanelManager alertPanel;
    public PauseManager pauseManager;
    [Range(1, 120)]
    public int framesBetweenChecks;
    private int _counter;
    private bool _lostTracking = false;
    
    private bool _originalPaddleStatus;
    private float _originalTimeScale;
    
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
                _originalPaddleStatus = paddle.activeSelf;
                paddle.SetActive(false);
            }

            if (_lostTracking && activeController == OVRInput.Controller.RTouch)
            {
                _lostTracking = false;
                alertPanel.HidePanel();
                pauseManager.PauseLock(false);
                Time.timeScale = _originalTimeScale;
                paddle.SetActive(_originalPaddleStatus);
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
