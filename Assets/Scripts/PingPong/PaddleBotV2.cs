using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace PingPong
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PaddleBotV2 : MonoBehaviour
    {

        

        public Ball3D ball;
        public GameObject bounceDirTarget;
        public Collider tableTopCollider;
        public GameObject testTargetPrefab;
        public Difficulty difficulty = Difficulty.Easy;
        
        
        private DifficultyParams _config;
        private Rigidbody _rb;
        private Collider[] _cols;
        private (Vector3 position, Vector3 velocity, Vector3 angularVelocity) _lastTargetState;
        private float _bounceFactor = 1.4f;
        private bool _isTurn;
        [NonSerialized] public bool bouncedInBotField = false;

        void Start()
        {
            SetDifficulty(difficulty);
            
            // Time.timeScale = 1f;
            // Time.fixedDeltaTime = 0.005f * Time.timeScale;
            _rb = GetComponent<Rigidbody>();
            _cols = GetComponentsInChildren<Collider>();
        
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders) //Ricerca dell'elemento a mano visto che unity < css
            {
                if (col.material == null) continue;
                if (!col.gameObject.CompareTag("MainSubcomponent")) continue;
                _bounceFactor = col.material.bounciness;
                break;
            }
            _lastTargetState = (_rb.position, _rb.linearVelocity, _rb.angularVelocity);
        }

        private void Update()
        {
            Debug.DrawLine(this.transform.position + new Vector3(0, 0.13f, 0), this.transform.position + this.transform.forward*0.3f + new Vector3(0, 0.13f, 0), Color.coral);
        }

        void FixedUpdate()
        {
            if(!_isTurn)return;
            Debug.DrawLine(_rb.position, _rb.position + new Vector3(0, 0.13f, 0), Color.tomato);
            // Go to predicted bounce position
            if (!bouncedInBotField)
            {
                var bouncePositionsNullable = ball.GetBounceStates(0.25f, _config.ignoreMagnus);
                if (bouncePositionsNullable.HasValue)
                {
                    var fallPosition = bouncePositionsNullable.Value.fallPosition;
                    var bounceS = bouncePositionsNullable.Value.afterBounceState;
                    if (!(fallPosition.x > tableTopCollider.bounds.max.x + _config.borderSimulationSlack ||
                          fallPosition.x < -tableTopCollider.bounds.max.x - _config.borderSimulationSlack ||
                          fallPosition.z > 0 ||
                          fallPosition.z < -tableTopCollider.bounds.max.z - _config.borderSimulationSlack ||
                          bounceS.position.z > -0.4
                        ))
                    {

                        testTargetPrefab.transform.position = bounceS.position;
                        this._lastTargetState = bounceS;
                    }

                    ;
                }
            }

            var tPos = this._lastTargetState.position;
            Debug.DrawRay(tPos, new Vector3(0, 10, 0), Color.red);

        
            if (
                true//bounceS.velocity.normalized.z < 0
                &&
                Vector3.Distance(ball.transform.position, this.transform.position) > 0.2 // when ball is close, freeze to avoid orbital launches
            )
            {   
                _rb.MovePosition(Vector3.MoveTowards(_rb.position, tPos, _config.speed * Time.fixedDeltaTime));
                _rb.MoveRotation(Quaternion.RotateTowards(this.transform.rotation, CalculatePaddleRotation(this._lastTargetState), _config.rotationSpeed *  Time.fixedDeltaTime));
            }
            else
            {
                CalculatePaddleRotation(this._lastTargetState);// serve per disegnare i debug, non applico l'angolo.
            
            }
            //this.transform.forward = CalculatePaddleRotation();
        
        
            Debug.DrawLine(tPos, _rb.position, Color.chartreuse);
        }


        Quaternion CalculatePaddleRotation((Vector3 position, Vector3 velocity, Vector3 angularVelocity)bounceState)
        {
            float horizontalTravel = Mathf.Abs(bounceDirTarget.transform.position.z - this.transform.position.z);
            float totalTravel = Vector3.Distance(bounceDirTarget.transform.position, this.transform.position);
            float horizontalRatio = totalTravel > 0.01f ? (horizontalTravel / totalTravel) : 1f;
        
            float currentSpeed = Mathf.Abs(bounceState.velocity.magnitude);
            float afterBounceSpeed = currentSpeed * _bounceFactor;
            if (afterBounceSpeed < 0.1f) afterBounceSpeed = 0.1f;
        
            float equivalentHorizontalSpeed = afterBounceSpeed * horizontalRatio;
            float drag = ball.GetRigidBody().linearDamping;

            float fligthETA;
            if (drag <= 0.001f)
            {   //no drag approximation
                fligthETA = horizontalTravel /  equivalentHorizontalSpeed;
            }
            else
            {
                //decayng factor
                float decayingFactor = (horizontalTravel * drag) / equivalentHorizontalSpeed;
        
                if (decayingFactor >= 1f)
                {
                    //ball won't reach middle, handle by adding speed?
                    fligthETA = horizontalTravel /  equivalentHorizontalSpeed * 2f; 
                    Debug.LogWarning("Cant Reach other side!");
                }
                else
                {
                    // Ballistic formula:
                    fligthETA = -(1f / drag) * Mathf.Log(1f - decayingFactor);
                }
            }
        
        
            float gravity = Mathf.Abs(Physics.gravity.y);
            float gravityHeigthLoss = 0.5f * gravity * (Mathf.Pow(fligthETA, 2));
            Vector3 compensatedTarget = bounceDirTarget.transform.position + new Vector3(0, gravityHeigthLoss, 0);
        
            Vector3 inverseTravelDirection = -bounceState.velocity.normalized;
        
            Vector3 targetBouncePos = new Vector3(bounceState.position.x, bounceState.position.y, this.transform.position.z);
            Vector3 targetBounceDir = (compensatedTarget - targetBouncePos).normalized;
        
            Vector3 targetBounceSurfaceNormal = (targetBounceDir + inverseTravelDirection).normalized;
        
        
            Debug.DrawLine(targetBouncePos, targetBouncePos + targetBounceSurfaceNormal, Color.cyan);
        
        
            Debug.DrawLine(bounceDirTarget.transform.position, bounceDirTarget.transform.position + new Vector3(0,0,0.2f), Color.blue);
            Debug.DrawLine(bounceDirTarget.transform.position, bounceDirTarget.transform.position + new Vector3(0,0.2f,0), Color.green);
            Debug.DrawLine(bounceDirTarget.transform.position, bounceDirTarget.transform.position + new Vector3(0.2f,0,0), Color.red);
            Debug.DrawLine(compensatedTarget, compensatedTarget + new Vector3(0,0,0.2f), Color.Lerp(Color.blue, Color.black, .5f));
            Debug.DrawLine(compensatedTarget, compensatedTarget + new Vector3(0,0.2f,0), Color.Lerp(Color.green, Color.black, .5f));
            Debug.DrawLine(compensatedTarget, compensatedTarget + new Vector3(0.2f,0,0), Color.Lerp(Color.red, Color.black, .5f));
    
            Quaternion nuovaRotazione = Quaternion.FromToRotation(Vector3.forward, targetBounceSurfaceNormal);
            return nuovaRotazione;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("PingPongBall"))
            {
                 Rigidbody brb = collision.gameObject.GetComponent<Rigidbody>();
                 Ball3D bScript =  collision.gameObject.GetComponent<Ball3D>();
                 Vector3 currVel = collision.relativeVelocity;
                 Vector3 collNormal = collision.contacts[0].normal;
                 Vector3 bounceVel = Vector3.Reflect(currVel, collNormal);//collision.contacts[0].normal);
                 
                 
                StartCoroutine(IgnoreCollisionsFor(collision.collider, _cols));
                 // change bounce speed
                //bounceVel = bounceVel.normalized * 3.5f; 
                brb.linearVelocity = bounceVel;
                var fPos = ball.PredictFallPosition();
                if(!fPos.HasValue) return;
                var currentDistance = collision.contacts[0].point.z - fPos.Value.z;
                var targetDistance = collision.contacts[0].point.z - bounceDirTarget.transform.position.z;
                var factor = Mathf.Clamp(targetDistance / currentDistance, _config.minSpeedMuliplier, _config.maxSpeedMuliplier) * RandMult();
                bounceVel.z *= factor;
                brb.linearVelocity = bounceVel;
                
                _isTurn = false;
            }
        }

        public void SetTurn(bool isTurn)
        {
            _isTurn = isTurn;
            Debug.Log("Turn " + (_isTurn?"yes":"no") );
            GetComponentInChildren<Renderer>().material.SetColor("_BaseColor", isTurn ? Color.red : Color.white);
        }

        public bool GetTurn()
        {
            return _isTurn;
        }

        public void ClearTargetState(Vector3 position)
        {
            this._lastTargetState = (position, Vector3.zero, Vector3.zero);
        }

        private IEnumerator IgnoreCollisionsFor(Collider mainCol, Collider[] otherCols, float time = 0.1f)
        {
            foreach (var colW in otherCols)
            {
                Physics.IgnoreCollision(mainCol, colW, true);
            }
            yield return WaitsCache.GetWaitForSeconds(time);
            foreach (var colW in otherCols)
            {
                Physics.IgnoreCollision(mainCol, colW, false);
            }
        }
        
        public enum Difficulty
        {
            Easy,
            Medium,
            Hard,
            Max
        }

        public void SetDifficulty(Difficulty diff)
        {
            _config = DifficultyParams.GetFromDifficulty(diff); // set Difficulty
        }

        private float RandMult()
        {
            return 1 + Random.Range(-_config.generalizedError, _config.generalizedError);
        }
        
        private struct DifficultyParams
        {
            public float speed;
            public float rotationSpeed;
            public bool ignoreMagnus;
            public float maxSpeedMuliplier;
            public float minSpeedMuliplier;
            public float generalizedError;
            public float borderSimulationSlack;

            public static readonly DifficultyParams Easy = new DifficultyParams { speed = 3f,  rotationSpeed = 300f,  ignoreMagnus = true, maxSpeedMuliplier = 1.2f, minSpeedMuliplier = 1, generalizedError = 0.3f, borderSimulationSlack = -0.2f};
            public static readonly DifficultyParams Medium = new DifficultyParams { speed = 6f,  rotationSpeed = 300f,  ignoreMagnus = true, maxSpeedMuliplier =  1.5f, minSpeedMuliplier = 0.9f, generalizedError = 0.2f, borderSimulationSlack = -0.1f};
            public static readonly DifficultyParams Hard = new DifficultyParams { speed = 8f,  rotationSpeed = 500f,  ignoreMagnus = false, maxSpeedMuliplier =  1.8f, minSpeedMuliplier = 0.7f, generalizedError = 0.1f, borderSimulationSlack = 0f};
            public static readonly DifficultyParams Max = new DifficultyParams { speed = 10f,  rotationSpeed = 600f,  ignoreMagnus = false, maxSpeedMuliplier = 6,  minSpeedMuliplier = 0.1f, generalizedError = 0, borderSimulationSlack = 0.2f};

            public static DifficultyParams GetFromDifficulty(Difficulty difficulty)
            {
                return difficulty switch
                {
                    Difficulty.Easy => Easy,
                    Difficulty.Medium => Medium,
                    Difficulty.Hard => Hard,
                    Difficulty.Max => Max,
                    _ => Easy
                };
            }
        }
    }
}