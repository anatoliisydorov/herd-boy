using Dev.Services;
using UnityEngine;

namespace Dev.Core
{
    public interface ISmartStartable
    {
        public bool IsStarted { get; }
        public void OnStart();
    }

    public class SmartStart: SmartAwake, ISmartStartable
    {
        protected bool forceStart;

        public bool IsStarted { get; private set; }

        public override void OnAwake()
        {
            base.OnAwake();
            if (!forceStart) SingletoneServer.Instance.Get<CoreUpdater>().AddStart(this);
        }

        private void Start()
        {
            if (forceStart) OnStart();
        }
        
        public virtual void OnStart()
        {
            IsStarted = true;
            SingletoneServer.Instance.Get<CoreUpdater>().RemoveStart(this);
        }
    }
}