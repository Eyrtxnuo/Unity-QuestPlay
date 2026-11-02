using System.Collections;
using UnityEngine;

public class RecenterManager : MonoBehaviour
{
    [SerializeField] private Transform centerEyeAnchor;
    public bool preserveEyeHeight = true;

    private void OnEnable()
    {
        AlignViewTo((transform.parent?.forward)?? Vector3.forward);
        AlignPositionTo((transform.parent?.position) ?? Vector3.zero, preserveEyeHeight);
        // evento scatenato quando l'utente tiene premuto il tasto Meta
        if (OVRManager.display != null)
        {
            OVRManager.display.RecenteredPose += OnSystemRecentered;
        }
    }


    private void OnDisable()
    {
        if (OVRManager.display != null)
        {
            OVRManager.display.RecenteredPose -= OnSystemRecentered;
        }
    }
    
    
    public void AlignViewTo(Vector3 targetDir)
    {
        if (centerEyeAnchor == null) return;
        
        Vector3 direction = centerEyeAnchor.forward;
        direction.y = 0f;
        
        targetDir.y = 0f;
        if (direction.sqrMagnitude < 0.001f || targetDir.sqrMagnitude < 0.001f) return;// succede solo guardando perfettamente verso l'alto. boh non gestito
        float deltaYaw = Vector3.SignedAngle(direction, targetDir, Vector3.up);
        
        // ruota senza spostare la testa
        transform.RotateAround(centerEyeAnchor.position, Vector3.up, deltaYaw);
    }
    
    public void AlignPositionTo(Vector3 targetPos, bool preserveEyeHeight = true)
    {
        if (centerEyeAnchor == null) return;

        if (preserveEyeHeight)
        {
            targetPos.y = centerEyeAnchor.position.y;
        }

        transform.position += (targetPos - centerEyeAnchor.position);
    }
   

    /// Scatenato automaticamente quando l'utente preme a lungo il tasto Meta (reset vista)
    private void OnSystemRecentered()
    {
        StartCoroutine(OnSystemRecenteredCoroutine());
    }

    private IEnumerator OnSystemRecenteredCoroutine()
    {
        yield return new WaitForEndOfFrame();
        
        
        // L'OS ha già raddrizzato il visore rispetto al mondo fisico.
        // Azzeriamo la rotazione Y per evitare che le due rotazioni si sommino.
        Vector3 currentEuler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(currentEuler.x, 0f, currentEuler.z);
        AlignPositionTo((transform.parent?.position) ?? Vector3.zero, preserveEyeHeight);
        Debug.Log("[RecenterManager] Recentered view!");
    }
}