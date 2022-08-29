using System;
using UnityEngine;

namespace Dev.Actions
{
    [RequireComponent(typeof(Renderer))]
    public class VisibilityActions: MonoBehaviour
    {
        public Action OnBecomeVisibleCall;
        public Action OnBecomeInvisibleCall;

        private void OnBecameVisible()
        {
            OnBecomeVisibleCall?.Invoke();
        }

        private void OnBecameInvisible()
        {
            OnBecomeInvisibleCall?.Invoke();
        }
    }
}
