using Dev.Actions;
using Dev.AimableMechanics;
using System;
using UnityEngine;

namespace Dev.Hands
{
    public interface ICarryable
    {
        public bool IsCarryed { get; }
        public Action OnPickedUp { get; set; }
        public Action OnPuttedDown { get; set; }
        Transform GetTransform();
        Collider GetCollider();
        void PickUp();
        void PutDown();
        IThrowable GetThrowable();
    }

    [RequireComponent(typeof(ThrowingComponent))]
    [RequireComponent(typeof(Rigidbody))]
    public class CarryableItem : MonoBehaviour, ICarryable
    {
        [SerializeField] private VisibilityActions _visibilityActions;
        [SerializeField] private Collider _collider;

        private ThrowingComponent _throwable;
        private Rigidbody _rigidbody;

        public bool IsCarryed { get; private set; }
        public Action OnPickedUp { get; set; }
        public Action OnPuttedDown { get; set; }

        private void Awake()
        {
            _throwable = GetComponent<ThrowingComponent>();
            _rigidbody = GetComponent<Rigidbody>();

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

            IsCarryed = true;
            OnPickedUp?.Invoke();
        }

        public void PutDown()
        {
            _collider.enabled = true;
            _rigidbody.isKinematic = false;

            IsCarryed = false;
            OnPuttedDown?.Invoke();
        }
    }
}
