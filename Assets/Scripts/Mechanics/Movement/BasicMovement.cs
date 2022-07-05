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

    public class BasicMovement : MonoBehaviour, IMovable
    {
        [SerializeField] private float _moveSpeed = 20f;
        [SerializeField] private float _rotateSpeed = 20f;

        public bool IsBlocked { get; set; }
        public MovementJobData MoveData { get; protected set; }
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
        
        private void OnEnable()
        {
            Dev.Services.SingletoneServer.Instance.Get<BasicMovementHandler>().AddBasicMovement(this);
        }

        private void OnDisable()
        {
            Dev.Services.SingletoneServer.Instance.Get<BasicMovementHandler>().RemoveBasicMovement(this);
        }

        public void Rotate(Vector2 direction)
        {
            var newDirection = new Vector3(direction.x, 0f, direction.y);
            Rotate(newDirection);
        }

        public void Move(Vector2 input)
        {
            var movement = new Vector3(input.x, 0f, input.y);
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
            var moveData = MoveData;
            moveData.Movement = movement;
            moveData.MoveSpeed = _moveSpeed;

            MoveData = moveData;
        }
    }
}