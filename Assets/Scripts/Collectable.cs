using UnityEngine;

namespace TrippleMergeCity
{
    public class Collectable : MonoBehaviour, ICollectable
    {
        [SerializeField] private Vector3 m_defaultRotation;

        
        public void Collect( Transform target )
        {
            gameObject.SetActive( false );
        }
    }
}
