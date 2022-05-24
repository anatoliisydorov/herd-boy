using Dev.Services;
using UnityEngine;

namespace Dev.Core
{
    public interface ISmartFixedUpdatable
    {
        public void OnFixedUpdate();
    }

    public class SmartFixedUpdate: SmartUpdate, ISmartFixedUpdatable
    {
        protected override void OnEnabled()
        {
            base.OnEnabled();
            SingletoneServer.Instance.Get<CoreUpdater>().AddFixedUpdate(this);
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            SingletoneServer.Instance.Get<CoreUpdater>().RemoveFixedUpdate(this);
        }

        public virtual void OnFixedUpdate() {}
    }
}