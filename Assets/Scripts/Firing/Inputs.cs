using System;
using PingPong;
using UnityEngine;

namespace Firing
{
    public class Inputs : MonoBehaviour
    {
        public PauseManager pauseManager;

        void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch))
            {
                pauseManager.PauseInput();
            }
        }
    }
}