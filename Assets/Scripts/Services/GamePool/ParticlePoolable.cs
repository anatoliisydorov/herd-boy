using Dev.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dev.ObjectsManagement
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlePoolable: SmartUpdate, IPoolable
    {
        [SerializeField] private bool _deactivateIfNotAlive = true;
        private ParticleSystem _particle;

        public AssetReference Reference { get; set; }
        public GameObject Obj { get => gameObject; }

        private void Awake()
        {
            _particle = GetComponent<ParticleSystem>();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_deactivateIfNotAlive && !_particle.IsAlive())
            {
                Deactivate();
            }
        }

        public void Activate(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            GamePool.ReturnPoolable(this);
        }
    }
}
