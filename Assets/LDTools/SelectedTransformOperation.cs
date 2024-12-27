using UnityEditor;
using UnityEngine;

namespace VW.Client.Core.Editor.LDTools {
    public static class SelectedTransformOperation {
        #region Private

        [MenuItem("GameObject/Transform Operation/Random Transform Selected %r")]
        private static void RandomTransform() {
            const float XZ_RAND = 5.0f;
            const float Y_RAND = 90.0f;
            const float SCALE_RANGE = 0.1f;

            foreach (var selectedTransform in Selection.transforms) {
                var scaleRandInt = Random.Range(-1 * SCALE_RANGE, SCALE_RANGE);
                var yRandFloat = Random.Range(-1 * Y_RAND, Y_RAND);
                var xRandFloat = Random.Range(-1 * XZ_RAND, XZ_RAND);
                var zRandFloat = Random.Range(-1 * XZ_RAND, XZ_RAND);

                var scaleChange = new Vector3(scaleRandInt, scaleRandInt, scaleRandInt);
                selectedTransform.Rotate(xRandFloat, yRandFloat, zRandFloat, Space.Self);
                selectedTransform.localScale = scaleChange + Vector3.one;
            }
        }

        [MenuItem("GameObject/Transform Operation/Set default scale and rotate &r")]
        private static void RotateScaleDef() {
            foreach (var selectedTransform in Selection.transforms) {
                selectedTransform.localRotation = Quaternion.identity;
                selectedTransform.localScale = Vector3.one;
            }
        }

        #endregion
    }
}