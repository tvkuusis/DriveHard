using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
[CustomEditor(typeof(SnapToGrid), true)]
[CanEditMultipleObjects]
public class SnapToGridEditor : Editor {

    public override void OnInspectorGUI() {
        SnapToGrid actor = target as SnapToGrid;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotate Y-axis", GUILayout.Width(100));
        actor.rotateY = EditorGUILayout.Toggle(actor.rotateY);
        GUILayout.EndHorizontal();
        //base.OnInspectorGUI();

        //SnapToGrid actor = target as SnapToGrid;
        //if (actor.snapToGrid) {
        //    actor.transform.position = RoundTransform(actor.transform.position, actor.snapValue);
        //}

        //if (actor.sizeToGrid) {
        //    actor.transform.localScale = RoundTransform(actor.transform.localScale, actor.sizeValue);
        //}


        //if (Event.current.keyCode == KeyCode.Tab) {
        //    actor.transform.Rotate(Vector3.up, 90);
        //}

        //int controlID = GUIUtility.GetControlID(FocusType.Passive);
        //if (Event.current.type == EventType.Layout) {
        //    HandleUtility.AddDefaultControl(controlID);

        //}
        //Event.current.Use();
    }
    

    void OnSceneGUI() {
        SnapToGrid actor = target as SnapToGrid;
        Event e = Event.current;
        switch (e.type) {
            case EventType.KeyDown: {
                    if (Event.current.keyCode == (KeyCode.Tab)) {
                        Undo.RecordObject(actor.transform, "Rotated object");
                        actor.Rot();
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    }
                    if(Event.current.keyCode == (KeyCode.Space)) {
                        Undo.RecordObject(actor.transform, "Scaled object");
                        actor.transform.localScale = new Vector3(actor.transform.localScale.x * -1, actor.transform.localScale.y, actor.transform.localScale.z);
                        //SceneView.lastActiveSceneView.Focus();
                        //EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
                        //EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    }
                    if (Event.current.keyCode == (KeyCode.C)) {
                        Undo.RecordObject(actor.transform, "Toggled rotation");
                        actor.rotateY = !actor.rotateY;
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    }
                    break;
            }
        }
        
    }
    // The snapping code
    private Vector3 RoundTransform(Vector3 v, float snapValue) {
        return new Vector3
        (
            snapValue * Mathf.Round(v.x / snapValue),
            snapValue * Mathf.Round(v.y / snapValue),
            snapValue * Mathf.Round(v.z / snapValue)
            //v.z
        );
    }

}