using Dev.Actions;
using Dev.AimableMechanics;
using UnityEngine;

namespace Dev.Hands
{
    public interface ICarryable
    {
        Transform GetTransform();
        Collider GetCollider();
        void PickUp();
        void PutDown();
        IThrowable GetThrowable();
    }

    [RequireComponent(typeof(ThrowingComponent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class CarryableItem : MonoBehaviour, ICarryable
    {
        [SerializeField] private VisibilityActions _visibilityActions;
        private ThrowingComponent _throwable;
        private Rigidbody _rigidbody;
        private Collider _collider;

        private void Awake()
        {
            _throwable = GetComponent<ThrowingComponent>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _visibilityActions.OnBecomeVisibleCall += () => _rigidbody.isKinematic = false;
            _visibilityActions.OnBecomeInvisibleCall += () => _rigidbody.isKinematic = true;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public Collider GetCollider()
        {
            return _collider;
        }

        public IThrowable GetThrowable()
        {
            return _throwable;
        }

        public void PickUp()
        {
            _collider.enabled = false;
            _rigidbody.isKinematic = true;
        }

        public void PutDown()
        {
            _collider.enabled = true;
            _rigidbody.isKinematic = false;
        }
    }
}
