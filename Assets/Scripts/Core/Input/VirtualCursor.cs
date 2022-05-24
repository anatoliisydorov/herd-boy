using UnityEngine;
using UnityEngine.InputSystem;

namespace Dev.Input
{
    public class VirtualCursor
    {
        public void ResetCursorPosition()
        {
            Vector2 newScreenPosition = new Vector2(Screen.width / 2, Screen.height / 2);
            SetCursorPosition(newScreenPosition);
        }

        public void SetCursorPosition(Vector2 screenPosition)
        {
            Mouse.current.WarpCursorPosition(screenPosition);
        }

        public Vector2 GetCursorPosition()
        {
            return Mouse.current.position.ReadValue();
        }
    }
}