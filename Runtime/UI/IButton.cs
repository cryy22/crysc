using System;
using UnityEngine;

namespace Crysc.UI
{
    public abstract class EventButton : MonoBehaviour
    {
        public event EventHandler Clicked;

        protected void InvokeClicked()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
