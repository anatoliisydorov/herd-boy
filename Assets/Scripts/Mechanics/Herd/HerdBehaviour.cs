using Dev.Core;
using Dev.Input;
using UnityEngine;

namespace Dev.Herd
{
    public class HerdBehaviour: MonoBehaviour, IAimable
    {
        [SerializeField] private SheepBehaviour[] _sheeps = new SheepBehaviour[0];

        [SerializeField] private float _moveRadius;
        [SerializeField] private float _minTimeToUpdatePosition;
        [SerializeField] private float _maxTimeToUpdatePosition;

        [SerializeField] private Transform _targetTransform;

        private void Awake()
        {
            Services.SingletoneServer.Instance.Set(this);
        }

        private void Start()
        {
            InitializeSheeps();
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
            SetPosition(aimingPoint);
        }

        public Vector3 GetSheepsTargetPoint()
        {
            return _targetTransform.position;
        }
    }
}