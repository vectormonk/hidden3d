using System.Collections.Generic;
using UnityEngine;


namespace TrippleMergeCity
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameManager m_gameManager;
        [SerializeField] private List<ObjectHolder> m_objectsHolders;


        private void Awake()
        {
            m_gameManager.OnCollected += Collect;
        }


        public void Collect( Collectable collectable )
        {
            for( int i = 0; i < m_objectsHolders.Count; i++ )
            {
                if( !m_objectsHolders[ i ].IsEmpty )
                    continue;

                m_objectsHolders[ i ].Collect( collectable );
                return;
            }
        }
    }
}