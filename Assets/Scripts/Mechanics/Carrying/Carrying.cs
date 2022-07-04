using Dev.AimableMechanics;
using Dev.ObjectsManagement;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dev.Hands
{
    public class Carrying: MonoBehaviour
    {
        [SerializeField] private AssetReference _slingProjectile;

        public void GetCurrentThrowing(Action<ThrowingComponent> onGettingCompleted)
        {
            GamePool.GetPoolable(_slingProjectile, transform.position, transform.rotation, onGettingCompleted);
        }
    }
}