using Dev.Movement;
using UnityEngine;

namespace Dev.AimableMechanics
{
    [RequireComponent(typeof(BasicMovement))]
    public class ThrowingStraightComponent : ThrowingComponent, IThrowable
    {
        [SerializeField] private float _speed = 10f;

        private BasicMovement _movement;

        private void Awake()
        {
            _movement = GetComponent<BasicMovement>();
            _movement.MoveSpeed = _speed;
        }

        protected override void OnThrow(Vector3 targetPoint)
        {
            var direction = (targetPoint - transform.position).normalized;
            _movement.Move(direction);
            _movement.IsBlocked = false;
        }

        protected override void Stop()
        {
            base.Stop();
            _movement.IsBlocked = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger)
                Stop();
        }
    }
}