using UnityEngine;

public class resetCameraPosition : MonoBehaviour
{
    public GameObject cameraObject;    
    

    private void Update()
    {
        AlignToZPositive();
        this.enabled = false;
    }
    
    
    public void AlignToZPositive()
    {
        if (cameraObject is null) return;
        
        Vector3 direction = cameraObject.transform.forward;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;// succede solo guardando perfettamente verso l'alto? boh non gestito
        Vector3 targetDir = Vector3.forward;
        // targetDir.y = 0f; /// servirebbe, ma Vector3.forward ha già y = 0
        
        float angle = Vector3.SignedAngle(direction, targetDir, Vector3.up);
        transform.Rotate(0f, angle, 0f, Space.World);
    }
}
