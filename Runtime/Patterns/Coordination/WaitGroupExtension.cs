using System.Collections;

namespace Crysc.Patterns.Coordination
{
    public static class WaitGroupExtension
    {
        public static IEnumerator WithWaitGroup(this IEnumerator enumerator, WaitGroup waitGroup)
        {
            waitGroup.Join();
            yield return enumerator;
            waitGroup.Leave();
        }

        public static IEnumerator WithWaitGroupSynchronously(this IEnumerator enumerator, WaitGroup waitGroup)
        {
            yield return waitGroup.Wait();
            yield return enumerator.WithWaitGroup(waitGroup);
        }
    }
}
