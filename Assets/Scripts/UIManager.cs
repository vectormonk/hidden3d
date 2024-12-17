using System.Collections.Generic;
using TrippleMergeCity.Input;
using UnityEngine;


namespace TrippleMergeCity
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameManager m_gameManager;
        [SerializeField] private TouchInputManager m_touchInput;

        [SerializeField] private List<ObjectHolder> m_objectsHolders;


        private void Awake()
        {
            m_touchInput.Init( m_gameManager ); 

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