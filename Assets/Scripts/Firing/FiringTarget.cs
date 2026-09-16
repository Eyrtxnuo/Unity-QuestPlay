using System;
using UnityEngine;

namespace Firing
{
    [RequireComponent(typeof(Collider))]
    public class FiringTarget : MonoBehaviour
    {
        public virtual int Punteggio { get; set; } = 0;
        

        protected virtual int OnShot(Vector3 position)
        {
            Destroy(this.gameObject);
            return Punteggio;
        }

        public int Hit(Vector3 position)
        {
            return OnShot(position);
        }
    }
}