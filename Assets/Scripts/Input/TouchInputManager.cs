using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;


namespace TrippleMergeCity.Input
{
    public class TouchInputManager : MonoBehaviour //, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private PlayerInput m_playerInput;

        private GameManager _gameManager;
        private bool _dragged;
        private bool _zooming;

        private Vector2 _firstPosition;
        private Vector2 _secondsPosition;
        private float _prevDelta;


        public void Init( GameManager gameManager )
        {
            _gameManager = gameManager;


            m_playerInput.Bind( "Gameplay/Touch", ( touch ) => OnTouch( touch, 0 ) );
            m_playerInput.Bind( "Gameplay/SecondTouch", ( touch ) => OnTouch( touch, 1 ) );
        }


        private void OnTouch( TouchState touch, int touchID )
        {
            if( touch.phase == UnityEngine.InputSystem.TouchPhase.Began )
                OnPointerDown( touch, touchID );
            else if( touch.phase == UnityEngine.InputSystem.TouchPhase.Moved )
                OnDrag( touch, touchID );
            else if( touch.phase == UnityEngine.InputSystem.TouchPhase.Ended )
                OnPointerUp( touch, touchID );
        }


        private void Update()
        {
            
        }

        
        public void OnPointerDown( TouchState touch, int touchID )
        {
            if( touchID > 1 )
                return;

            _gameManager.CameraController.OnPointerDown( touch.position );

            if( touchID == 0 )
                _firstPosition = touch.position;

            if( touchID == 1 )
                _secondsPosition = touch.position;

            if( touchID > 0 )
                SetZooming( true );
        }


        public void OnDrag( TouchState touch, int touchID )
        {
            if( touchID > 1 )
                return;

            if( _zooming )
            {
                if( touchID == 0 )
                    _firstPosition = touch.position;
    
                if( touchID == 1 )
                    _secondsPosition = touch.position;

                float delta = Vector2.Distance( _firstPosition, _secondsPosition );
                float dd = delta - _prevDelta;
                _gameManager.CameraController.Zoom( dd / 500f );

                _prevDelta = delta;
            }
            else
            {
                _dragged = true;
                _gameManager.CameraController.OnDrag( touch.position );
            }
        }

        
        public void OnPointerUp( TouchState touch, int touchID )
        {
            if( touchID > 1 )
                return;

            if( _zooming )
            {
                SetZooming( false );
                return;
            }

            if( !_dragged )
            {
                _gameManager.OnPointerClick( touch.position );
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