using UnityEditor;
using UnityEngine;

namespace Firing
{
    public class CanLauncher : MonoBehaviour
    {
        public GameObject canPrefab;
        public Vector3 defaultPosition;
        public float targetHeight;

        private float _prefabGravity;

        private void Start()
        {
            _prefabGravity = -canPrefab.GetComponent<CustomGravity>().gravityAcceleration;
        }

        public void LaunchCan(Vector3? target = null)
        {
            if (!canPrefab) return;
            target ??= transform.position;

            GameObject can = Instantiate(canPrefab, transform.position, Quaternion.identity);
            Rigidbody rb = can.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = CalculateTrajectory(transform.position, defaultPosition, targetHeight);
            }
        }

        private Vector3 CalculateTrajectory(Vector3 start, Vector3 target, float peakHeight)
        {
            // Calcola le distanze verticali dal picco (con un limite di sicurezza per evitare radici negative)
            float displacementYStart = Mathf.Max(0.1f, peakHeight - start.y);
            float displacementYTarget = Mathf.Max(0.1f, peakHeight - target.y);
            //Debug.Log("disps: " + displacementYStart + "  " + displacementYTarget);

            // Calcola la velocità necessaria sull'asse Y (moto uniformemente accelerato)
            float velocityY = Mathf.Sqrt(2f * _prefabGravity * displacementYStart);
            //Debug.Log("velocity: " + velocityY);
            
            // Calcola il tempo di salita (fino all'apice) e il tempo di discesa (fino al target)
            float timeUp = Mathf.Sqrt(2f * displacementYStart / _prefabGravity);
            float timeDown = Mathf.Sqrt(2f * displacementYTarget / _prefabGravity);
            float totalTime = timeUp + timeDown;
            //Debug.Log("totalTime: " + totalTime);

            // Calcola lo spostamento orizzontale e la velocità necessaria sugli assi X e Z (moto rettilineo uniforme)
            Vector3 displacementXZ = new Vector3(target.x - start.x, 0, target.z - start.z);
            Vector3 velocityXZ = displacementXZ / totalTime;
            //Debug.Log("velocityXZ: " + velocityXZ);
            //Debug.Break();

            // Ritorna il vettore velocità combinato
            return new Vector3(velocityXZ.x, velocityY, velocityXZ.z);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.darkOrange;
            Gizmos.DrawSphere(defaultPosition, 0.05f);
            
            Vector3 peakPosition = new Vector3(
                (transform.position.x + defaultPosition.x)/2, 
                targetHeight, 
                (transform.position.z + defaultPosition.z)/2
            ); 
            
            Gizmos.color = Color.brown;
            Gizmos.DrawIcon(peakPosition, "peak2");
        }
        
        //DEBUG
        void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
            {
                Debug.Log("Pulsante B premuto, lancio una birra in aria.");
                LaunchCan();
            }
        }
        
#endif
        
    }
}