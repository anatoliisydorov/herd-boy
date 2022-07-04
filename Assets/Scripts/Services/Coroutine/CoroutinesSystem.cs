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
    }
}