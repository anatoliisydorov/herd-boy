using UnityEngine;
using Dev.Core;
using Dev.Time;
using System.Collections.Generic;

namespace Dev.Movement
{
    public interface IMovable
    {
        public bool IsBlocked { get; set; }
        public Vector3 MoveStep { get; }
        public Transform Transform { get; }

        public void Move(Vector2 movement);
        public void Move(Vector3 movement);
    }

    public class BasicMovement : SmartStart, IMovable
    {
        [SerializeField] private float _speed = 20f;

        public bool IsBlocked { get; set; }
        public Vector3 MoveStep { get; protected set; }
        public Transform Transform{ get => transform; }

        public float Speed 
        {
            get => _speed;
            set => _speed = value;
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

        public void Move(Vector2 input)
        {
            if (IsBlocked) return;
            
            Vector3 movement = new Vector3(input.x, 0f, input.y);
            Move(movement);
        }

        public void Move(Vector3 movement)
        {
            if (IsBlocked) return;
            
            // var deltaTime = GameTime.DeltaTime;
            MoveStep = _speed * movement;
            // MoveStep = _speed * deltaTime * movement;
            
            // Debug.Log($"Move: deltatime: {deltaTime} == {MoveStep}");
        }
    }
}