using UnityEngine;
using UnityEditor;


namespace TrippleMergeCity.Utility
{
    [System.Serializable]
    public struct Range
    {
        public float min;
        public float max;

        public Range( float min, float max )
        {
            this.min = min;
            this.max = max;
        }

        public bool IsInRange( float value ) => value >= min && value <= max;
        public float Clamp( float value ) => Mathf.Clamp( value, min, max );
        public float GetRandom() => Random.Range(min, max);
        public int GetRandomInt() => (int)Random.Range(min, max + 1 );
        public float GetLinear( float t ) => min + ( max - min ) * t;

        public float Normalize( float value )
        {
            if ( value == min && value == max )
                return 1;
            else if (value <= min)
                return 0;
            else if (value >= max)
                return 1;

            return NormalizeUnclamped( value );
        }

        public float NormalizeUnclamped( float value ) => ( value - min ) / ( max - min );

        public float Distance() => max - min;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer( typeof( Range ) )]
    public class RangeDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel( position, GUIUtility.GetControlID( FocusType.Passive ), label );

            SerializedProperty min = property.FindPropertyRelative( "min" );
            SerializedProperty max = property.FindPropertyRelative( "max" );

            float halfWidth = position.width / 2;
            float spacing = 8f;
            float halfSpacing = spacing / 2;

            EditorGUI.PropertyField( new Rect( position.x, position.y, halfWidth - halfSpacing, position.height ), min, GUIContent.none );
            EditorGUI.PropertyField( new Rect( position.x + halfWidth + halfSpacing, position.y, halfWidth - halfSpacing, position.height ), max, GUIContent.none );

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) => EditorGUIUtility.singleLineHeight;
    }
#endif
}
