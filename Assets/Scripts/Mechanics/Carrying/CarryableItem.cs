using Dev.Actions;
using Dev.AimableMechanics;
using UnityEngine;

namespace Dev.Hands
{
    public interface ICarryable
    {
        Transform GetTransform();
        void PickUp();
        void PutDown();
        IThrowable GetThrowable();
    }

    [RequireComponent(typeof(ThrowingComponent))]
    [RequireComponent(typeof(Rigidbody))]
    public class CarryableItem : MonoBehaviour, ICarryable
    {
        [SerializeField] private VisibilityActions _visibilityActions;
        private ThrowingComponent _throwable;
        private Rigidbody _rigidbody;

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

        public IThrowable GetThrowable()
        {
            return _throwable;
        }

        public void PickUp()
        {
            _rigidbody.isKinematic = true;
        }

        public void PutDown()
        {
            _rigidbody.isKinematic = false;
        }
    }
}
