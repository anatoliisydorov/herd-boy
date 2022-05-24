// using System.Collections;
// using System.Collections.Generic;
// using Unity.Collections;
// using Unity.Mathematics;
// using UnityEngine;

// namespace Dev.Movement
// {
//     public interface INavMeshMovable
//     {
//         bool IsBlockNavMesh { get; set; }
//         bool IsMoving { get; }
//         Vector3 startPosition { get; }
//         Vector3 endPosition { get; }
//         void MoveToPoint(Vector3 toPoint, bool takePriority);
//         void CompletePath(NativeArray<float3> result);
//         void StopMove();
//     }

//     public class NavMeshMovement : BasicMovement, INavMeshMovable
//     {
//         private bool _isInPriority;
//         private List<Vector3> _points = new List<Vector3>();

//         public bool IsBlockNavMesh { get; set; }
//         public bool IsMoving { get; private set; }
//         public Vector3 startPosition { get; private set; }
//         public Vector3 endPosition { get; private set; }

//         public void MoveToPoint(Vector3 toPoint, bool takePriority = false)
//         {
            
//         }
        
//         public void CompletePath(NativeArray<float3> result)
//         {
//             for (int i = 0; i < result.Length; i++)
//             {
//                 _points.Add(result[i]);
//             }
//         }

//         public void StopMove()
//         {
            
//         }

//         private IEnumerator YIELD_Moving()
//         {
//             while (_points.Count > 0)
//             {
//                 Vector3 nearestPoint = _points[0];
//                 _points.RemoveAt(0);

//                 while (Vector3.Distance(Transform.position, nearestPoint) > .5f)
//                 {
//                     Move((nearestPoint - Transform.position).normalized);
//                     yield return null;
//                 }
//             }
//         }
//     }
// }