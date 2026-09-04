using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PingPong
{
    public class FXManager : MonoBehaviour
    {
        public SfxPool sfxPool;
        public AudioClip playerServeClip;
        public AudioClip opponentServeClip;
        
        [SerializeField] private List<GameObject> confettiLaunchers;
        
        private AudioSource[] _confettiAudioSources;

        public ParticleSystem.MinMaxGradient playerColor;
        public ParticleSystem.MinMaxGradient opponentColor;
        
        public float scoredConfettiDuration = 1f;
        public float winConfettiDuration = 4f;
        
        
        public AudioClip winConfettiClip;

        void Start()
        {
            _confettiAudioSources = new AudioSource[confettiLaunchers.Count];
            for (int i = 0; i < confettiLaunchers.Count; i++)
            {
                confettiLaunchers[i].TryGetComponent(out _confettiAudioSources[i]); 
            }
        }
        
        public void ScoredFx(Team  team)
        {
            var color =  GetColor(team);
            PlayParticleEffects(scoredConfettiDuration, color, winConfettiClip);
        }

        public void WinMatchFx(Team team)
        {
            var color =  GetColor(team);
            PlayParticleEffects(winConfettiDuration, color, winConfettiClip);
        }

        public void PlayParticleEffects(float duration, ParticleSystem.MinMaxGradient color, AudioClip clip = null)
        {
            for(int i = 0; i < confettiLaunchers.Count; i++)
            {
                var launcher = confettiLaunchers[i];
                var lParSys = launcher.GetComponentInChildren<ParticleSystem>();
                lParSys.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var main = lParSys.main;
                main.startColor = color;
                main.duration = duration;
                lParSys.Play();

                if (clip && _confettiAudioSources[i])
                {
                    StartCoroutine(PlayClipForTime(_confettiAudioSources[i], clip, duration));
                }
            }
        }

        public void OpponentServe(Vector3 position)
        {
            sfxPool.PlayClipAtPosition(opponentServeClip, position, pitch:Random.Range(0.95f, 1.05f), priority:100);
        }

        public void PlayerServe(Vector3 position)
        {
            sfxPool.PlayClipAtPosition(playerServeClip, position, volume:0.3f , pitch:Random.Range(0.95f, 1.05f), priority:100);
        }

        private ParticleSystem.MinMaxGradient GetColor(Team team)
        {
            return team == Team.Player ? playerColor : opponentColor;
        }

        IEnumerator PlayClipForTime(AudioSource source, AudioClip clip, float duration, float dissolveTime = 1)
        {
            source.clip = clip;
            source.Play();
            float fullVolTime = duration - dissolveTime;
            if (fullVolTime > 0)
            {
                yield return WaitsCache.GetWaitForSeconds(fullVolTime);
            }
            var initialVol =  source.volume;
            float fadingTime = 0;
            while (fadingTime < dissolveTime)// && source.isPlaying // ottimizzazione utile se source può essere stoppata (ora non dovrebbe mai)
            {
                fadingTime += Time.deltaTime;
                source.volume = Mathf.Lerp(initialVol, 0, fadingTime / dissolveTime);
                //Debug.Log(fadingTime + " s: " +  source.volume);
                yield return null;
            }
            source.Stop();
            source.volume = initialVol;
        }

        public void PauseAll()
        {
            sfxPool.PauseAll();
            foreach (var source in _confettiAudioSources)
            {
                source.Pause();
            }
        }

        public void ResumeAll()
        {
            sfxPool.UnPauseAll();
            foreach (var source in _confettiAudioSources)
            {
                source.UnPause();
            }
        }
    }
}
