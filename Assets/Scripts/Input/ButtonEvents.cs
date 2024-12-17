using UnityEngine;
using UnityEngine.Events;


namespace TrippleMergeCity.Input
{
    public class ButtonEvents
    {
        public UnityEvent<bool> OnUpdated = new();
        public UnityEvent OnPressed = new();
        public UnityEvent OnReleased = new();


        public void AddListener( UnityAction<bool> call ) => OnUpdated.AddListener( call );
    }
}