using System.Collections;
using System.Runtime.CompilerServices;
using LocomotionCollision;
using PingPong;
using UnityEngine;

namespace CowRun
{
    [RequireComponent(typeof(Animator))]
    public class ScarecrowKing : Scarecrow
    {
        private Vector3 _jumpDistance = new Vector3(0.3f, 0, 0);
        private bool _dirRight = true;

        public SplineLocomotionCollision roadSpline;
        public float directionReverseBorder = 0.5f;
        public float angleOffset = 20;
    
        //public override int PointsValue { get; protected set; } = 10;

        protected override void Start()
        {
            PointsValue = 10;
            base.Start();
            StartCoroutine(StartAnimationRoutine());
        }
        


        private IEnumerator StartAnimationRoutine()
        {
            yield return null;
            EseguiBalzoLaterale();
        }
        
        
        
        public void EseguiBalzoLaterale()
        {
            if(this.IsHit) return;// when hit everything stops
            
            var (closestPoint, distance, f, splineTangent) = roadSpline.GetClosestPointAndTangent(container.transform.position);
            
            //Reverse?
            if ((roadSpline.roadWidth/2) - directionReverseBorder < distance)
            {
                _dirRight = !_dirRight;
            }
            
            //angle the container
            var actualOffset = _dirRight ? -angleOffset : angleOffset;
            Vector3 rotatedTangent = Quaternion.Euler(0, actualOffset, 0) * splineTangent;
            container.transform.rotation = Quaternion.LookRotation(rotatedTangent, Vector3.up);
            


            //animation instance
            if (_dirRight)
            {
                StartCoroutine(JumpSequenceRoutine("LateralJumpRight", _jumpDistance));
            }
            else
            {
                StartCoroutine(JumpSequenceRoutine("LateralJumpLeft", FlipX(_jumpDistance)));
            }
        }

        private IEnumerator JumpSequenceRoutine(string animationName = null, Vector3 positionOffset = new Vector3())
        {
            _animator.Play(animationName, 0, 0);
            yield return null;
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float animationDuration = stateInfo.length;

            yield return WaitsCache.GetWaitForSeconds(animationDuration);// waits for the animation end
            
            if(!this.IsHit){// skip tp if during animation the scarecrow got hit
                container.transform.position += container.transform.TransformDirection(positionOffset);
            }
            
            //Debug.Log("Balzo fatto");

            EseguiBalzoLaterale();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 FlipX(Vector3 vector)
        {
            vector.x = -vector.x;
            return vector;
        }
    }
}