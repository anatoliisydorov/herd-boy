using Dev.Actions;
using Dev.AimableMechanics;
using Dev.Character;
using Dev.ObjectsManagement;
using Dev.Services;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dev.Hands
{
    public class Carring: MonoBehaviour
    {
        [SerializeField] private AssetReference _slingProjectile;
        [SerializeField] private TriggerActions _triggerActions;
        [SerializeField] private Transform _holdingTransform;
        [SerializeField] private BaseCharacter _character;

        private ICarryable _currentTargetCarryable;
        private ICarryable _holdingCarryable;
        private ICarryable[] _carryablesInsideTrigger = new ICarryable[5];

        public bool IsCarring => _holdingCarryable != null;

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
            if (_holdingCarryable != null)
            {
                onGettingCompleted.Invoke(_holdingCarryable.GetThrowable());
                return;
            }

            Action<ThrowingStraightComponent> onComplete = (result) => onGettingCompleted.Invoke(result);
            GamePool.GetPoolable(_slingProjectile, transform.position, transform.rotation, onComplete);
        }

        public void CleanHoldingCarryable()
        {
            PutDownCarryable();
        }

        public void SwitchPickAndPut()
        {
            if (_holdingCarryable == null)
                PickUpCarryable();
            else
                PutDownCarryable();
        }

        private void PickUpCarryable()
        {
            if (_currentTargetCarryable == null) return;

            _holdingCarryable = _currentTargetCarryable;
            _currentTargetCarryable = null;

            _holdingCarryable.PickUp();
            _holdingCarryable.GetTransform().SetParent(_holdingTransform);
            _holdingCarryable.GetTransform().position = _holdingTransform.position;
            _holdingCarryable.GetTransform().rotation = _holdingTransform.rotation;
        }

        private void PutDownCarryable()
        {
            if (_holdingCarryable == null) return;

            var collider = _holdingCarryable.GetCollider();
            _holdingCarryable.PutDown();
            _holdingCarryable.GetTransform().SetParent(null);
            _holdingCarryable = null;
            _currentTargetCarryable = null;

            TryRefreshCurrentCarryable();
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

            if (carryable == _currentTargetCarryable)
                _currentTargetCarryable = null;

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
                    float distance = Vector3.Distance(transform.position, _carryablesInsideTrigger[i].GetTransform().position);
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