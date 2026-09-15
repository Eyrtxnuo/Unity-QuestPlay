using System;
using UnityEngine;

namespace PingPong
{
    public class PaddleSafeRespawn :  MonoBehaviour
    {

        public Material safeModeMaterial;
        public GameObject target; 
        public float safeDistance = 0.1f;
        public float safeTime = 0.15f;
        
        private Collider[] _colliders;
        private bool[] _originalCollisionEnabled;
        private Renderer[] _renderers;
        private Material[] _originalMaterials;

        private bool _listenEndSafeMode;
        private float _currentSafeTime = 0f;
        
        void Awake()
        {
            _colliders = GetComponentsInChildren<Collider>();
            _originalCollisionEnabled = new bool[_colliders.Length];
            for (int i = 0; i < _colliders.Length; i++)
            {
                _originalCollisionEnabled[i] = _colliders[i].enabled;
            }
            _renderers = GetComponentsInChildren<Renderer>();
            _originalMaterials = new Material[_renderers.Length];
            for (int i = 0; i < _renderers.Length; i++)
            {
                _originalMaterials[i] = _renderers[i].material;
            }
        }

        void Start()
        {
            EnableSafeMode();
            _listenEndSafeMode = true;
        }


        void Update()
        {
           if(!_listenEndSafeMode) return;
           if (CanExitSafeMode())
           {
               _currentSafeTime += Time.deltaTime;
           }
           else
           {
               _currentSafeTime = 0f;
           }

           if (_currentSafeTime > safeTime)
           {
               DisableSafeMode();
               _listenEndSafeMode = false;
           }
        }

        private bool CanExitSafeMode()
        {
            var point = target.transform.position;
            foreach (var col in _colliders)
            {
                var closest = col.bounds.ClosestPoint(point);
                var distance = Vector3.Distance(point, closest);
                if (distance < safeDistance)
                {
                    return false;
                }
            }
            return true;
        }
        
        public void EnableSafeMode(bool listenEndSafeMode = false)
        {
            _listenEndSafeMode = listenEndSafeMode;
            foreach(var col in _colliders) col.enabled = false;
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material = safeModeMaterial;
            }
        }
        
        public void DisableSafeMode()
        {
            for (int i = 0; i < _colliders.Length; i++)
            {
                _colliders[i].enabled = _originalCollisionEnabled[i];
            }

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material = _originalMaterials[i];
            }
        }
    }
}