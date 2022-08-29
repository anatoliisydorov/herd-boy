using Dev.Core;
using System;
using UnityEngine;

namespace Dev.Actions
{
    public class MovementActions: SmartUpdate
    {
        private static float DIFF_DISTANCE = .5f;
        private static float DIFF_ANGLE = 1f;

        public Action<Vector3> OnMovementCall;
        public Action<Quaternion> OnRotateCall;

        private Vector3 _previousPosition;
        private Quaternion _previousRotation;

        public override void OnUpdate()
        {
            base.OnUpdate();

            CheckPosition();
            CheckRotation();
        }

        private void CheckPosition()
        {
            if (Vector3.Distance(transform.position, _previousPosition) < DIFF_DISTANCE) return;

            OnMovementCall?.Invoke(transform.position - _previousPosition);
            _previousPosition = transform.position;
        }

        private void CheckRotation()
        {
            if (Quaternion.Angle(transform.rotation, _previousRotation) < DIFF_ANGLE) return;

            OnRotateCall?.Invoke(transform.rotation);
            _previousRotation = transform.rotation;
        }
    }
}
