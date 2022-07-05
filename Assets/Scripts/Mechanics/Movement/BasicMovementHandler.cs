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
    public struct MovementJobData
    {
        public Vector3 Movement;
        public Quaternion TargetRotation;
        public float MoveSpeed;
        public float RotateSpeed;
    }

    public struct MovementJob: IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<MovementJobData> Data;
        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            var data = Data[index];
            var newPosition = transform.position + data.Movement;
            transform.position = Vector3.Lerp(transform.position, newPosition, data.MoveSpeed * DeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, data.TargetRotation, data.RotateSpeed * DeltaTime);
        }
    }

    public class BasicMovementHandler: SmartUpdate
    {
        private JobHandle _moveHandle;
        private TransformAccessArray _transforms;
        private NativeArray<MovementJobData> _movementData;
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
            _movementData = new NativeArray<MovementJobData>(_movables.Count, Allocator.TempJob);
            for (int i = 0; i < _movables.Count; i++)
            {
                _movementData[i] = _movables[i].MoveData;
                
                transforms[i] = _movables[i].Transform;
            }
            
            _transforms = new TransformAccessArray(transforms);

            float deltaTime = GameTime.DeltaTime;
            var moveJob = new MovementJob()
            {
                Data = _movementData,
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
            _movementData.Dispose();
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