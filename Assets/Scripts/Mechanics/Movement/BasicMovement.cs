using UnityEngine;
using Dev.Core;
using Dev.Time;
using System.Collections.Generic;

namespace Dev.Movement
{
    public interface IMovable
    {
        public bool IsBlocked { get; set; }

        public MovementJobData MoveData { get; }
        public Transform Transform { get; }

        public void Move(Vector2 movement);
        public void Move(Vector3 movement);
    }

    public class BasicMovement : SmartStart, IMovable
    {
        [SerializeField] private float _moveSpeed = 20f;
        [SerializeField] private float _rotateSpeed = 20f;

<<<<<<< Updated upstream
        public bool IsBlocked { get; set; }
        public Vector3 MoveStep { get; protected set; }
=======
        private bool _isBlocked;

        public MovementJobData MoveData { get; protected set; }
>>>>>>> Stashed changes
        public Transform Transform{ get => transform; }

        public float MoveSpeed 
        {
            get => _moveSpeed;
            set => _moveSpeed = value;
        }

        public float RotateSpeed
        {
            get => _rotateSpeed;
            set => _rotateSpeed = value;
        }
        
        protected override void OnEnabled()
        {
            base.OnEnabled();
            Dev.Services.SingletoneServer.Instance.Get<BasicMovementHandler>().AddBasicMovement(this);
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            Dev.Services.SingletoneServer.Instance.Get<BasicMovementHandler>().RemoveBasicMovement(this);
        }

        public void Rotate(Vector2 direction)
        {
            var newDirection = new Vector3(direction.x, 0f, direction.y);
            Rotate(newDirection);
        }

        public void Move(Vector2 input)
        {
<<<<<<< Updated upstream
            if (IsBlocked) return;
            
            Vector3 movement = new Vector3(input.x, 0f, input.y);
=======
            var movement = new Vector3(input.x, 0f, input.y);
>>>>>>> Stashed changes
            Move(movement);
        }

        public void Rotate(Vector3 direction)
        {
            var targetRotation = Quaternion.LookRotation(direction);

            var moveData = MoveData;
            moveData.TargetRotation = targetRotation;
            moveData.RotateSpeed = _rotateSpeed;

            MoveData = moveData;
        }

        public void Move(Vector3 movement)
        {
<<<<<<< Updated upstream
            if (IsBlocked) return;
            
            // var deltaTime = GameTime.DeltaTime;
            MoveStep = _speed * movement;
            // MoveStep = _speed * deltaTime * movement;
            
            // Debug.Log($"Move: deltatime: {deltaTime} == {MoveStep}");
=======
            var moveData = MoveData;
            moveData.Movement = movement;
            moveData.MoveSpeed = _moveSpeed;

            MoveData = moveData;
>>>>>>> Stashed changes
        }
    }
}