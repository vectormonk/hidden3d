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


        private UniversalRenderPipelineAsset _urpAsset;


        private Plane _plane;
        private Vector3 _prevPosition;
        private bool _drag;
        private float _zoom;
        private float _baseShadowDistance;


        private void Awake()
        {
            _plane = new( Vector3.down, Vector3.zero );

            _urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

            _baseShadowDistance = _urpAsset.shadowDistance;

            SetZoom( 0.5f );
        }


        public void Zoom( Vector2 delta ) => Zoom( delta.y / 3000f );

        public void Zoom( float delta )
        {
            SetZoom( _zoom + delta );
        }


        private void SetZoom( float zoom )
        {
            _zoom = Mathf.Clamp01( zoom );

            float d = m_minMaxZoom.max - m_minMaxZoom.min;
            m_camera.orthographicSize = m_minMaxZoom.min + ( 1 - _zoom ) * d;

            m_camera.transform.localPosition = new( 0f, 0f, -m_camera.orthographicSize );

            m_uiCamera.orthographicSize = m_camera.orthographicSize;

            _urpAsset.shadowDistance = ( Mathf.Abs( m_camera.transform.localPosition.z ) + 1 ) * 2;
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

            // newPosition.x = Mathf.Clamp( newPosition.x, -m_minBounds.x, m_minBounds.x );
            // newPosition.z = Mathf.Clamp( newPosition.z, -m_minBounds.y, m_minBounds.y );
            
            m_cameraHolder.position = newPosition;
        }

        
        public void OnPointerUp()
        {
            _drag = false;
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