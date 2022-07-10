using Dev.Actions;
using Dev.AimableMechanics;
using Dev.ObjectsManagement;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dev.Hands
{
    public class Carring: MonoBehaviour
    {
        [SerializeField] private AssetReference _slingProjectile;
        [SerializeField] private TriggerActions _triggerActions;

        private ICarryable _currentTargetCarryable;
        private ICarryable[] _carryablesInsideTrigger = new ICarryable[5];

        private void Awake()
        {
            _triggerActions.OnTriggerEnterCall += OnTriggerEnterAction;
            _triggerActions.OnTriggerExitCall += OnTriggerExitAction;
        }

        private void OnDestroy()
        {
            _triggerActions.OnTriggerEnterCall -= OnTriggerEnterAction;
            _triggerActions.OnTriggerExitCall -= OnTriggerExitAction;
        }

        public void GetCurrentThrowing(Action<IThrowable> onGettingCompleted)
        {
            if (_currentTargetCarryable != null)
            {
                onGettingCompleted.Invoke(_currentTargetCarryable.GetThrowable());
                return;
            }

            Action<ThrowingComponent> onComplete = (result) => onGettingCompleted.Invoke(result);
            GamePool.GetPoolable(_slingProjectile, transform.position, transform.rotation, onComplete);
        }

        private void OnTriggerEnterAction(Collider collider)
        {
            if (!collider.TryGetComponent(out ICarryable carryable)) return;
            if (ContainCarryable(carryable)) return;

            if (TryInsertCarryable(carryable))
                TryRefreshCurrentCarryable();
        }

        private void OnTriggerExitAction(Collider collider)
        {
            if (!collider.TryGetComponent(out ICarryable carryable)) return;
            if (!ContainCarryable(carryable)) return;

            if (TryRemoveCarryable(carryable))
                TryRefreshCurrentCarryable();
        }

        private bool ContainCarryable(ICarryable carryable)
        {
            for (int i = 0; i < _carryablesInsideTrigger.Length; i++)
                if (carryable == _carryablesInsideTrigger[i]) return true;
            return false;
        }

        private bool TryInsertCarryable(ICarryable carryable)
        {
            for (int i = 0; i < _carryablesInsideTrigger.Length; i++)
            {
                if (_carryablesInsideTrigger[i] == null)
                {
                    _carryablesInsideTrigger[i] = carryable;
                    return true;
                }
            }
            return false;
        }

        private bool TryRemoveCarryable(ICarryable carryable)
        {
            for (int i = 0; i < _carryablesInsideTrigger.Length; i++)
            {
                if (_carryablesInsideTrigger[i] == carryable)
                {
                    _carryablesInsideTrigger[i] = null;
                    return true;
                }
            }
            return false;
        }

        private bool TryRefreshCurrentCarryable()
        {
            if (_currentTargetCarryable != null) return false;

            float clossestCarryableDistance = float.MaxValue;
            for (int i = 0; i < _carryablesInsideTrigger.Length; i++)
            {
                if (_carryablesInsideTrigger[i] != null)
                {
                    float distance = Vector3.Distance(transform.position, _carryablesInsideTrigger[i].GetPosition());
                    if (distance < clossestCarryableDistance)
                    {
                        clossestCarryableDistance = distance;
                        _currentTargetCarryable = _carryablesInsideTrigger[i];
                    }
                }
            }

            if (_currentTargetCarryable != null)
                return true;

            return false;
        }
    }
}