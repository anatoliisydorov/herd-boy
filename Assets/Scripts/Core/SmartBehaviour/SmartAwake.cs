using Dev.Services;
using UnityEngine;

namespace Dev.Core
{
    public interface ISmartAwakable
    {
        public bool IsAwaked { get; }
        public void OnAwake();
        public void Enable();
    }

    public class SmartAwake : MonoBehaviour, ISmartAwakable
    {
        protected bool forceAwake;

        public bool IsAwaked { get; private set; }

        private void Awake()
        {
            if (forceAwake) OnAwake();
            else if (!IsAwaked)
            {
                SingletoneServer.Instance.Get<CoreUpdater>().AddAwake(this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (IsAwaked) OnEnabled();
        }

        private void OnDisable()
        {
            if (IsAwaked) OnDisabled();
        }

        protected virtual void OnEnabled() {}

        protected  virtual void OnDisabled() {}

        public virtual void OnAwake()
        {
            IsAwaked = true;
        }

        public void Enable()
        {
            enabled = true;
        }
    }
}