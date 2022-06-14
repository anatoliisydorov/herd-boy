using Dev.Core;
using Dev.Input;
using UnityEngine;

namespace Dev.AimableMechanics
{
    public class Sling: SmartStart, IAimable
    {

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
            Debug.Log($"Shoot with sling or throw: {aimingPoint}");
        }
    }
}