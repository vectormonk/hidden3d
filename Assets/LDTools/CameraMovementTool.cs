using UnityEditor;
using UnityEngine;

public class CameraMovementTool : EditorWindow
{
    private GameObject ldCamera;
    private float moveSpeed = 1f;
    private bool isCameraActive = true; // Стан активності камери
    private float cameraSize = 20f; // Розмір ортографічної камери

    [MenuItem("Tools/Camera Movement Tool")]
    public static void ShowWindow()
    {
        GetWindow<CameraMovementTool>("Camera Movement Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("LD Camera Movement", EditorStyles.boldLabel);

        ldCamera = (GameObject)EditorGUILayout.ObjectField("LD Camera", ldCamera, typeof(GameObject), true);
        moveSpeed = EditorGUILayout.FloatField("Move Speed", moveSpeed);

        if (ldCamera != null)
        {
            GUILayout.Space(10);

            // Включення/відключення камери
            isCameraActive = EditorGUILayout.Toggle("Enable Camera", isCameraActive);
            ToggleCamera(isCameraActive);

            GUILayout.Space(10);

            // Налаштування параметра Size для ортографічної камери
            Camera cameraComponent = ldCamera.GetComponent<Camera>();
            if (cameraComponent != null && cameraComponent.orthographic)
            {
                GUILayout.Label("Camera Size", EditorStyles.boldLabel);

                // Повзунок для налаштування масштабу
                cameraSize = EditorGUILayout.Slider(cameraSize, 5f, 100f);
                cameraComponent.orthographicSize = cameraSize;

                GUILayout.Space(10);

                // Пресети масштабу
                GUILayout.Label("Zoom Presets", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("5", GUILayout.Width(50))) SetCameraSize(5f);
                if (GUILayout.Button("10", GUILayout.Width(50))) SetCameraSize(10f);
                if (GUILayout.Button("15", GUILayout.Width(50))) SetCameraSize(15f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("20", GUILayout.Width(50))) SetCameraSize(20f);
                if (GUILayout.Button("25", GUILayout.Width(50))) SetCameraSize(25f);
                if (GUILayout.Button("30", GUILayout.Width(50))) SetCameraSize(30f);
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            GUILayout.Label("Move Camera", EditorStyles.boldLabel);

            // Хрестоподібне розташування кнопок
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+Y", GUILayout.Width(50)))
                MoveCamera(Vector3.up);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-X", GUILayout.Width(50)))
                MoveCamera(Vector3.left);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+X", GUILayout.Width(50)))
                MoveCamera(Vector3.right);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-Y", GUILayout.Width(50)))
                MoveCamera(Vector3.down);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("Please assign a camera to control.", MessageType.Warning);
        }
    }

    private void MoveCamera(Vector3 direction)
    {
        if (ldCamera != null && isCameraActive)
        {
            Undo.RecordObject(ldCamera.transform, "Move Camera");
            ldCamera.transform.position += direction * moveSpeed;
        }
    }

    private void ToggleCamera(bool enable)
    {
        if (ldCamera != null)
        {
            Camera cameraComponent = ldCamera.GetComponent<Camera>();
            if (cameraComponent != null)
            {
                cameraComponent.enabled = enable;
            }
        }
    }

    private void SetCameraSize(float size)
    {
        if (ldCamera != null)
        {
            Camera cameraComponent = ldCamera.GetComponent<Camera>();
            if (cameraComponent != null && cameraComponent.orthographic)
            {
                Undo.RecordObject(cameraComponent, "Set Camera Size");
                cameraSize = size;
                cameraComponent.orthographicSize = size;
            }
        }
    }
}
