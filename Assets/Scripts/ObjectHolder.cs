using PrimeTween;
using UnityEngine;

namespace TrippleMergeCity
{
    public class ObjectHolder : MonoBehaviour
    {
        [SerializeField] private Transform m_objectHolder;
        [SerializeField] private float m_duration = 0.5f;


        private Collectable _obj;


        public bool IsEmpty => _obj == null;


        private Sequence _sequance;


        public void Collect( Collectable collectable )
        {
            Vector3 toScale = collectable.transform.localScale;

            collectable.transform.SetParent( m_objectHolder, true );

            Vector3 scaleFrom = collectable.transform.localScale;
            

            collectable.gameObject.layer = m_objectHolder.gameObject.layer;

            _sequance.Stop();

            _sequance = Sequence.Create( useUnscaledTime: false );
            _sequance.Chain( Tween.Custom( 0, 1, m_duration * 6f, Move ) );
            _sequance.Group( Tween.Scale( collectable.transform, scaleFrom, toScale, m_duration ) );

            _obj = collectable;
            
            return;

            void Move( float value )
            {
                collectable.transform.position = Vector3.Lerp( collectable.transform.position, m_objectHolder.position, value );
                collectable.transform.rotation = Quaternion.Lerp( collectable.transform.rotation, m_objectHolder.rotation, value );
            }
        }
    }
}
