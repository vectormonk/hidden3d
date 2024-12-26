using TrippleMergeCity.Input;
using UnityEngine;
using UnityEngine.InputSystem;


namespace TrippleMergeCity
{
    public class GameManager : MonoBehaviour
    {
        [field: SerializeField] public CameraController CameraController { get; private set; }
        [SerializeField] private PlayerInput m_playerInput;
        [SerializeField] private LayerMask m_layerMask;


        public event System.Action<Collectable> OnCollected;


        private void Awake()
        {
            m_playerInput.Bind( "Gameplay/Scroll", CameraController.Zoom );
            m_playerInput.actions.FindActionMap( "Gameplay" ).Enable();

            SetMaxAvailableFrameRate();
        }


        void Update()
        {

        }


        public void OnPointerClick( Vector2 pointerPosition )
        {
            Ray ray = CameraController.PointerToRay( pointerPosition );
            if( !Physics.SphereCast( ray, 0.1f, out RaycastHit hit, 100f, m_layerMask ) )
                return;

            if( !hit.transform.TryGetComponent( out Collectable collectable ) )
                return;

            OnCollected?.Invoke( collectable );
        }
        

        private static void SetMaxAvailableFrameRate()
        {
            int maxRefreshRate = 60;
#if UNITY_EDITOR
            maxRefreshRate = 240;
#endif
            foreach ( Resolution t in Screen.resolutions )
            {
                if ( maxRefreshRate < t.refreshRate )
                    maxRefreshRate = Mathf.CeilToInt( t.refreshRate );
            }

            Application.targetFrameRate = maxRefreshRate;
            Debug.Log("[GameSettings]: Set target frame rate to: " + Application.targetFrameRate);
        }
    }
}