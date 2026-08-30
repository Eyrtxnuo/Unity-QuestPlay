using UnityEngine;
using UnityEngine.Animations;

namespace PingPong
{
    public class BouncesSfxManager :  MonoBehaviour
    {
        public SfxPool sourcePool;
        public Ball3D ball;
        [Range(0, 10)] public float volume = 1; 
        public AudioClip[] tableBounceClips;
        public AudioClip paddleBounceClipPlayer;
        public AudioClip paddleBounceClipOpponent;
        public AudioClip otherBounceClip;

        public void Start()
        {
            ball.OnAnyCollision += HandleBounce;
        }

        private void HandleBounce(Collision collision)
        {
            
            if (collision.gameObject.CompareTag("PingPongField"))
            {
                PlayTableBounce(collision.contacts[0].point);
            }
            else if (collision.gameObject.CompareTag("PingPongPlayerPaddle"))
            {
                PlayPlayerPaddleBounce(collision.contacts[0].point);
            }
            else if (collision.gameObject.CompareTag("PingPongOpponentPaddle"))
            {
                PlayOpponentPaddleBounce(collision.contacts[0].point);
            }
            else
            {
             if(otherBounceClip != null) PlayOtherBounce(collision.contacts[0].point);
            }
        }
    
        public void PlayTableBounce(Vector3 position)
        {
            sourcePool.PlayClipAtPosition(tableBounceClips[Random.Range(0, tableBounceClips.Length)], position, volume:volume, pitch:Random.Range(0.95f, 1.05f), priority:100);
        }

        public void PlayPlayerPaddleBounce(Vector3 position)
        {
            sourcePool.PlayClipAtPosition(paddleBounceClipPlayer, position, volume:volume, pitch:Random.Range(0.95f, 1.05f), priority:100);
        }

        public void PlayOpponentPaddleBounce(Vector3 position)
        {
            sourcePool.PlayClipAtPosition(paddleBounceClipOpponent, position, volume:volume, pitch:Random.Range(0.95f, 1.05f), priority:100);
        }

        public void PlayOtherBounce(Vector3 position)
        {
            sourcePool.PlayClipAtPosition(otherBounceClip, position, volume:volume, pitch:Random.Range(0.95f, 1.05f), priority:100);
        }
    }
}