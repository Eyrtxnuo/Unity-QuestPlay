using UnityEngine;

namespace PingPong
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Rigidbody))]
    public class RedShilfing : MonoBehaviour
    {
        public float maxSpeed=20;
        public Gradient speedGradient;
    
        private Renderer _renderer;
        private Rigidbody _rigidbody;
    
        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _rigidbody = GetComponent<Rigidbody>();
        }
        // Update is called once per frame
        void Update()
        {
            float zSpeed = _rigidbody.linearVelocity.z;
            float normalizedSpeed = Mathf.Clamp01((zSpeed+maxSpeed) / (maxSpeed*2));
            var color = speedGradient.Evaluate(normalizedSpeed);
            _renderer.material.SetColor("_BaseColor", color);
        }
    }
}
