using Dev.AimableMechanics;
using Dev.Herd;
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
    }

    [RequireComponent(typeof(LineRenderer),
        typeof(Sling))]
    public class AimingController: BaseController
    {
        [SerializeField] private float _aimingSpeed = 10f;

        private AimingState _state = AimingState.NONE;
        private Vector2 _lastAimScreenPosition;
        private Vector3 _lastAimPoint;

        private InputSystem _inputSystem;
        private VirtualCursor _virtualCursor;
        private Camera _mainCamera;

        private IAimable _currentAimable;
        private HerdBehaviour _herd;
        private Sling _sling;

        private LineRenderer _line;

        private InputSystem InputSystem
        {
            get
            {
                if (_inputSystem == null) _inputSystem = SingletoneServer.Instance.Get<InputSystem>();
                return _inputSystem;
            }
        }

        public override void OnAwake()
        {
            base.OnAwake();

            _sling = GetComponent<Sling>();
            _line = GetComponent<LineRenderer>();

            SetActiveTrajectory(false);
        }

        public override void OnStart()
        {
            base.OnStart();

            _inputSystem = SingletoneServer.Instance.Get<InputSystem>();
            _virtualCursor = SingletoneServer.Instance.Get<VirtualCursor>();
            _herd = SingletoneServer.Instance.Get<HerdBehaviour>();

            _mainCamera = Camera.main;
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            InputSystem.OnHerdTargetCall += HandleHerdTarget;
            InputSystem.OnSlingOrThrowCall += HandleSlingOrThrow;
            InputSystem.OnAimingCall += HandleAiming;
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            
            InputSystem.OnHerdTargetCall -= HandleHerdTarget;
            InputSystem.OnSlingOrThrowCall -= HandleSlingOrThrow;
            InputSystem.OnAimingCall -= HandleAiming;
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
            }
        }

        private void HandleSlingOrThrow(bool startPress)
        {
            if (_state == AimingState.NONE && startPress)
            {
                _state = AimingState.SLING_OR_THROW;
                _currentAimable = _sling;
                StartAiming();
            }
            else if (_state == AimingState.SLING_OR_THROW && !startPress)
            {
                CompleteAiming();
            }
        }

        private void HandleAiming(Vector2 aimingChange)
        {
            if (_state == AimingState.NONE) return;
            
            if (_inputSystem.CurrentDevice == DeviceType.Gamepad)
            {
                Vector2 newAimScreenPosition = _lastAimScreenPosition + (aimingChange.normalized * _aimingSpeed * Dev.Time.GameTime.DeltaTime);
                _virtualCursor.SetCursorPosition(newAimScreenPosition);
            }
            
            SetAimingPosition(_virtualCursor.GetCursorPosition());
        }

        private void StartAiming()
        {
            _lastAimScreenPosition = new Vector2(-1f, -1f);

            Vector2 startAimingScreenPoint = _mainCamera.WorldToScreenPoint(_currentAimable.GetInitAimingPoint());
            _virtualCursor.SetCursorPosition(startAimingScreenPoint);

            SetActiveTrajectory(true);
            if (CheckRaycast(out Vector3 resultPosition, startAimingScreenPoint))
            {
                _lastAimPoint = resultPosition;
                RenderTrajectory();
            }
        }

        private void SetAimingPosition(Vector2 aimingScreenPosition)
        {
            if (Vector2.Distance(_lastAimScreenPosition, aimingScreenPosition) < .1f) return;
            _lastAimScreenPosition = aimingScreenPosition;

            if (CheckRaycast(out Vector3 resultPosition, aimingScreenPosition))
            {
                _lastAimPoint = resultPosition;
                RenderTrajectory();
            }
        }

        private bool CheckRaycast(out Vector3 result, Vector2 aimingScreenPosition)
        {
            result = Vector3.zero;
            Ray ray = _mainCamera.ScreenPointToRay(aimingScreenPosition);
            Debug.Log($"Fuuuuuuck: {aimingScreenPosition}");
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                result = hit.point;
                
                if (_state == AimingState.HERD_TARGET)
                {
                    Vector3 startNavCheckPosition = hit.point + Vector3.up;
                    Vector3 endNavCheckPosition = hit.point - Vector3.up;

                    if (!NavMesh.Raycast(startNavCheckPosition, endNavCheckPosition, out NavMeshHit navHit, NavMesh.AllAreas))
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

        private void SetActiveTrajectory(bool active)
        {
            if (_line.enabled == active) return;
            _line.enabled = active;
        }

        private void RenderTrajectory()
        {
            Vector3[] points = new Vector3[]
            {
                _currentAimable.GetAimablePosition(), _lastAimPoint
            };

            _line.SetPositions(points);
            
            float width =  _line.startWidth;
            _line.material.mainTextureScale = new Vector2(1f / width, 1.0f);
        }

        private void CompleteAiming()
        {
            SetActiveTrajectory(false);
            _state = AimingState.NONE;
            _currentAimable.OnAimingComplete(_lastAimPoint);
        }
    }
}