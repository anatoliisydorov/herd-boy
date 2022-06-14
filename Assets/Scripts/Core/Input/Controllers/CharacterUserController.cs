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
            Vector3 cameraForward = _cameraTransform.forward;
            Vector3 cameraRight = _cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = Vector3.ClampMagnitude(cameraForward * inputMovement.y + cameraRight * inputMovement.x, 1f);
            Debug.Log($"Move call: {moveDirection}");
            _player.BasicMovement.Move(moveDirection.normalized);
        }
    }
}