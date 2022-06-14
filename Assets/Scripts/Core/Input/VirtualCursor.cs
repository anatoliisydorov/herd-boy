using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace Dev.Input
{
    public class VirtualCursor
    {
        private Mouse _virtualMouse;

        public VirtualCursor()
        {
            _virtualMouse = Mouse.current;
        }

        public void ResetCursorPosition()
        {
            Vector2 newScreenPosition = new Vector2(Screen.width / 2, Screen.height / 2);
            SetCursorPosition(newScreenPosition);
        }

        public void SetCursorPosition(Vector2 screenPosition)
        {
            Vector2 correctScreenPosition = Extensions.ScreenExtansions.ClampToScreenSize(screenPosition);
            _virtualMouse.WarpCursorPosition(correctScreenPosition);
        }

        public Vector2 GetCursorPosition()
        {
            return _virtualMouse.position.ReadValue();
        }

        public void SetCursorVisibility(bool visibility)
        {
            Cursor.visible = visibility;
        }
    }
}