using Dev.Core;
using Dev.Movement;
using Dev.ObjectsManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dev.AimableMechanics
{
    [RequireComponent(typeof(BasicMovement))]
    public class ThrowingComponent : MonoBehaviour, IThrowable
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private AssetReference _stopParticlesAsset;

        private BasicMovement _movement;

        private void Awake()
        {
            _movement = GetComponent<BasicMovement>();
            _movement.MoveSpeed = _speed;
        }

        public virtual void Throw(Vector3 direction)
        {
            _movement.Move(direction);
            _movement.IsBlocked = false;
        }

        protected virtual void Stop()
        {
            _movement.IsBlocked = true;
            if (_stopParticlesAsset != null)
            {
                GamePool.GetPoolable<ParticlePoolable>(_stopParticlesAsset, transform.position, transform.rotation);
                if (TryGetComponent(out IPoolable poolable))
                {
                    poolable.Deactivate();
                }
                //Instantiate(_stopParticles, transform.position, transform.rotation);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger)
                Stop();
        }
    }
}