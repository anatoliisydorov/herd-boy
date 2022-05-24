// using System;
// using Dev.Movement;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using static Dev.Input.InputActions;

// namespace Dev.Input
// {
//     public class TestInput : MonoBehaviour, IGameplayActions
//     {
//         private InputActions _inputActions;

//         public Action<Vector3> OnMovementChange;

//         private void Awake()
//         {
//             _inputActions = new InputActions();
//             _inputActions.Gameplay.SetCallbacks(this);

//             if (TryGetComponent(out BasicMovement movement))
//             {
//                 OnMovementChange += movement.Move;
//             }
//         }

//         private void OnEnable()
//         {
//             _inputActions.Gameplay.Enable();
//         }

//         private void OnDisable()
//         {
//             _inputActions.Gameplay.Disable();
//         }

//         public void OnMovement(InputAction.CallbackContext context)
//         {
//             Debug.Log($"Movement - {context.ReadValue<Vector2>()}");
//             Vector2 inputVector = context.ReadValue<Vector2>();
//             Vector3 moveVector = new Vector3(inputVector.x, 0f, inputVector.y);
//             OnMovementChange?.Invoke(moveVector);
//         }
//     }
// }