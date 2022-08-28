using UnityEngine;

namespace Dev.AimableMechanics
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class ThrowingWithPhysicsComponent: ThrowingComponent
    {
        private static float MAX_FORCE = 10f;
        private static float THROWING_RANDOM_OFFSET = .5f;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override float GetMass()
        {
            return _rigidbody.mass;
        }

        public override Vector3 CalculateVelocity(Vector3 aimingPoint)
        {
            var projectileXZPosition = new Vector3(transform.position.x, 0f, transform.position.z);
            var targetXZPosition = new Vector3(aimingPoint.x, 0f, aimingPoint.z);

            var distance = Vector3.Distance(projectileXZPosition, targetXZPosition);
            var gravity = Physics.gravity.y;
            var height = aimingPoint.y - transform.position.y;

            var angle = Vector3.Angle(aimingPoint - transform.position, targetXZPosition - projectileXZPosition);
            if (height < 0) angle *= -1;
            angle += 35f;
            angle = angle >= 90f ? 89f : angle;
            var tanAlpha = Mathf.Tan(angle * Mathf.Deg2Rad);
            var multiplier = height - distance * tanAlpha;

            float Vz = Mathf.Sqrt(gravity * distance * distance / (2f * multiplier));
            var Vy = tanAlpha * Vz;

            var velocity = new Vector3(0f, Vy, Vz);
            var velocityRotation = Quaternion.LookRotation(targetXZPosition - projectileXZPosition, Vector3.up);
            var finalVelocity = velocityRotation * velocity;
            finalVelocity = Vector3.ClampMagnitude(finalVelocity, MAX_FORCE);

            Debug.Log(finalVelocity);

            return finalVelocity;
        }

        protected override void OnThrow(Vector3 targetPoint)
        {
            targetPoint += new Vector3(Random.Range(-THROWING_RANDOM_OFFSET, THROWING_RANDOM_OFFSET), 0f, Random.Range(-THROWING_RANDOM_OFFSET, THROWING_RANDOM_OFFSET));
            _rigidbody.velocity = CalculateVelocity(targetPoint);
        }
    }
}
