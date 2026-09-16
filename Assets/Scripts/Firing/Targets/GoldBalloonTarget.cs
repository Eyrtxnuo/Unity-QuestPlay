using System;
using UnityEngine;

namespace Firing.Targets
{
    public class GoldBalloonTarget : BalloonTarget
    {
        [field: NonSerialized] public override int Punteggio { get; set; } = 5;
        
    }
}