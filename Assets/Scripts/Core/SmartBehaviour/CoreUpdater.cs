using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Dev.Core
{
    public class CoreUpdater: MonoBehaviour
    {
        private Stopwatch _generalStopWatch = new Stopwatch();
        private float _awakeTimeBudget = 5f;
        private float _startTimeBudget = 5f;

        private List<ISmartAwakable> _awakableQueue = new List<ISmartAwakable>();
        private List<ISmartStartable> _startableQueue = new List<ISmartStartable>();
        

        private float _updateTimeBudget = 10f;
        private int _lastUpdatedIndex = 0;

        private List<ISmartUpdatable> _updatableForceQueue = new List<ISmartUpdatable>();
        private List<ISmartUpdatable> _updatableQueue = new List<ISmartUpdatable>();
        private List<ISmartFixedUpdatable> _fixedUpdatableQueue = new List<ISmartFixedUpdatable>();


        public bool AddAwake(ISmartAwakable awakable)
        {
            if (_awakableQueue.Contains(awakable)) return false;

            _awakableQueue.Add(awakable);
            return true;
        }

        public void RemoveAwake(ISmartAwakable awakable)
        {
            if (_awakableQueue.Contains(awakable))
                _awakableQueue.Remove(awakable);
        }
        
        public bool AddStart(ISmartStartable startable)
        {
            if (_startableQueue.Contains(startable)) return false;

            _startableQueue.Add(startable);
            return true;
        }

        public void RemoveStart(ISmartStartable startable)
        {
            if (_startableQueue.Contains(startable))
                _startableQueue.Remove(startable);
        }

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
            CallStarts();
            CallAwakes();
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

        private void CallStarts()
        {
            if (_awakableQueue.Count > 0) return;
            
            _generalStopWatch.Reset();

            for (int i = 0; i < _startableQueue.Count; i++)
            {
                if (_startableQueue[i].IsStarted)
                {
                    _startableQueue.Remove(_startableQueue[i]);
                    i--;
                }
                else
                {
                    _startableQueue[i].OnStart();
                    i--;
                }

                if (_generalStopWatch.ElapsedMilliseconds >= _startTimeBudget) break;
            }
        }

        private void CallAwakes()
        {
            _generalStopWatch.Reset();

            for (int i = 0; i < _awakableQueue.Count; i++)
            {
                if (_awakableQueue[i].IsAwaked)
                {
                    _awakableQueue.RemoveAt(i);
                    i--;
                }
                else
                {
                    _awakableQueue[i].OnAwake();
                    _awakableQueue[i].Enable();
                    RemoveAwake(_awakableQueue[i]);
                    i--;
                }

                if (_generalStopWatch.ElapsedMilliseconds >= _awakeTimeBudget) break;
            }
        }
    }
}