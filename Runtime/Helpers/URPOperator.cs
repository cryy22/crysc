using Crysc.Patterns.OverlayCameras;
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
            baseCameraData.cameraStack.Sort((a, b) => a.GetOverlayIndex().CompareTo(b.GetOverlayIndex()));
        }

        public static void RemoveCameraFromStack(Camera overlayCamera)
        {
            Camera mainCamera = Camera.main;
            if (!mainCamera || !overlayCamera) return;

            UniversalAdditionalCameraData baseCameraData = mainCamera.GetUniversalAdditionalCameraData();
            if (baseCameraData.renderType == CameraRenderType.Base)
                baseCameraData.cameraStack.Remove(overlayCamera);
        }

        private static int GetOverlayIndex(this Camera camera)
        {
            return camera.GetComponent<OverlayIndexer>().Index;
        }
    }
}
