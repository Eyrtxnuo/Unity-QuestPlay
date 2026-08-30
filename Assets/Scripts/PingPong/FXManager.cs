using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PingPong
{
    public class FXManager : MonoBehaviour
    {
        public SfxPool sfxPool;
        public AudioClip opponentServeClip;
        
        public List<GameObject> confettiLaunchers;

        public ParticleSystem.MinMaxGradient playerColor;
        public ParticleSystem.MinMaxGradient opponentColor;
        
        public float scoredConfettiDuration = 1f;
        public float winConfettiDuration = 4f;
    
        public void ScoredFx(Team  team)
        {
            var color =  GetColor(team);
            foreach (var launcher in confettiLaunchers)
            {
                var lParSys = launcher.GetComponentInChildren<ParticleSystem>();
                lParSys.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var main = lParSys.main;
                //main.stop
                main.startColor = color;
                main.duration = scoredConfettiDuration;
                lParSys.Play();
            }
            
        }

        public void WinMatchFx(Team team)
        {
            var color =  GetColor(team);
            foreach (var launcher in confettiLaunchers)
            {
                var lParSys = launcher.GetComponentInChildren<ParticleSystem>();
                lParSys.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var main = lParSys.main;
                main.startColor = color;
                main.duration = winConfettiDuration;
                lParSys.Play();
            }
        }

        public void OpponentServe(Vector3 position)
        {
            sfxPool.PlayClipAtPosition(opponentServeClip, position, pitch:Random.Range(0.95f, 1.05f));
            
        }

        private ParticleSystem.MinMaxGradient GetColor(Team team)
        {
            return team == Team.Player ? playerColor : opponentColor;
        }
    }
}
