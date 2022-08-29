using Dev.Services;
using UnityEngine;

namespace Dev.Core
{
    public interface ISmartUpdatable
    {
        public void OnUpdate();
    }

    public interface ISmartLateUpdatable
    {
        public void OnLateUpdate();
    }

    public class SmartUpdate: MonoBehaviour, ISmartUpdatable, ISmartLateUpdatable
    {
        [SerializeField] protected bool enableLateUpdate;
        protected bool forceUpdate;

        protected virtual void OnEnable()
        {
            if (forceUpdate) SingletoneServer.Instance.Get<CoreUpdater>().AddForceUpdate(this);
            else SingletoneServer.Instance.Get<CoreUpdater>().AddUpdate(this);

            if (enableLateUpdate) SingletoneServer.Instance.Get<CoreUpdater>().AddForceUpdate(this);
        }

        protected virtual void OnDisable()
        {
            if (forceUpdate) SingletoneServer.Instance.Get<CoreUpdater>().RemoveForceUpdate(this);
            else SingletoneServer.Instance.Get<CoreUpdater>().RemoveUpdate(this);
        }

        public virtual void OnUpdate() { }
        public virtual void OnLateUpdate() { }
    }
}