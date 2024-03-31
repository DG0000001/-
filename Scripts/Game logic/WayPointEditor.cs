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
        // ����Handles����ɫΪ��ɫ
        Handles.color = Color.red;

        // ѭ������Waypoint��Points�����е�ÿ����
        for (int i = 0; i < Waypoint.Points.Length; i++)
        {
            EditorGUI.BeginChangeCheck();

            // ���������ƶ������Handles
            Vector3 currentWaypointPoint = Waypoint.CurrentPosition + Waypoint.Points[i];

            Vector3 newWaypointPoint = Handles.FreeMoveHandle(
                currentWaypointPoint,
                Quaternion.identity,
                0.7f,
                new Vector3(0.3f, 0.3f, 0.3f),
                Handles.SphereHandleCap
                );

            // ����������ʾ�����������ı�
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.fontSize = 16;
            textStyle.normal.textColor = Color.white;

            Vector3 textAlligment = Vector3.down * 0.35f + Vector3.right * 0.35f;
            Handles.Label(Waypoint.CurrentPosition + Waypoint.Points[i] + textAlligment, $"{i + 1}", textStyle);

            EditorGUI.EndChangeCheck();

            // ��麽��λ���Ƿ����˱仯
            if (EditorGUI.EndChangeCheck())
            {
                // Ϊ�˳�����������¼����
                Undo.RecordObject(target, "Free Move Handle");

                // ����Handles����λ�ø��º���λ��
                Waypoint.Points[i] = newWaypointPoint - Waypoint.CurrentPosition;
            }
        }
    }
}