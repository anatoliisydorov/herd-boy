using Dev.Character;
using Dev.Herd;
using Dev.Services;
using UnityEngine;

namespace Dev.Input
{
    public class CharacterUserController: BaseController
    {
        private InputSystem _inputSystem;

        private Transform _cameraTransform;
        private PlayerCharacter _player;
        private HerdBehaviour _herd;

        private void Awake()
        {
            _inputSystem = SingletoneServer.Instance.Get<InputSystem>();
        }

        private void Start()
        {
            if (Camera.main != null)
                _cameraTransform = Camera.main.transform;

            if (World.GetWorld().GetSingleComponent(out PlayerCharacter player))
                _player = player;
            if (World.GetWorld().GetSingleComponent(out HerdBehaviour herd))
                _herd = herd;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _inputSystem.OnMovementCall += HandleMovement;
            _inputSystem.OnInteractCall += HandleInteract;

            _inputSystem.OnHerdStateCall += HandleherdStateChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _inputSystem.OnMovementCall -= HandleMovement;
            _inputSystem.OnInteractCall -= HandleInteract;

            _inputSystem.OnHerdStateCall -= HandleherdStateChange;
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
            _player.BasicMovement.Move(moveDirection);
        }

        private void HandleInteract()
        {
            _player.Carring.SwitchPickAndPut();
        }

        private void HandleherdStateChange()
        {
            _herd.SwitchState();
        }
    }
}