using System.Collections.Generic;
using Dev.Core;
using Dev.Time;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Jobs;

namespace Dev.Movement
{
    public struct MovementJob: IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Vector3> Movements;
        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            transform.position = transform.position + (Movements[index] * DeltaTime);
        }
    }

    public class BasicMovementHandler: SmartUpdate
    {
        private JobHandle _moveHandle;
        private TransformAccessArray _transforms;
        private NativeArray<Vector3> _movements;
        private List<IMovable> _movables = new List<IMovable>();

        public override void OnAwake()
        {
            base.OnAwake();
            forceUpdate = true;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (_movables.Count == 0) return;
            
            Transform[] transforms = new Transform[_movables.Count];
            _movements = new NativeArray<Vector3>(_movables.Count, Allocator.TempJob);
            for (int i = 0; i < _movables.Count; i++)
            {
                _movements[i] = _movables[i].MoveStep;
                
                transforms[i] = _movables[i].Transform;
            }
            
            _transforms = new TransformAccessArray(transforms);

            float deltaTime = GameTime.DeltaTime;
            var moveJob = new MovementJob()
            {
                Movements = _movements,
                DeltaTime = deltaTime
            };

            _moveHandle = moveJob.Schedule(_transforms);
            JobHandle.ScheduleBatchedJobs();
        }

        private void LateUpdate()
        {        
            if (!_transforms.isCreated) return;

            _moveHandle.Complete();

            _transforms.Dispose();
            _movements.Dispose();
        }

        public void AddBasicMovement(IMovable movable)
        {
            if (_movables.Contains(movable)) return;

            _movables.Add(movable);
        }

        public void RemoveBasicMovement(IMovable movable)
        {
            if (!_movables.Contains(movable)) return;

            _movables.Remove(movable);
        }
    }
}