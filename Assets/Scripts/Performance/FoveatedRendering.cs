using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Performance
{
    public class FoveatedRendering : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float foveationLevel = 1.0f; // 0.25 = Low, 0.5 = Medium, 1.0 = High

        void Start()
        {
            List<XRDisplaySubsystem> xrDisplays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(xrDisplays);

            if (xrDisplays.Count > 0)
            {
                xrDisplays[0].foveatedRenderingLevel = foveationLevel;
                Debug.Log($"[XR] Foveated Rendering impostato a: {xrDisplays[0].foveatedRenderingLevel}");
            }
            
            Debug.Log($"[XR] Eye Texture Scale: {XRSettings.eyeTextureResolutionScale} - Size: {XRSettings.eyeTextureWidth}x{XRSettings.eyeTextureHeight}");
        }
    }
}