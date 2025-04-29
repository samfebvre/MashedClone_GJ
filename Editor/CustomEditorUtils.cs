using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class CustomEditorUtils
    {

        #region Public Methods

        // Custom function to draw horizontal line
        public static void DrawSeparator( float thickness = 1f, float padding = 10f )
        {
            // Add some empty space
            EditorGUILayout.Space();

            Rect horizontalLine = EditorGUILayout.GetControlRect( false, thickness, GUILayout.ExpandWidth( true ) );
            horizontalLine.height =  thickness;
            horizontalLine.y      += padding / 2;
            horizontalLine.x      -= 2;

            EditorGUI.DrawRect( horizontalLine, Color.grey );

            // Add some empty space
            EditorGUILayout.Space();
        }

        #endregion

    }
}