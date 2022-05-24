using System.Collections;
using UnityEngine;

namespace Dev.Services
{
    public class CoroutinesSystem: MonoBehaviour
    {
        public Coroutine BeginCoroutine(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }

        public void EndCouroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }

        public void EndAllCoroutines()
        {
            StopAllCoroutines();
        }
    }
}