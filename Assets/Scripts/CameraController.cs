using TrippleMergeCity.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace TrippleMergeCity
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private Camera m_uiCamera;
        [SerializeField] private Transform m_cameraHolder;

        [Space, SerializeField] private Range m_minMaxZoom;
        [SerializeField] private Vector2 m_minBounds;
        [SerializeField] private Vector2 m_maxBounds = new( 100, 100 );


        private UniversalRenderPipelineAsset _urpAsset;


        private Transform _cameraTransform;
        private Plane _plane;
        private Vector3 _prevPosition;
        private bool _drag;
        private float _zoom;
        private float _baseShadowDistance;

        private bool _isZooming;
        private Vector3 _pinchWorldCenter;


        private void Awake()
        {
            _plane = new( Vector3.down, Vector3.zero );

            _urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

            _cameraTransform = m_camera.transform;

            _baseShadowDistance = _urpAsset.shadowDistance;

            SetZoom( new( Screen.width / 2, Screen.height / 2 ), 0.5f );
        }


        public void Zoom( Vector2 pinchCenter, Vector2 delta ) => Zoom( pinchCenter, delta.y / 500f );

        public void Zoom( Vector2 pinchCenter, float delta )
        {
            delta *= Mathf.Max( 1 - _zoom, 0.25f );

            SetZoom( pinchCenter, _zoom + delta );
        }


        private void SetZoom( Vector2 pinchCenter, float zoom )
        {
            float zoomBefore = _zoom;

            _zoom = Mathf.Clamp01( zoom );

            /*
            if( !Application.isMobilePlatform )
                TryGetPointOnPlane( GetRayFromViewportPoint( ScreenPointToView( pinchCenter ) ), out _pinchWorldCenter );
            else
            {
                if( !_isZooming )
                {
                    _isZooming = true;
                    TryGetPointOnPlane( GetRayFromViewportPoint( ScreenPointToView( pinchCenter ) ), out _pinchWorldCenter );
                }
            }
            */

            TryGetPointOnPlane( GetRayFromViewportPoint( ScreenPointToView( pinchCenter ) ), out Vector3 currentPinchPosition );

            UpdateZoom();
            
            // TryGetPointOnPlane( GetRayFromViewportPoint( new( 0.5f, 0.5f ) ), out Vector3 newPinchPosition );
            TryGetPointOnPlane( GetRayFromViewportPoint( ScreenPointToView( pinchCenter ) ), out Vector3 newPinchPosition );

            // m_cameraHolder.position -= ( newPinchPosition - _pinchWorldCenter ) * ( _zoom - zoomBefore );
            m_cameraHolder.position -= newPinchPosition - currentPinchPosition;

            UpdateBounds();

            if( IsOutOfBounds() )
                FindMinZoom();
        }


        private void FindMinZoom()
        {
            float newZoom = _zoom;

            int br = 1000000;

            while( IsOutOfBounds() && newZoom < 1 )
            {
                newZoom += 0.002f;
                _zoom = newZoom;

                UpdateZoom();
                UpdateBounds();

                br--;

                if( br <= 0 )
                {
                    Debug.Log( "break" );
                    break;
                }
            }
        }


        private void UpdateZoom()
        {
            float minMaxDelta = m_minMaxZoom.max - m_minMaxZoom.min;
            m_camera.orthographicSize = m_minMaxZoom.min + ( 1 - _zoom ) * minMaxDelta;

            _cameraTransform.localPosition = new( 0f, 0f, -Mathf.Max( m_camera.orthographicSize, 5 ) );

            m_uiCamera.orthographicSize = m_camera.orthographicSize;

            _urpAsset.shadowDistance = ( Mathf.Abs( _cameraTransform.localPosition.z ) + 1 ) * 2;
        }


        public void OnPointerDown( Vector2 pointerPosition )
        {
            if( !TryGetPosition( pointerPosition, out _prevPosition ) )
                return;

            _drag = true;
        }


        public void OnDrag( Vector2 pointerPosition )
        {
            if( !_drag )
                return;

            if( !TryGetPosition( pointerPosition, out Vector3 position ) )
                return;

            Vector3 delta = _prevPosition - position;
            Vector3 newPosition = m_cameraHolder.position + delta;

            m_cameraHolder.position = newPosition;

            UpdateBounds();
        }


        private void UpdateBounds()
        {
            TryGetPointOnPlane( ViewportPointToRay( 0, 1 ), out Vector3 tlPoint );

            Vector3 newPosition = m_cameraHolder.localPosition;

            if( tlPoint.x < m_minBounds.x )
                newPosition.x -= tlPoint.x;

            if( tlPoint.z > m_maxBounds.y )
                newPosition.z -= tlPoint.z - 100;
            
            TryGetPointOnPlane( ViewportPointToRay( 1, 0 ), out Vector3 brPoint );

            if( brPoint.x > m_maxBounds.x )
                newPosition.x -= brPoint.x - 100;

            if( brPoint.z < m_minBounds.y )
                newPosition.z -= brPoint.z;

            m_cameraHolder.localPosition = newPosition;
        }


        private bool IsOutOfBounds()
        {
            TryGetPointOnPlane( ViewportPointToRay( 0, 1 ), out Vector3 tlPoint );

            if( tlPoint.x < m_minBounds.x )
                return true;

            if( tlPoint.z > m_maxBounds.y )
                return true;
            
            TryGetPointOnPlane( ViewportPointToRay( 1, 0 ), out Vector3 brPoint );

            if( brPoint.x > m_maxBounds.x )
                return true;

            if( brPoint.z < m_minBounds.y )
                return true;

            return false;
        }


        private Ray ViewportPointToRay( Vector2 vector ) => ViewportPointToRay( vector.x, vector.y );

        private Ray ViewportPointToRay( float x, float y )
        {
            float orthographicSize = m_camera.orthographicSize;

            float width = orthographicSize * m_camera.aspect;
            float height = orthographicSize;


            Vector3 point = new()
            {
                x = ( x - 0.5f ) * 2f * width,
                y = ( y - 0.5f ) * 2f * height,
                z = m_camera.nearClipPlane
            };

            Vector3 worldPosition = _cameraTransform.TransformPoint( point );

            return new( worldPosition, _cameraTransform.forward );
        }


        private Vector2 ScreenPointToView( Vector2 vector ) => new( Mathf.Clamp01( vector.x / Screen.width ), Mathf.Clamp01( vector.y / Screen.height ) );

        private Vector2 ScreenPointToView( float x, float y ) => new( Mathf.Clamp01( x / Screen.width ), Mathf.Clamp01( y / Screen.height ) );


        private bool TryGetPointOnPlane( Ray ray, out Vector3 point )
        {
            if( _plane.Raycast( ray, out float distance ) )
            {
                point = ray.GetPoint( distance );
                return true;
            }

            point = Vector3.zero;
            return false;
        }
        
        
        public Ray GetRayFromViewportPoint( Vector2 viewportPoint ) => GetRayFromViewportPoint( _cameraTransform, m_camera.orthographicSize, new( viewportPoint.x, viewportPoint.y, m_camera.orthographicSize ) );

        public static Ray GetRayFromViewportPoint( Transform cameraTransform, float orthographicSize, Vector3 viewportPoint )
        {
            // Calculate world space dimensions of the orthographic view
            float halfOrthoHeight = orthographicSize;
            float halfOrthoWidth = halfOrthoHeight * Camera.main.aspect; // Assuming Camera.main aspect if no camera component is available.

            // Calculate world space position of the viewport point
            Vector3 worldPoint = cameraTransform.position;
            worldPoint.x += (viewportPoint.x - 0.5f) * 2f * halfOrthoWidth;
            worldPoint.y += (viewportPoint.y - 0.5f) * 2f * halfOrthoHeight;

            // The ray's origin is the calculated world point
            Vector3 rayOrigin = worldPoint;

            // Orthographic rays are parallel to the camera's forward direction
            Vector3 rayDirection = cameraTransform.forward;

            return new Ray(rayOrigin, rayDirection);
        }

        
        public void OnPointerUp()
        {
            _drag = false;
            _isZooming = false;
        }


        public Ray PointerToRay( Vector2 pointerPosition ) => m_camera.ScreenPointToRay( pointerPosition );


        private bool TryGetPosition( Vector2 pointerPosition, out Vector3 position )
        {
            position = default;

            Ray ray = PointerToRay( pointerPosition );

            if( !_plane.Raycast( ray, out float distance ) )
                return false;

            position = ray.GetPoint( distance );
            return true;
        }


        private void OnDestroy()
        {
            if( _urpAsset )
                _urpAsset.shadowDistance = _baseShadowDistance;
        }
    }
}