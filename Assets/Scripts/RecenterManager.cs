using UnityEngine;

public class RecenterManager : MonoBehaviour
{
    //[SerializeField] private Transform centerEyeAnchor;

    private void OnEnable()
    {
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

    // uso quello resetCameraPosition ora, possibile merge dei file
    // public void AlignViewTo(Transform target)
    // {
    //     float currentYaw = centerEyeAnchor.rotation.eulerAngles.y;
    //     float targetYaw = target.rotation.eulerAngles.y;
    //     float deltaYaw = targetYaw - currentYaw;
    //
    //     transform.RotateAround(centerEyeAnchor.position, Vector3.up, deltaYaw);
    // }

    /// Scatenato automaticamente quando l'utente preme a lungo il tasto Meta (reset vista)
    private void OnSystemRecentered()
    {
        // L'OS ha già raddrizzato il visore rispetto al mondo fisico.
        // Azzeriamo la rotazione Y  per evitare che le due rotazioni si sommino.
        Vector3 currentEuler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(currentEuler.x, 0f, currentEuler.z);
        Debug.Log("[RecenterManager] Recentered view!");
    }
}