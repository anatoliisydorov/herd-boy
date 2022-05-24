using Dev.Services;
using UnityEngine;

namespace Dev.Core
{
    public interface ISmartUpdatable
    {
        public void OnUpdate();
    }

    public class SmartUpdate: SmartStart, ISmartUpdatable
    {
        protected bool forceUpdate;

        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (forceUpdate) SingletoneServer.Instance.Get<CoreUpdater>().AddForceUpdate(this);
            else SingletoneServer.Instance.Get<CoreUpdater>().AddUpdate(this);
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            if (forceUpdate) SingletoneServer.Instance.Get<CoreUpdater>().RemoveForceUpdate(this);
            else SingletoneServer.Instance.Get<CoreUpdater>().RemoveUpdate(this);
        }

        public virtual void OnUpdate() { }
    }
}