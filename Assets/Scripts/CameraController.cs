using UnityEngine;
using UnityEngine.EventSystems;


namespace TrippleMergeCity
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private Camera m_uiCamera;
        [SerializeField] private Transform m_cameraHolder;
        [SerializeField] private Vector2 m_minMaxZoom;


        private Plane _plane;
        private Vector3 _prevPosition;
        private bool _drag;
        private float _zoom;


        private void Awake()
        {
            _plane = new( Vector3.down, Vector3.zero );
        }


        public void Zoom( Vector2 delta ) => Zoom( delta.y / 500f );

        public void Zoom( float delta )
        {
            SetZoom( Mathf.Clamp( _zoom + delta, m_minMaxZoom.x, m_minMaxZoom.y ) );
        }


        private void SetZoom( float zoom )
        {
            _zoom = zoom;

            float maxFOV = 60;

            m_camera.fieldOfView = maxFOV - ( _zoom - 1 ) * ( maxFOV / m_minMaxZoom.y );
            m_uiCamera.fieldOfView = m_camera.fieldOfView;

            m_camera.transform.localPosition = new( 0f, 0f, -10 + ( _zoom - 1 ) * 2.5f );
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

            m_cameraHolder.Translate( _prevPosition - position, Space.World );
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
    }
}