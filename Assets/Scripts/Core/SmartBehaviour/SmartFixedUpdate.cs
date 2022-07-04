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
        protected override void OnEnable()
        {
            base.OnEnable();
            SingletoneServer.Instance.Get<CoreUpdater>().AddFixedUpdate(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SingletoneServer.Instance.Get<CoreUpdater>().RemoveFixedUpdate(this);
        }

        public virtual void OnFixedUpdate() {}
    }
}