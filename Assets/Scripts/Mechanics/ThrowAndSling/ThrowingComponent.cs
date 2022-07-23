using Dev.Actions;
using Dev.ObjectsManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dev.AimableMechanics
{
    public abstract class ThrowingComponent : MonoBehaviour, IThrowable
    {
        [SerializeField] private AssetReference _stopParticlesAsset;
        [SerializeField] private TriggerActions _trigger;

        protected abstract void OnThrow(Vector3 targetPoint);

        private void Awake()
        {
            if (_trigger != null)
            {
                _trigger.OnTriggerEnterCall += OnTriggerEnterCall;
            }
        }

        private void OnDestroy()
        {
            if (_trigger != null)
            {
                _trigger.OnTriggerEnterCall -= OnTriggerEnterCall;
            }
        }

        public void Throw(Vector3 targetPoint)
        {
            OnThrow(targetPoint);
        }

        public virtual float GetMass()
        {
            return 1f;
        }

        protected virtual void Stop()
        {
            if (_stopParticlesAsset != null)
            {
                GamePool.GetPoolable<ParticlePoolable>(_stopParticlesAsset, transform.position, transform.rotation);
                if (TryGetComponent(out IPoolable poolable))
                {
                    poolable.Deactivate();
                }
            }
        }

        private void OnTriggerEnterCall(Collider collider)
        {
            Stop();
        }
    }
}
