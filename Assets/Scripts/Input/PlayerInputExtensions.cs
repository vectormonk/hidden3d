using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;


namespace TrippleMergeCity.Input
{
    public static class PlayerInputExtensions
    {
        public static void Bind( this PlayerInput playerInput, string actionName, UnityEvent<bool> updatedAction )
        {
            var action = playerInput.actions[ actionName ];
			action.performed += Update;
			action.canceled += Update;

            return;

            void Update( InputAction.CallbackContext context ) => updatedAction?.Invoke( context.ReadValueAsButton() );
        }


        public static void Bind( this PlayerInput playerInput, string actionName, ButtonEvents updatedAction )
        {
            var action = playerInput.actions[ actionName ];
			action.performed += Update;
			action.canceled += Update;

            return;

            void Update( InputAction.CallbackContext context )
            {
                bool pressed = context.ReadValueAsButton();
                
                updatedAction.OnUpdated?.Invoke( pressed );
                if( pressed )
                    updatedAction.OnPressed?.Invoke();
                else
                    updatedAction.OnReleased?.Invoke();
            }
        }


        public static void Bind( this PlayerInput playerInput, string actionName, UnityEvent<float> updatedAction )
        {
            var action = playerInput.actions[ actionName ];
			action.performed += Update;
			action.canceled += Update;

            return;

            void Update( InputAction.CallbackContext context ) => updatedAction?.Invoke( context.ReadValue<float>() );
        }


        public static void Bind( this PlayerInput playerInput, string actionName, UnityEvent<Vector2> updatedAction )
        {
            var action = playerInput.actions[ actionName ];
			action.performed += Update;
			action.canceled += Update;

            return;

            void Update( InputAction.CallbackContext context ) => updatedAction?.Invoke( context.ReadValue<Vector2>() );
        }


        public static void Bind( this PlayerInput playerInput, string actionName, UnityAction<Vector2> updatedAction )
        {
            var action = playerInput.actions[ actionName ];
			action.performed += Update;
			action.canceled += Update;

            return;

            void Update( InputAction.CallbackContext context ) => updatedAction?.Invoke( context.ReadValue<Vector2>() );
        }


        public static void Bind( this PlayerInput playerInput, string actionName, UnityAction<TouchState> updatedAction )
        {
            var action = playerInput.actions[ actionName ];
            
            action.started += ( context ) => updatedAction?.Invoke( context.ReadValue<TouchState>() );
			action.performed += ( context ) => updatedAction?.Invoke( context.ReadValue<TouchState>() );
			action.canceled += ( context ) => updatedAction?.Invoke( context.ReadValue<TouchState>() );
        }
    }


    public enum TouchAction
    {
        Perform,
        End
    }
}
