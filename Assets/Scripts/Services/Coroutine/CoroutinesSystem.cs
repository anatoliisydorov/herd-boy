using System;
using System.Collections;
using UnityEngine;

namespace Dev.Services
{
    public class CoroutinesSystem: MonoBehaviour
    {
        public static Coroutine BeginCoroutine(IEnumerator enumerator)
        {
            return SingletoneServer.Instance.Get<CoroutinesSystem>().LocalBeginCoroutine(enumerator);
        }
        public static void EndCouroutine(Coroutine coroutine)
        {
            SingletoneServer.Instance.Get<CoroutinesSystem>().LocalEndCouroutine(coroutine);
        }
        public static void EndAllCoroutines()
        {
            SingletoneServer.Instance.Get<CoroutinesSystem>().LocalEndAllCoroutines();
        }

        public static Coroutine DelayedAction(float delay, Action action)
        {
            var system = SingletoneServer.Instance.Get<CoroutinesSystem>();
            return system.LocalBeginCoroutine(system.YIELD_DelayedAction(delay, action));
        }

        public static Coroutine DelayedAfterFramesAction(int frameCount, Action action)
        {
            var system = SingletoneServer.Instance.Get<CoroutinesSystem>();
            return system.LocalBeginCoroutine(system.YIELD_DelayedAfterFramesAction(frameCount, action));
        }


        public Coroutine LocalBeginCoroutine(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }

        public void LocalEndCouroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }

        public void LocalEndAllCoroutines()
        {
            StopAllCoroutines();
        }

        public IEnumerator YIELD_DelayedAction(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        public IEnumerator YIELD_DelayedAfterFramesAction(float frameCount, Action action)
        {
            for (int i = 0; i < frameCount; i++)
                yield return null;

            action?.Invoke();
        }
    }
}