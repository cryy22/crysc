#region

using System.Collections;
using UnityEngine;

#endregion

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

        public static IEnumerator WrapRoutine(this ICoroutineController controller, IEnumerator routine)
        {
            Coroutine subroutine = controller.StartCoroutine(routine);
            if (subroutine == null)
                yield break;

            var endedNaturally = false;

            try
            {
                yield return subroutine;
                endedNaturally = true;
            }
            finally
            {
                if (!endedNaturally)
                    Debug.Log("Killed early!!");

                controller.StopCoroutine(subroutine);
                controller.ActiveCoroutine = null;
            }
        }
    }
}
