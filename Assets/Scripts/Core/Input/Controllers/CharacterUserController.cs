using Dev.Character;
using Dev.Core;
using Dev.Movement;
using Dev.Services;
using UnityEngine;

namespace Dev.Input
{
    public class CharacterUserController: BaseController
    {
        private InputSystem _inputSystem;

        private Transform _cameraTransform;
        private PlayerCharacter _player;

        public override void OnAwake()
        {
            base.OnAwake();

            _inputSystem = SingletoneServer.Instance.Get<InputSystem>();
        }

        public override void OnStart()
        {
            base.OnStart();
            if (Camera.main != null)
                _cameraTransform = Camera.main.transform;

            if (World.GetWorld().GetSingleComponent(out PlayerCharacter player))
                _player = player;
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            _inputSystem.OnMovementCall += HandleMovement;
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            _inputSystem.OnMovementCall -= HandleMovement;
        }

        private void HandleMovement(Vector2 inputMovement)
        {
            var cameraForward = _cameraTransform.forward;
            var cameraRight = _cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            var moveDirection = Vector3.ClampMagnitude(cameraForward * inputMovement.y + cameraRight * inputMovement.x, 1f);
            Debug.Log($"Move call: {moveDirection}");
            _player.BasicMovement.Move(moveDirection.normalized);
            if (inputMovement.magnitude > .1f) _player.BasicMovement.Rotate(moveDirection.normalized);
        }
    }
}