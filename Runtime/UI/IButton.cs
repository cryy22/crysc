#region

using System;
using UnityEngine;

#endregion

namespace Crysc.UI
{
    public abstract class EventButton : MonoBehaviour
    {
        public event Action Clicked;

        public abstract void SetText(string text);

        protected void InvokeClicked()
        {
            Clicked?.Invoke();
        }
    }
}
