using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Dev.Core
{
    public class CoreUpdater: MonoBehaviour
    {
        private Stopwatch _generalStopWatch = new Stopwatch();        

        private float _updateTimeBudget = 10f;
        private int _lastUpdatedIndex = 0;

        private List<ISmartUpdatable> _updatableForceQueue = new List<ISmartUpdatable>();
        private List<ISmartUpdatable> _updatableQueue = new List<ISmartUpdatable>();
        private List<ISmartLateUpdatable> _lateUpdatableQueue = new List<ISmartLateUpdatable>();
        private List<ISmartFixedUpdatable> _fixedUpdatableQueue = new List<ISmartFixedUpdatable>();

        public bool AddForceUpdate(ISmartUpdatable updatable)
        {
            if (_updatableForceQueue.Contains(updatable)) return false;

            _updatableForceQueue.Add(updatable);
            return true;
        }

        public void RemoveForceUpdate(ISmartUpdatable updatable)
        {
            if (_updatableForceQueue.Contains(updatable))
                _updatableForceQueue.Remove(updatable);
        }
        
        public bool AddUpdate(ISmartUpdatable updatable)
        {
            if (_updatableQueue.Contains(updatable)) return false;

            _updatableQueue.Add(updatable);
            return true;
        }

        public void RemoveUpdate(ISmartUpdatable updatable)
        {
            if (_updatableQueue.Contains(updatable))
                _updatableQueue.Remove(updatable);
        }

        public bool AddFixedUpdate(ISmartFixedUpdatable fixedUpdatable)
        {
            if (_fixedUpdatableQueue.Contains(fixedUpdatable)) return false;

            _fixedUpdatableQueue.Add(fixedUpdatable);
            return true;
        }

        public void RemoveFixedUpdate(ISmartFixedUpdatable fixedUpdatable)
        {
            if (_fixedUpdatableQueue.Contains(fixedUpdatable))
                _fixedUpdatableQueue.Remove(fixedUpdatable);
        }


        public bool AddLateUpdate(ISmartLateUpdatable updatable)
        {
            if (_lateUpdatableQueue.Contains(updatable)) return false;

            _lateUpdatableQueue.Add(updatable);
            return true;
        }

        public void RemoveLateUpdate(ISmartLateUpdatable updatable)
        {
            if (_lateUpdatableQueue.Contains(updatable))
                _lateUpdatableQueue.Remove(updatable);
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _fixedUpdatableQueue.Count; i++)
            {
                _fixedUpdatableQueue[i].OnFixedUpdate();
            }
        }
        private void Update()
        {
            if (!_generalStopWatch.IsRunning) _generalStopWatch.Start();

            CallForceUpdates();
            CallUpdates();
        }

        private void LateUpdate()
        {
            CallLateUpdates();
        }

        private void CallForceUpdates()
        {
            for (int i = 0; i < _updatableForceQueue.Count; i++)
            {
                _updatableForceQueue[i].OnUpdate();
            }
        }

        private void CallUpdates()
        {
            if (_updatableQueue.Count == 0) return;
            _generalStopWatch.Reset();
            
            int maxUpdatesCount = 0;
            while (_generalStopWatch.ElapsedMilliseconds < _updateTimeBudget)
            {
                _updatableQueue[_lastUpdatedIndex].OnUpdate();
                _lastUpdatedIndex++;
                maxUpdatesCount++;

                if (_lastUpdatedIndex >= _updatableQueue.Count) _lastUpdatedIndex = 0;

                if (maxUpdatesCount >= _updatableQueue.Count) break;
            }
        }

        private void CallLateUpdates()
        {
            if (_lateUpdatableQueue.Count == 0) return;
            _generalStopWatch.Reset();

            int maxUpdatesCount = 0;
            while (_generalStopWatch.ElapsedMilliseconds < _updateTimeBudget)
            {
                _lateUpdatableQueue[_lastUpdatedIndex].OnLateUpdate();
                _lastUpdatedIndex++;
                maxUpdatesCount++;

                if (_lastUpdatedIndex >= _lateUpdatableQueue.Count) _lastUpdatedIndex = 0;

                if (maxUpdatesCount >= _lateUpdatableQueue.Count) break;
            }
        }
    }
}