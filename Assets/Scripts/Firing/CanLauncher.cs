using Firing.Targets;
using UnityEngine;

namespace Firing
{
    public class CanLauncher : MonoBehaviour
    {
        public CanTarget canPrefab;
        public Vector3 defaultPosition;
        public float targetHeight;
        public float maxDeviation;
        public BoundArea canReturnArea;
        public SfxPool sfxPool;

        private float _prefabGravity;

        private void Start()
        {
            _prefabGravity = -canPrefab.GetComponent<CustomGravity>().gravityAcceleration;
        }

        public GameObject LaunchCan(Vector3? target = null)
        {
            if (!canPrefab) return null;
            var tar = target.HasValue ? target.Value : defaultPosition;
            tar.x += Random.Range(-maxDeviation, maxDeviation);
            tar.z += Random.Range(-maxDeviation, maxDeviation);

            CanTarget can = Instantiate(canPrefab, transform.position, Quaternion.identity);
            can.returnArea = canReturnArea; 
            can.sfxPool = sfxPool;
            Rigidbody rb = can.rb;
            if (rb)
            {
                rb.linearVelocity = CalculateTrajectory(transform.position, tar, targetHeight);
                rb.angularVelocity = new  Vector3(Random.Range(-10f, 10f), Random.Range(-10f,10f), Random.Range(-10f, 10f));
            }
            return can.gameObject;
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
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(defaultPosition,  new Vector3(maxDeviation*2, 0, maxDeviation*2));
            
            Vector3 peakPosition = new Vector3(
                (transform.position.x + defaultPosition.x)/2, 
                targetHeight, 
                (transform.position.z + defaultPosition.z)/2
            ); 
            
            Gizmos.color = Color.brown;
            Gizmos.DrawIcon(peakPosition, "peak2");
        }
#endif        
        //DEBUG
        void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
            {
                Debug.Log("Pulsante B premuto, lancio una birra in aria.");
                LaunchCan();
            }
        }
        

           
    }
}