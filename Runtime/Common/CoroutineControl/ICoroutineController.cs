using System.Collections;
using UnityEngine;

namespace Crysc.Common.CoroutineControl
{
    public interface ICoroutineController
    {
        Coroutine ActiveCoroutine { get; set; }
        bool HasActiveCoroutine => ActiveCoroutine != null;
        
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
    }
}
