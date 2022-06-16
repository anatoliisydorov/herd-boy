using Dev.AimableMechanics;
using Dev.Core;
using UnityEngine;

namespace Dev.Hands
{
    public class Carrying: SmartStart
    {
        [SerializeField] private ThrowingComponent _slingProjectile;

        public ThrowingComponent GetCurrentThrowing()
        {
            var projectile = Instantiate(_slingProjectile, transform.position, transform.rotation);
            projectile.gameObject.SetActive(true);
            return projectile;
        }
    }
}