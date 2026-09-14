using System;
using UnityEngine;

namespace Firing
{
    [RequireComponent(typeof(Collider))]
    public class FiringTarget : MonoBehaviour
    {
        public virtual int Punteggio { get; set; } = 0;

        void OnShot(Vector3 position)
        {
            Destroy(this.gameObject);
        }
    }
}