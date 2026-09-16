using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace PingPong
{
    public abstract class WaitsCache
    {
        private static readonly Dictionary<float, WaitForSeconds> WaitCache = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds GetWaitForSeconds(float time)
        {
            if (!WaitCache.TryGetValue(time, out var waitObj))
            {
                waitObj = new WaitForSeconds(time);
                WaitCache.Add(time, waitObj);
            }

            return waitObj;
        }

        public static WaitForSeconds WAIT_100ms = GetWaitForSeconds(0.1f);//direct access and pre-added to dictionary
    }
}