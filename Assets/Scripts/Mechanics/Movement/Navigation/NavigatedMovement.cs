using Dev.Core;
using Dev.Services;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Dev.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(NavMeshObstacle))]
    public class NavigatedMovement : BasicMovement, INavigatable
    {
        private NavMeshAgent _agent;
        private NavMeshObstacle _obstacle;

        private bool _isMoving;
        private Vector3 _currentTarget;
        private Coroutine _moveCoroutine;

        public float AgentRadius { get => _agent.radius; }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _obstacle = GetComponent<NavMeshObstacle>();
        }

        public void MoveTo(Vector3 targetPoint)
        {
            _isMoving = true;
            _currentTarget = targetPoint;
            SingletoneServer.Instance.Get<NavigationManager>().AddNavigatable(this);

            BuildPathAndMove();
        }

        public void Stop()
        {
            EndMoveCoroutine();

            _agent.enabled = false;
            _obstacle.enabled = true;
            IsBlocked = true;
            _isMoving = false;
        }

        private void EndMoveCoroutine()
        {
            if (_moveCoroutine != null)
                CoroutinesSystem.EndCouroutine(_moveCoroutine);
            _moveCoroutine = null;
        }

        private IEnumerator YIELD_MoveTo()
        {
            _obstacle.enabled = false;
            _agent.enabled = true;

            _agent.SetDestination(_currentTarget);
            while (_agent.pathPending)
                yield return null;

            if (_agent.pathStatus != NavMeshPathStatus.PathComplete)
                yield break;

            var path = _agent.path.corners;
            _agent.enabled = false;
            IsBlocked = false;
            IsRotateWithMovement = true;

            for (int i = 0; i < path.Length; i++)
            {
                var targetPoint = path[i];
                if (i != 0)
                    Debug.DrawLine(path[i - 1], targetPoint, Color.red, 5f);
                while (Vector3.Distance(targetPoint, transform.position) > .5f)
                {
                    var direction = targetPoint - transform.position;
                    var movement = new Vector2(direction.x, direction.z);
                    Move(movement);
                    yield return null;
                }
            }

            Move(Vector3.zero);

            _obstacle.enabled = true;
            IsBlocked = true;
            _isMoving = false;

            _moveCoroutine = null;
            SingletoneServer.Instance.Get<NavigationManager>().RemoveNavigatable(this);
            SingletoneServer.Instance.Get<NavigationManager>().RebuildPaths();
        }

        public void BuildPathAndMove()
        {
            EndMoveCoroutine();
            _moveCoroutine = CoroutinesSystem.BeginCoroutine(YIELD_MoveTo());
        }
    }
}
