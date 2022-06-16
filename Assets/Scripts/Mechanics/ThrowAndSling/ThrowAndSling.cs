using Dev.Character;
using Dev.Core;
using Dev.Hands;
using Dev.Input;
using Dev.Services;
using UnityEngine;

namespace Dev.AimableMechanics
{
    public interface IThrowable
    {
        void Throw(Vector3 direction);
    }

    public class ThrowAndSling: SmartStart, IAimable
    {
        private Carrying _carrying;

        public override void OnStart()
        {
            base.OnStart();

            if (World.GetWorld().GetSingleComponent(out PlayerCharacter player))
            {
                _carrying = player.Carrying;
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
            var throwing = _carrying.GetCurrentThrowing();
            var direction = (aimingPoint - transform.position).normalized;
            throwing.Throw(direction);
        }
    }
}