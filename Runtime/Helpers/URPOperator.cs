using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Crysc.Helpers
{
    public static class URPOperator
    {
        public static void AddCameraToStack(Camera overlayCamera)
        {
            Camera mainCamera = Camera.main;
            if (!mainCamera || !overlayCamera) return;

            UniversalAdditionalCameraData baseCameraData = mainCamera.GetUniversalAdditionalCameraData();
            baseCameraData.cameraStack.Add(overlayCamera);
        }

        public static void RemoveCameraFromStack(Camera overlayCamera)
        {
            Camera mainCamera = Camera.main;
            if (!mainCamera || !overlayCamera) return;

            UniversalAdditionalCameraData baseCameraData = mainCamera.GetUniversalAdditionalCameraData();
            baseCameraData.cameraStack.Remove(overlayCamera);
        }
    }
}
