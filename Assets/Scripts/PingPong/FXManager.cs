using System;
using System.Collections.Generic;
using UnityEngine;

namespace PingPong
{
    public class FXManager : MonoBehaviour
    {
        public List<GameObject> confettiLaunchers;

        public ParticleSystem.MinMaxGradient playerColor;
        public ParticleSystem.MinMaxGradient opponentColor;
        
        public float scoredConfettiDuration = 1f;
        public float winConfettiDuration = 4f;
    
        public void ScoredSfx(Team  team)
        {
            var color =  GetColor(team);
            foreach (var launcher in confettiLaunchers)
            {
                var lParSys = launcher.GetComponentInChildren<ParticleSystem>();
                var main = lParSys.main;
                main.startColor = color;
                main.duration = scoredConfettiDuration;
                lParSys.Play();
            }
            
        }

        public void WInMatchSfx(Team team)
        {
            var color =  GetColor(team);
            foreach (var launcher in confettiLaunchers)
            {
                var lParSys = launcher.GetComponentInChildren<ParticleSystem>();
                var main = lParSys.main;
                main.startColor = color;
                main.duration = winConfettiDuration;
                lParSys.Play();
            }
        }


        private ParticleSystem.MinMaxGradient GetColor(Team team)
        {
            return team == Team.Player ? playerColor : opponentColor;
        }
    }
}
