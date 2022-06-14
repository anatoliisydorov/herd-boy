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
    }
}