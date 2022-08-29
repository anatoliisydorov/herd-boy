using Dev.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dev.ObjectsManagement
{
    public interface IPoolable
    {
        AssetReference Reference { get; set; }
        GameObject Obj { get; }

        void Activate(Vector3 position, Quaternion rotation);
        void Deactivate();
    }

    public sealed class GamePool
    {
        public static void GetPoolable<T>(AssetReference reference, Action<T> onGettedPoolable = null) where T : Component
        {
            SingletoneServer.Instance.Get<GamePool>().GetPoolableLocal(reference, onGettedPoolable);
        }
        public static void GetPoolable<T>(AssetReference reference, Vector3 position, Quaternion rotation, Action<T> onGettedPoolable = null) where T : Component
        {
            SingletoneServer.Instance.Get<GamePool>().GetPoolableLocal(reference, position, rotation, onGettedPoolable);
        }
        public static void ReturnPoolable<T>(T poolable) where T : IPoolable
        {
            SingletoneServer.Instance.Get<GamePool>().ReturnPoolableLocal(poolable);
        }
        public static void GeneratePoolablePacke(AssetReference reference, int packCount = 5, Action onGenerateCompleted = null)
        {
            SingletoneServer.Instance.Get<GamePool>().GeneratePoolablePackeLocal(reference, packCount, onGenerateCompleted);
        }
        public static void ClearPoolablePacke(AssetReference reference)
        {
            SingletoneServer.Instance.Get<GamePool>().ClearPoolablePackeLocal(reference);
        }
        public static void ClearAll()
        {
            SingletoneServer.Instance.Get<GamePool>().ClearAllLocal();
        }

        private Dictionary<object, Queue<IPoolable>> _freePoolables = new Dictionary<object, Queue<IPoolable>>();
        private List<IPoolable> _usedPoolables = new List<IPoolable>();

        private GameObject _poolablesParrent;

        public GamePool()
        {
            _poolablesParrent = new GameObject("GamePool");
            MonoBehaviour.DontDestroyOnLoad(_poolablesParrent);
        }

        public void GetPoolableLocal<T>(AssetReference reference, Action<T> onGettedPoolable) where T : Component
        {
            GetPoolableLocal(reference, Vector3.zero, Quaternion.identity, onGettedPoolable);
        }

        public void GetPoolableLocal<T>(AssetReference reference, Vector3 position, Quaternion rotation, Action<T> onGettedPoolable) where T : Component
        {
            if (!_freePoolables.ContainsKey(reference.RuntimeKey) || _freePoolables[reference.RuntimeKey].Count == 0)
            {
                GeneratePoolablePackeLocal(reference, 5, () => 
                {
                    var poolable = GetPoolableLocal(reference);
                    poolable.Activate(position, rotation);
                    poolable.Obj.transform.position = position;
                    poolable.Obj.transform.rotation = rotation;
                    onGettedPoolable?.Invoke(poolable.Obj.GetComponent<T>());
                });

                return;
            }

            var poolable = GetPoolableLocal(reference);
            poolable.Activate(position, rotation);
            poolable.Obj.transform.position = position;
            poolable.Obj.transform.rotation = rotation;
            onGettedPoolable?.Invoke(poolable.Obj.GetComponent<T>());
        }

        public void ReturnPoolableLocal<T>(T poolable) where T : IPoolable
        {
            _usedPoolables.Remove(poolable);
            _freePoolables[poolable.Reference.RuntimeKey].Enqueue(poolable);
        }

        public void GeneratePoolablePackeLocal(AssetReference reference, int packCount = 5, Action onGenerateCompleted = null)
        {
            int generatedCount = 0;
            for (int i = 0; i < packCount; i++)
            {
                ObjectsGenerator.AddInstatiateRequest(reference, (result) =>
                {
                    if (!_freePoolables.ContainsKey(reference.RuntimeKey))
                    {
                        _freePoolables.Add(reference.RuntimeKey, new Queue<IPoolable>(packCount));
                    }

                    if (!result.TryGetComponent(out IPoolable poolable))
                    {
                        poolable = result.AddComponent<MonoPoolable>();
                    }

                    poolable.Obj.SetActive(false);
                    poolable.Reference = reference;
                    poolable.Obj.transform.SetParent(_poolablesParrent.transform);
                    _freePoolables[reference.RuntimeKey].Enqueue(poolable);

                    generatedCount++;
                    if (generatedCount == packCount)
                    {
                        onGenerateCompleted?.Invoke();
                    }
                });
            }
        }

        public void ClearPoolablePackeLocal(object key)
        {
            var queue = _freePoolables[key];
            while (queue.Count > 0)
            {
                ObjectsGenerator.DestroyInstance(queue.Dequeue().Obj);
            }

            Addressables.Release(key);
        }

        public void ClearAllLocal()
        {
            for (int i = 0; i < _usedPoolables.Count; i++)
            {
                ObjectsGenerator.DestroyInstance(_usedPoolables[i].Obj);
            }

            foreach (var poolablesPack in _freePoolables)
            {
                ClearPoolablePackeLocal(poolablesPack.Key);
            }
        }

        private IPoolable GetPoolableLocal(AssetReference reference)
        {
            var poolable = _freePoolables[reference.RuntimeKey].Dequeue();
            _usedPoolables.Add(poolable);
            return poolable;
        }
    }
}