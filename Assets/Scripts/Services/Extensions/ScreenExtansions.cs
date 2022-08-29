using UnityEngine;

namespace Dev.Extensions
{
    public static class ScreenExtansions
    {
        public static Vector2 ClampToScreenSize(Vector2 screenPosition)
        {
            screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
            screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);

            return screenPosition;
        }

        public static Vector2 ClampByDistanceMultiplier(Vector2 startPosition, Vector2 targetPosition, float distanceMultiplier)
        {
            if (distanceMultiplier >= 1f || distanceMultiplier <= 0f)
                return targetPosition;

            var maxDistance = (Screen.width / 2f) * distanceMultiplier;
            var direction = targetPosition - startPosition;
            var clampedDirection = Vector2.ClampMagnitude(direction, maxDistance);
            return startPosition + clampedDirection;
        }
    }
}