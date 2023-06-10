using UnityEngine;

namespace Crysc.Platforms
{
    public class ConditionalPlatformConfigurer : MonoBehaviour
    {
        [SerializeField] private bool ShowOnWindows = true;
        [SerializeField] private bool ShowOnWebGL = true;
        [SerializeField] private bool ShowOnLinux = true;
        [SerializeField] private bool ShowOnMacOS = true;
        [SerializeField] private bool ShowOnEditor = true;

        private void Awake()
        {
            if (IsWindows()) gameObject.SetActive(ShowOnWindows);
            if (IsWebGL()) gameObject.SetActive(ShowOnWebGL);
            if (IsLinux()) gameObject.SetActive(ShowOnLinux);
            if (IsMacOS()) gameObject.SetActive(ShowOnMacOS);
            if (IsEditor()) gameObject.SetActive(ShowOnEditor);
        }

        private bool IsWindows() { return Application.platform == RuntimePlatform.WindowsPlayer; }
        private bool IsWebGL() { return Application.platform == RuntimePlatform.WebGLPlayer; }
        private bool IsLinux() { return Application.platform == RuntimePlatform.LinuxPlayer; }
        private bool IsMacOS() { return Application.platform == RuntimePlatform.OSXPlayer; }
        private bool IsEditor() { return Application.isEditor; }
    }
}
