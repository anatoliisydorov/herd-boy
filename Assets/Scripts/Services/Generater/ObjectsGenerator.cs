using Dev.Core;
using Dev.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dev.ObjectsManagement
{
    public struct InstatiateRequest
    {
        public AssetReference Reference;
        public Action<GameObject> OnComplete;

        public InstatiateRequest(AssetReference reference, Action<GameObject> onComplete)
        {
            Reference = reference;
            OnComplete = onComplete;
        }
    }

    public class ObjectsGenerator : MonoBehaviour
    {
        public static void AddInstatiateRequest(AssetReference reference, Action<GameObject> onComplete)
        {
            SingletoneServer.Instance.Get<ObjectsGenerator>().AddInstatiateRequestLocal(reference, onComplete);
        }

        public static void DestroyInstance(GameObject obj)
        {
            SingletoneServer.Instance.Get<ObjectsGenerator>().DestroyInstanceLocal(obj);
        }

        private Queue<InstatiateRequest> _instatniateQueue = new Queue<InstatiateRequest>();
        private Queue<GameObject> _destroyQueue = new Queue<GameObject>();

        private int _instatiateTimeBudget = 10;
        private int _destroyTimeBudget = 10;
        private Stopwatch _generalStopWatch = new Stopwatch();

        private Coroutine _instantiateCoroutine;
        private Coroutine _destroyCoroutine;

        public void AddInstatiateRequestLocal(AssetReference reference, Action<GameObject> onComplete)
        {
            if (reference.RuntimeKey == null) return;

            _instatniateQueue.Enqueue(new InstatiateRequest(reference, onComplete));
            if (_instantiateCoroutine == null)
            {
                _instantiateCoroutine = CoroutinesSystem.BeginCoroutine(YIELD_GeneratorInstantiate());
            }
        }

        public void DestroyInstanceLocal(GameObject obj)
        {
            obj.SetActive(false);
            _destroyQueue.Enqueue(obj);
            if (_destroyCoroutine == null)
            {
                _destroyCoroutine = CoroutinesSystem.BeginCoroutine(YEILD_GeneratorDestroy());
            }
        }

        private IEnumerator YIELD_GeneratorInstantiate()
        {
            if (!_generalStopWatch.IsRunning) _generalStopWatch.Start();
            _generalStopWatch.Reset();

            while (_instatniateQueue.Count != 0)
            {
                var request = _instatniateQueue.Dequeue();
                //var handle = Addressables.InstantiateAsync(request.Reference);
                var handle = request.Reference.InstantiateAsync();

                while (!handle.IsDone) 
                {
                    yield return null;
                    _generalStopWatch.Reset();
                }

                request.OnComplete(handle.Result);

                if (_generalStopWatch.ElapsedMilliseconds >= _instatiateTimeBudget)
                {
                    yield return null;
                    _generalStopWatch.Reset();
                }
            }

            if (_destroyQueue.Count == 0)
            {
                _generalStopWatch.Stop();
            }
            _instantiateCoroutine = null;
        }

        private IEnumerator YEILD_GeneratorDestroy()
        {
            if (!_generalStopWatch.IsRunning) _generalStopWatch.Start();
            _generalStopWatch.Reset();

            while (_destroyQueue.Count != 0)
            {
                var obj = _destroyQueue.Dequeue();
                if (!Addressables.ReleaseInstance(obj))
                {
                    Destroy(obj);
                }
                if (_generalStopWatch.ElapsedMilliseconds >= _destroyTimeBudget)
                {
                    yield return null;
                    _generalStopWatch.Reset();
                }
            }

            if (_instatniateQueue.Count == 0)
            {
                _generalStopWatch.Stop();
            }
            _destroyCoroutine = null;
        }
    }
}