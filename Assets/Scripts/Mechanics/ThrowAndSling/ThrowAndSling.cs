using Dev.Character;
using Dev.Core;
using Dev.Hands;
using Dev.Input;
using Dev.Services;
using System;
using UnityEngine;

namespace Dev.AimableMechanics
{
    public interface IThrowable
    {
        float GetMass();
        void Throw(Vector3 targetPoint);
    }

    public class ThrowAndSling: MonoBehaviour, IAimable
    {
        private Carring _carrying;

        private const float MAX_THROWING_MASS = 1f;

        private void Start()
        {
            if (World.GetWorld().GetSingleComponent(out PlayerCharacter player))
            {
                _carrying = player.Carring;
            }
        }

        public Vector3 GetAimablePosition()
        {
            return transform.position;
        }

        public Vector3 GetInitAimingPoint()
        {
            return transform.position;
        }

        public void OnAimingComplete(Vector3 aimingPoint)
        {
            _carrying.ThrowCarryable(aimingPoint);
        }

        public float GetAimingMulltiplier()
        {
            return MAX_THROWING_MASS / _carrying.GetCarryableMass();
        }
    }
}