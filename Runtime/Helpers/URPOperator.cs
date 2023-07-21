using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Crysc.Helpers
{
    public static class URPOperator
    {
        public static void AddCameraToStack(Camera overlayCamera)
        {
            UniversalAdditionalCameraData baseCameraData = Camera.main.GetUniversalAdditionalCameraData();
            baseCameraData.cameraStack.Add(overlayCamera);
        }

        public static void RemoveCameraFromStack(Camera overlayCamera)
        {
            UniversalAdditionalCameraData baseCameraData = Camera.main.GetUniversalAdditionalCameraData();
            baseCameraData.cameraStack.Remove(overlayCamera);
        }
    }
}
