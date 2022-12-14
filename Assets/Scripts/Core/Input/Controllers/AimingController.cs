using Dev.Actions;
using Dev.AimableMechanics;
using Dev.Character;
using Dev.Herd;
using Dev.Movement;
using Dev.Services;
using UnityEngine;
using UnityEngine.AI;

namespace Dev.Input
{
    public enum AimingState
    {
        NONE = 0,
        HERD_TARGET = 1,
        SLING_OR_THROW = 2
    }

    public interface IAimable
    {
        Vector3 GetInitAimingPoint();
        Vector3 GetAimablePosition();
        void OnAimingComplete(Vector3 aimingPoint);
        float GetAimingMulltiplier();
        Vector3[] GetTrajectory(Vector3 aimingPoint);
    }

    [RequireComponent(typeof(LineRenderer))]
    public class AimingController: BaseController
    {
        [SerializeField] private float _aimingSpeed = 10f;

        private AimingState _state = AimingState.NONE;
        private Vector2 _gamepadAiming;
        private Vector2 _lastAimScreenPosition;
        private Vector3 _lastAimPoint;

        private InputSystem _inputSystem;
        private Camera _mainCamera;

        private IAimable _currentAimable;
        private HerdBehaviour _herd;
        private ThrowAndSling _throw;
        private MovementActions _playerOnMoveActions;
        private IMovable _playerMovable;

        private LineRenderer _line;

        private InputSystem InputSystem
        {
            get
            {
                if (_inputSystem == null) _inputSystem = SingletoneServer.Instance.Get<InputSystem>();
                return _inputSystem;
            }
        }

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();

            SetActiveTrajectory(false);
        }

        private void Start()
        {
            _inputSystem = SingletoneServer.Instance.Get<InputSystem>();
            _herd = SingletoneServer.Instance.Get<HerdBehaviour>();

            if (World.GetWorld().GetSingleComponent(out PlayerCharacter player))
            {
                _throw = player.Throw;
                _playerOnMoveActions = player.OnMoveActions;
                _playerMovable = player.BasicMovement;

                _playerOnMoveActions.OnMovementCall += UpdateTrajectoryOnMovement;
                _playerOnMoveActions.OnRotateCall += UpdateTrajectoryOnRotate;
            }

            _mainCamera = Camera.main;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            UpdateGamepadAiming();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_playerOnMoveActions != null)
            {
                _playerOnMoveActions.OnMovementCall += UpdateTrajectoryOnMovement;
                _playerOnMoveActions.OnRotateCall += UpdateTrajectoryOnRotate;
            }

            InputSystem.OnHerdTargetCall += HandleHerdTarget;
            InputSystem.OnSlingOrThrowCall += HandleSlingOrThrow;
            InputSystem.OnMouseAimingCall += HandleMouseAiming;
            InputSystem.OnGamepadAimingCall += HandleGamepadAiming;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _playerOnMoveActions.OnMovementCall -= UpdateTrajectoryOnMovement;
            _playerOnMoveActions.OnRotateCall -= UpdateTrajectoryOnRotate;

