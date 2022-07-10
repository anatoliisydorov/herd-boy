using System;
using UnityEngine;

namespace Dev.Actions
{
    [RequireComponent(typeof(Collider))]
    public class TriggerActions: MonoBehaviour
    {
        public Action<Collider> OnTriggerEnterCall;
        public Action<Collider> OnTriggerExitCall;

        private void OnTriggerEnter(Collider other)
        {
            if (other != null) OnTriggerEnterCall?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other != null) OnTriggerExitCall?.Invoke(other);
        }
    }
}
