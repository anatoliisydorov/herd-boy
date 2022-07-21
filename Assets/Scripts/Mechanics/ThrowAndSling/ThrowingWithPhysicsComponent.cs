using UnityEngine;

namespace Dev.AimableMechanics
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class ThrowingWithPhysicsComponent: ThrowingComponent, IThrowable
    {

        private Rigidbody _rigidbody;

        private float _launchAngle = 45f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected override void OnThrow(Vector3 targetPoint)
        {
            var projectileXZPosition = new Vector3(transform.position.x, 0f, transform.position.z);
            var targetXZPosition = new Vector3(targetPoint.x, 0f, targetPoint.z);

            transform.LookAt(targetXZPosition);

            var distance = Vector3.Distance(projectileXZPosition, targetXZPosition);
            var gravity = Physics.gravity.y;
            var tanAlpha = Mathf.Tan(_launchAngle * Mathf.Deg2Rad);
            var height = targetPoint.y - transform.position.y;

            var Vz = Mathf.Sqrt(gravity * distance * distance / (2f * (height - distance * tanAlpha)));
            var Vy = tanAlpha * Vz;

            var localVelocity = new Vector3(0f, Vy, Vz);
            var globalVelocity = transform.TransformDirection(localVelocity);

            _rigidbody.velocity = globalVelocity;
        }
    }
}