            InputSystem.OnHerdTargetCall -= HandleHerdTarget;
            InputSystem.OnSlingOrThrowCall -= HandleSlingOrThrow;
            InputSystem.OnMouseAimingCall -= HandleMouseAiming;
            InputSystem.OnGamepadAimingCall -= HandleGamepadAiming;
        }

        private void UpdateTrajectoryOnMovement(Vector3 movement)
        {
            if (_state == AimingState.NONE) return;

            CheckRaycastAndRenderTrajectory(_lastAimScreenPosition);
        }

        private void UpdateTrajectoryOnRotate(Quaternion newRotation)
        {
            if (_state == AimingState.NONE) return;

            CheckRaycastAndRenderTrajectory(_lastAimScreenPosition);
        }

        private void HandleHerdTarget(bool startPress)
        {
            if (_state == AimingState.NONE && startPress)
            {
                _state = AimingState.HERD_TARGET;
                _currentAimable = _herd;
                StartAiming();
            }
            else if (_state == AimingState.HERD_TARGET && !startPress)
            {
                CompleteAiming();
                _gamepadAiming = Vector2.zero;
            }
        }

        private void HandleSlingOrThrow(bool startPress)
        {
            if (_state == AimingState.NONE && startPress)
            {
                _state = AimingState.SLING_OR_THROW;
                _currentAimable = _throw;
                StartAiming();
            }
            else if (_state == AimingState.SLING_OR_THROW && !startPress)
            {
                CompleteAiming();
                _gamepadAiming = Vector2.zero;
            }
        }

        private void HandleGamepadAiming(Vector2 aimingChange)
        {
            if (_state == AimingState.NONE) return;

            _gamepadAiming = aimingChange * _aimingSpeed;
        }

        private void HandleMouseAiming(Vector2 aimingChange)
        {
            if (_state == AimingState.NONE) return;

            SetAimingPosition(_inputSystem.VirtualCursor.GetCursorPosition());
        }

        private void UpdateGamepadAiming()
        {
            if (_state == AimingState.NONE || _inputSystem.CurrentDevice == DeviceType.Keyboard) return;

            Vector2 newAimScreenPosition = _lastAimScreenPosition + (_gamepadAiming * Dev.Time.GameTime.DeltaTime);
            newAimScreenPosition = Extensions.ScreenExtansions.ClampToScreenSize(newAimScreenPosition);

            SetAimingPosition(newAimScreenPosition);
        }

        private void StartAiming()
        {
            _playerMovable.IsRotateWithMovement = false;

            _lastAimScreenPosition = _mainCamera.WorldToScreenPoint(_currentAimable.GetInitAimingPoint());
            _inputSystem.VirtualCursor.SetCursorPosition(_lastAimScreenPosition);

            SetActiveTrajectory(true);
            CheckRaycastAndRenderTrajectory(_lastAimScreenPosition);
        }

        private void SetAimingPosition(Vector2 aimingScreenPosition)
        {
            aimingScreenPosition = Extensions.ScreenExtansions.ClampByDistanceMultiplier(_mainCamera.WorldToScreenPoint(_currentAimable.GetAimablePosition()), aimingScreenPosition, _currentAimable.GetAimingMulltiplier());

            if (Vector2.Distance(_lastAimScreenPosition, aimingScreenPosition) < .1f) return;
            _lastAimScreenPosition = aimingScreenPosition;

            CheckRaycastAndRenderTrajectory(aimingScreenPosition);
        }

        private void CheckRaycastAndRenderTrajectory(Vector2 screenPosition)
        {
            if (CheckRaycast(out Vector3 resultPosition, screenPosition))
            {
                RotatePlayer(resultPosition);

                _lastAimPoint = resultPosition;
                RenderTrajectory();
            }
        }

        private bool CheckRaycast(out Vector3 result, Vector2 aimingScreenPosition)
        {
            result = Vector3.zero;
            Ray ray = _mainCamera.ScreenPointToRay(aimingScreenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                result = hit.point;
                Debug.Log(hit.collider.name);
                
                if (_state == AimingState.HERD_TARGET)
                {
                    if (NavMesh.SamplePosition(hit.point, out var navHit, 1f, NavMesh.AllAreas))
                    {
                        result = navHit.position;
                        return true;
                    }
                    return false;

                }
                return true;
            }

            return false;
        }

        private void RotatePlayer(Vector3 aimedPoint)
        {
            var direction = aimedPoint - _playerMovable.Transform.position;
            direction.y = 0f;

            _playerMovable.Rotate(direction);
        }

        private void SetActiveTrajectory(bool active)
        {
            if (_line.enabled == active) return;
            _line.enabled = active;
        }

        private void RenderTrajectory()
        {
            var points = _currentAimable.GetTrajectory(_lastAimPoint);
            _line.positionCount = points.Length;
            _line.SetPositions(points);
            
            float width =  _line.startWidth;
            _line.material.mainTextureScale = new Vector2(1f / width, 1.0f);
        }

        private void CompleteAiming()
        {
            _playerMovable.IsRotateWithMovement = true;

            SetActiveTrajectory(false);
            _state = AimingState.NONE;
            _currentAimable.OnAimingComplete(_lastAimPoint);
        }
    }
}