using Dev.Character;
using Dev.Core;
using Dev.Input;
using Dev.Services;
using UnityEngine;

namespace Dev.Herd
{
    public enum HerdState
    {
        FOLLOW_AIM = 0,
        FOLLOW_PLAYER = 1
    }

    public class HerdBehaviour: MonoBehaviour, IAimable
    {
        [SerializeField] private SheepBehaviour[] _sheeps = new SheepBehaviour[0];

        [SerializeField] private float _moveRadius;
        [SerializeField] private float _minTimeToUpdatePosition;
        [SerializeField] private float _maxTimeToUpdatePosition;

        [SerializeField] private Transform _targetTransform;

        private HerdState _currentState = HerdState.FOLLOW_AIM;

        private PlayerCharacter _playerCharacter;

        private void Awake()
        {
            //Services.SingletoneServer.Instance.Set(this);
            World.GetWorld().AddSingleComponent(this);
        }

        private void Start()
        {
            InitializeSheeps();
            if (World.GetWorld().GetSingleComponent(out PlayerCharacter playerCharacter))
            {
                _playerCharacter = playerCharacter;
                _playerCharacter.OnMoveActions.OnMovementCall += OnPlayerMove;
            }
        }

        private void OnEnable()
        {
            if (_playerCharacter != null)
                _playerCharacter.OnMoveActions.OnMovementCall += OnPlayerMove;
        }

        private void OnDisable()
        {
            _playerCharacter.OnMoveActions.OnMovementCall -= OnPlayerMove;
        }

        private void InitializeSheeps()
        {
            float offsetDistance = _moveRadius / Mathf.Sin(Mathf.Deg2Rad * 60f);
            Vector3 offsetVector = transform.forward * offsetDistance;
            for (int i = 0; i < _sheeps.Length; i++)
            {
                float offsetAngle = i * 120f;
                Vector3 rotatedOffset = Quaternion.Euler(0f, offsetAngle, 0f) * offsetVector;
                SheepBehaviourInitInfo initInfo = new SheepBehaviourInitInfo()
                {
                    MoveRadius = _moveRadius,
                    MoveLocalOffset = rotatedOffset,
                    MinTimeToUpdatePosition = _minTimeToUpdatePosition,
                    MaxTimeToUpdatePosition = _maxTimeToUpdatePosition,
                    HerdBehaviour = this
                };

                _sheeps[i].Intialize(initInfo);
            }
        }

        public void SwitchState()
        {
            var newState = _currentState == HerdState.FOLLOW_AIM ? HerdState.FOLLOW_PLAYER : HerdState.FOLLOW_AIM;
            SetHerdState(newState);
        }

        public void SetHerdState(HerdState state)
        {
            if (state == _currentState) return;

            _currentState = state;

            if (_currentState == HerdState.FOLLOW_PLAYER)
            {
                UpdateTargetOnPlayerMove();
            }
        }

        public void SetPosition(Vector3 targetPosition)
        {
            _targetTransform.position = targetPosition;
            for (int i = 0; i < _sheeps.Length; i++)
            {
                _sheeps[i].RefreshTarget();
            }
        }

        public Vector3 GetInitAimingPoint()
        {
            return GetSheepsTargetPoint();
        }

        public Vector3 GetAimablePosition()
        {
            return (_targetTransform.position + Vector3.up);
        }

        public void OnAimingComplete(Vector3 aimingPoint)
        {
            SetHerdState(HerdState.FOLLOW_AIM);
            SetPosition(aimingPoint);
        }

        public Vector3 GetSheepsTargetPoint()
        {
            return _targetTransform.position;
        }

        public float GetAimingMulltiplier()
        {
            return 1f;
        }

        public Vector3[] GetTrajectory(Vector3 aimingPoint)
        {
            return Utilities.TrajectoryBuilder.CalculateStraightTrajectory(GetAimablePosition(), aimingPoint);
        }

        private void OnPlayerMove(Vector3 movement)
        {
            if (_currentState != HerdState.FOLLOW_PLAYER) return;

            UpdateTargetOnPlayerMove();
        }

        private void UpdateTargetOnPlayerMove()
        {
            var newTargetPosition = _targetTransform.position - _playerCharacter.transform.position;
            newTargetPosition = newTargetPosition.normalized * _moveRadius * 2f + _playerCharacter.transform.position;

            SetPosition(newTargetPosition);
        }
    }
}