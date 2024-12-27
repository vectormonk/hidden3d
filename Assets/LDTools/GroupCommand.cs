using UnityEditor;
using UnityEngine;

namespace VW.Client.Core.Editor.LDTools {
    public static class GroupCommand {
        #region Private

#if UNITY_EDITOR
        [MenuItem("GameObject/Group Selected %g")]
        private static void GroupSelected() {
            if (!Selection.activeTransform) {
                return;
            }

            var go = new GameObject(Selection.activeTransform.name + " Group");
            go.isStatic = true;
            go.layer = LayerMask.NameToLayer("Environment");
            var GOPos = new Vector3(0, 0, 0);
            var length = Selection.objects.Length;

            for (var i = 0; i < length; i++) {
                GOPos += Selection.transforms[i].localPosition;
            }

            GOPos = GOPos / length;

            //go.transform.SetPositionAndRotation(Selection.activeTransform.localPosition, new Quaternion());
            go.transform.SetPositionAndRotation(GOPos, new Quaternion());

            Undo.RegisterCreatedObjectUndo(go, "Group Selected");
            go.transform.SetParent(Selection.activeTransform.parent, false);

            foreach (var transform in Selection.transforms) {
                Undo.SetTransformParent(transform, go.transform, "Group Selected");
            }

            Selection.activeGameObject = go;
        }
#endif
        #endregion
    }
}