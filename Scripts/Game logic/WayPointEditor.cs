using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WayPoint))]
public class WayPointEditor : Editor
{
    WayPoint Waypoint => target as WayPoint;

    private void OnSceneGUI()
    {
        // 设置Handles的颜色为青色
        Handles.color = Color.red;

        // 循环遍历Waypoint的Points数组中的每个点
        for (int i = 0; i < Waypoint.Points.Length; i++)
        {
            EditorGUI.BeginChangeCheck();

            // 创建用于移动航点的Handles
            Vector3 currentWaypointPoint = Waypoint.CurrentPosition + Waypoint.Points[i];

            Vector3 newWaypointPoint = Handles.FreeMoveHandle(
                currentWaypointPoint,
                Quaternion.identity,
                0.7f,
                new Vector3(0.3f, 0.3f, 0.3f),
                Handles.SphereHandleCap
                );

            // 创建用于显示航点索引的文本
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.fontSize = 16;
            textStyle.normal.textColor = Color.white;

            Vector3 textAlligment = Vector3.down * 0.35f + Vector3.right * 0.35f;
            Handles.Label(Waypoint.CurrentPosition + Waypoint.Points[i] + textAlligment, $"{i + 1}", textStyle);

            EditorGUI.EndChangeCheck();

            // 检查航点位置是否发生了变化
            if (EditorGUI.EndChangeCheck())
            {
                // 为了撤销操作，记录对象
                Undo.RecordObject(target, "Free Move Handle");

                // 根据Handles的新位置更新航点位置
                Waypoint.Points[i] = newWaypointPoint - Waypoint.CurrentPosition;
            }
        }
    }
}