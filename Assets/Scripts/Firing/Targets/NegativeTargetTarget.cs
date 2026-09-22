using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing.Targets
{
    public class NegativeTargetTarget : TargetTarget
    {
        [field: NonSerialized] public override int Punteggio { get; set; } = -3;
        
    }
}