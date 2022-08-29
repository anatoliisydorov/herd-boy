using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dev.ObjectsManagement
{
    public class MonoPoolable : MonoBehaviour, IPoolable
    {
        public AssetReference Reference { get; set; }
        public GameObject Obj { get => gameObject; }

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
