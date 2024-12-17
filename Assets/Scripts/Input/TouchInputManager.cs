using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;


namespace TrippleMergeCity.Input
{
    public class TouchInputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput m_playerInput;

        private GameManager _gameManager;
        private bool _dragged;
        private bool _zooming;

        private Vector2 _startPosition;
        private Vector2 _firstPosition;
        private Vector2 _secondsPosition;
        private float _prevDelta;
        private bool _pressed;

        private Vector2 _mousePosition;


        private HashSet<int> _touches = new();


        public void Init( GameManager gameManager )
        {
#if UNITY_EDITOR
            m_playerInput.Bind( "Gameplay/MouseClick", OnMouseClick );
            m_playerInput.Bind( "Gameplay/MousePosition", OnMousePosition );
#endif
            _gameManager = gameManager;


            m_playerInput.Bind( "Gameplay/Touch", ( touch ) => OnTouch( touch, 0 ) );
            m_playerInput.Bind( "Gameplay/SecondTouch", ( touch ) => OnTouch( touch, 1 ) );
        }


        private void OnMouseClick( bool pressed )
        {
            _pressed = pressed;

            if( pressed )
                OnPointerDown( _mousePosition, 0 );
            else
                OnPointerUp( _mousePosition, 0 );
        }


        private void OnMousePosition( Vector2 position )
        {
            _mousePosition = position;
            
            if( _pressed )
                OnDrag( _mousePosition, 0 );
        }


        private void OnTouch( TouchState touch, int touchID )
        {
            if( touch.phase == UnityEngine.InputSystem.TouchPhase.Moved )
            {
                if( !_touches.Contains( touchID ) )
                    OnPointerDown( touch.position, touchID );
                else
                    OnDrag( touch.position, touchID );
            }
            else if( touch.phase == UnityEngine.InputSystem.TouchPhase.Ended )
                OnPointerUp( touch.position, touchID );
        }


        private void Update()
        {
            
        }

        
        public void OnPointerDown( Vector2 pointerPosition, int touchID )
        {
            if( !_touches.Add( touchID ) )
                return;

            if( touchID > 1 )
                return;

            _gameManager.CameraController.OnPointerDown( pointerPosition );

            if( touchID == 0 )
            {
                _firstPosition = pointerPosition;
                _startPosition = pointerPosition;
            }

            if( touchID == 1 )
                _secondsPosition = pointerPosition;

            if( touchID > 0 )
                SetZooming( true );
        }


        public void OnDrag( Vector2 pointerPosition, int touchID )
        {
            if( !_touches.Contains( touchID ) )
                return;

            if( touchID > 1 )
                return;

            if( _zooming )
            {
                if( touchID == 0 )
                    _firstPosition = pointerPosition;
    
                if( touchID == 1 )
                    _secondsPosition = pointerPosition;

                float delta = Vector2.Distance( _firstPosition, _secondsPosition );
                float dd = delta - _prevDelta;
                _gameManager.CameraController.Zoom( dd / 500f );

                _prevDelta = delta;
            }
            else
            {
                if( Vector2.Distance( _startPosition, pointerPosition ) < 5f )
                    return;
                    
                _dragged = true;
                _gameManager.CameraController.OnDrag( pointerPosition );
            }
        }

        
        public void OnPointerUp( Vector2 pointerPosition, int touchID )
        {
            if( !_touches.Remove( touchID ) )
                return;

            if( touchID > 1 )
                return;

            if( _zooming )
            {
                SetZooming( false );
                return;
            }

            if( !_dragged )
            {
                _gameManager.OnPointerClick( pointerPosition );
                return;
            }

            _dragged = false;
            _gameManager.CameraController.OnPointerUp();
        }


        private void SetZooming( bool value )
        {
            _zooming = value;

            if( value )
            {
                _dragged = false;
                _gameManager.CameraController.OnPointerUp();

                _prevDelta = Vector2.Distance( _firstPosition, _secondsPosition );
            }
        }
    }
}