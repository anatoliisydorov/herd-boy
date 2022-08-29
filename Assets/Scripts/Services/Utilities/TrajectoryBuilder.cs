using System.Collections.Generic;
using UnityEngine;

namespace Dev.Utilities
{
    public class TrajectoryBuilder
    {
        public static Vector3[] CalculateStraightTrajectory(Vector3 startPoint, Vector3 endPoint)
        {
            var points = new Vector3[]
            {
                startPoint, endPoint
            };
            return points;
        }

        public static Vector3[] CalculateByGravityTrajectory(Vector3 startPoint, Vector3 velocity, Vector3 targetPoint, int maxPointsCount = 10, bool checkCollision = true)
        {
            var gravityY = Physics.gravity.y;
            var duration = (2 * velocity.y) / gravityY;
            var height = startPoint.y - targetPoint.y;
            if (height > 0)
            {
                var calc = velocity.y * velocity.y + 2 * gravityY * -height;
                duration = (velocity.y + Mathf.Sqrt(calc)) / gravityY;
            }
            var stepTime = duration / maxPointsCount;

            var resultPoints = new Vector3[maxPointsCount + 1];
            resultPoints[0] = startPoint;

            for (int i = 1; i < resultPoints.Length; i++)
            {
                var stepTimePassed = stepTime * i;
                var movementPoint = new Vector3(
                    velocity.x * stepTimePassed,
                    velocity.y * stepTimePassed - .5f * gravityY * stepTimePassed * stepTimePassed,
                    velocity.z * stepTimePassed);

                var newTrajectoryPoint = -movementPoint + startPoint;
                resultPoints[i] = newTrajectoryPoint;

                var previousPoint = resultPoints[i - 1];
                var directionVector = newTrajectoryPoint - previousPoint;

                if (checkCollision && Physics.Raycast(previousPoint, directionVector, out var hit, directionVector.magnitude))
                {
                    resultPoints[i] = hit.point;

                    var tempResultPoints = new Vector3[i + 1];
                    for (int j = 0; j < tempResultPoints.Length; j++)
                    {
                        tempResultPoints[j] = resultPoints[j];
                    }

                    resultPoints = tempResultPoints;
                    break;
                }
            }

            return resultPoints;
        }
    }
}
