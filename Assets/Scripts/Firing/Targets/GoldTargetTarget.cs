using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing.Targets
{
    public class GoldTargetTarget : TargetTarget
    {
        [field: NonSerialized] public override int Punteggio { get; set; } = 10;
        
    }
}