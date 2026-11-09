#region

using System;
using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public static class AwaitableExtensions
    {
        public static async void RunIndefinitely(this Awaitable awaitable)
        {
            try
            {
                await awaitable;
            }
            catch (OperationCanceledException)
            { }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
