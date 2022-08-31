using System.Collections;
using Dev.Core;
using Dev.Hands;
using Dev.Services;
using UnityEngine;
using UnityEngine.AI;

namespace Dev.Herd
{
    public struct SheepBehaviourInitInfo
    {
        public float MoveRadius;
        public Vector3 MoveLocalOffset;

        public float MinTimeToUpdatePosition;
        public float MaxTimeToUpdatePosition;

        public HerdBehaviour HerdBehaviour;
    }

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CarryableItem))]
    public class SheepBehaviour: MonoBehaviour
    {
        private static float MAX_DISTANCE_TO_CONNECT = 5f;

        private float _moveRadius;
        private Vector3 _moveLocalOffset;

        private float _minTimeToUpdatePosition;
        private float _maxTimeToUpdatePosition;
        private Coroutine _delayedUpdatePositionCoroutine;

        private HerdBehaviour _herdBehaviour;
        private NavMeshAgent _agent;

        private CarryableItem _carryable;

        [SerializeField] private bool _isConnectedToHerd = true;

        public bool IsConnectedToHerd => _isConnectedToHerd;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _carryable = GetComponent<CarryableItem>();

            _carryable.OnPickedUp += OnPickedUp;
            _carryable.OnPuttedDown += OnPuttedDown;
        }

        public void Intialize(SheepBehaviourInitInfo initInfo)
        {
            _moveRadius = initInfo.MoveRadius - _agent.radius;
            _moveRadius = _moveRadius < 0f ? 0f : _moveRadius;
            _moveLocalOffset = initInfo.MoveLocalOffset;

            _minTimeToUpdatePosition = initInfo.MinTimeToUpdatePosition;
            _maxTimeToUpdatePosition = initInfo.MaxTimeToUpdatePosition;

            _herdBehaviour = initInfo.HerdBehaviour;

            RefreshTarget();
        }

        public void RefreshTarget()
        {
            Vector3 randomAditionalOffset = new Vector3(Random.Range(0f, _moveRadius), 0f, Random.Range(0f, _moveRadius));
            randomAditionalOffset.Normalize();

            Vector3 direction = _herdBehaviour.GetSheepsTargetPoint() + _moveLocalOffset + randomAditionalOffset;
            if (!IsConnectedToHerd) direction = transform.position + randomAditionalOffset;

            if (_agent.isActiveAndEnabled) _agent.isStopped = true;
            if (Physics.Raycast(direction + (Vector3.up * 5f), Vector3.down, out RaycastHit hit, 10f))
            {
                _agent.isStopped = false;
                _agent.SetDestination(hit.point);
            }

            DelayedUpdatePosition();
        }

        public void TryToConnect()
        {
            if (_carryable.IsCarryed) return;
            if (Vector3.Distance(_herdBehaviour.GetSheepsTargetPoint(), transform.position) <= MAX_DISTANCE_TO_CONNECT)
                _isConnectedToHerd = true;
        }

        private void OnPickedUp()
        {
            _isConnectedToHerd = false;
            if (_agent.isActiveAndEnabled) _agent.isStopped = true;
            if (_delayedUpdatePositionCoroutine != null)
                CoroutinesSystem.EndCouroutine(_delayedUpdatePositionCoroutine);

            _delayedUpdatePositionCoroutine = null;
        }

        private void OnPuttedDown()
        {
            TryToConnect();

            RefreshTarget();
        }

        private void DelayedUpdatePosition()
        {
            if (_delayedUpdatePositionCoroutine != null)
                CoroutinesSystem.EndCouroutine(_delayedUpdatePositionCoroutine);
            
            _delayedUpdatePositionCoroutine = CoroutinesSystem.BeginCoroutine(YIELD_DelayedUpdatePosition());
        }

        private IEnumerator YIELD_DelayedUpdatePosition()
        {
            float delayTime = Random.Range(_minTimeToUpdatePosition, _maxTimeToUpdatePosition);
            yield return new WaitForSeconds(delayTime);

            RefreshTarget();
        }
    }
}