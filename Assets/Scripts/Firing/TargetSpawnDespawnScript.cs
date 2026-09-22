using UnityEngine;

namespace Firing
{
    public class TargetSpawnDespawnScript : MonoBehaviour
    {
        public float AliveTime = 5f;

        private const float totalRotation = 720f;
        private const float animationTime = 0.8f;
        
        private float _timer = 0;
        private Vector3 _originalScale;
        private Vector3 _originalRotation;
        private Vector3 _startRotation;
        private int state = 0;

        private void Start()
        {
            _originalScale = transform.localScale;
            _originalRotation = transform.rotation.eulerAngles;
            _startRotation = transform.rotation.eulerAngles;
            _startRotation.y -= totalRotation;
        }

        private void Update()
        {
            Debug.Log("SpawnerTarget State: " + state);
            _timer += Time.deltaTime;
            switch (state)
            {
               case 0: // spawning
                   if (_timer <= animationTime)
                   {
                       this.transform.localScale = Vector3.Lerp(Vector3.zero, _originalScale, (_timer / animationTime));
                       this.transform.rotation = Quaternion.Euler(Vector3.Lerp(_startRotation, _originalRotation, (_timer / animationTime)));
                   }
                   else
                   {
                       state = 1;
                       this.transform.localScale =  _originalScale;
                       this.transform.rotation = Quaternion.Euler(_originalRotation);
                       _timer = 0;
                   }
                   break;
               case 1: // attesa despawn
                   if (_timer >= AliveTime)
                   {
                       state = 2;
                       _timer = 0;
                   }
                   break;
               case 2: // despawn
                   if (_timer <= animationTime)
                   {
                       this.transform.localScale = Vector3.Lerp(_originalScale, Vector3.zero, (_timer / animationTime));
                       this.transform.rotation = Quaternion.Euler(Vector3.Lerp(_originalRotation, _startRotation, (_timer / animationTime)));
                   }
                   else
                   {
                       Destroy(this.gameObject);
                   }
                   break;
            }
        }
    }
}