using System.Collections;

namespace Crysc.Common.CoroutineControl
{
    public static class CoroutineControllerExtensions
    {
        public static void StartActiveCoroutine(this ICoroutineController controller, IEnumerator routine)
        {
            controller.StopActiveCoroutine();
            controller.ActiveCoroutine = controller.StartCoroutine(
                controller.WrapRoutine(routine)
            );
        }
        
        public static void StopActiveCoroutine(this ICoroutineController controller)
        {
            if (!controller.HasActiveCoroutine) 
                return;
            
            controller.StopCoroutine(controller.ActiveCoroutine);
            controller.ActiveCoroutine = null;
        }

        public static IEnumerator WaitForCompletion(this ICoroutineController controller)
        {
            while (controller.HasActiveCoroutine)
                yield return null;
        }

        private static IEnumerator WrapRoutine(this ICoroutineController controller, IEnumerator routine)
        {
            yield return routine;
            controller.ActiveCoroutine = null;
        }
    }
}
