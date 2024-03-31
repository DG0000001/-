///
///     JsonDataEditor.cs
///
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using cfg.sceneData;

[System.Serializable]
public class GenerateEnemyConfigurationList
{
    public List<GenerateEnemyConfigurationInOneScene> generateEnemyConfigurationByOneSceneDatas;
}
public class JsonDataEditor : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    SerializedObject serializedObject;
    SerializedProperty serializedProperty;

    public GenerateEnemyConfigurationList generateEnemyConfiguration;

    private string filePath = "";

    [MenuItem("Window/Json Data Editor")]
    static void Init()
    {
        GetWindow(typeof(JsonDataEditor)).Show();
    }
    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        serializedProperty = serializedObject.FindProperty("generateEnemyConfiguration");
    }
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        if (GUILayout.Button("Load json data"))
            LoadAllJsonData();
        EditorGUILayout.Space();
        if (GUILayout.Button("Save data to json"))
            SaveAllDataToJson();
        EditorGUILayout.Space();


        #region SceneData
        GUILayout.BeginVertical("Box");
        if (generateEnemyConfiguration != null)
        {
            GUILayout.Label(filePath);
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();                     //应用修改的属性
        }
        #endregion

        EditorGUILayout.EndScrollView();
    }
    private void LoadSceneJsonData()
    {
        filePath = Application.dataPath + "../../GenerateDatas/myJson/" + "GenerateEnemyConfiguration-scenedata" + ".json";
        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);                 //读取文件
            generateEnemyConfiguration = JsonUtility.FromJson<GenerateEnemyConfigurationList>(dataAsJson);
            
        }
    }
    private void SaveSceneDataToJson()
    {
        filePath = Application.dataPath + "../../GenerateDatas/myJson/" + "GenerateEnemyConfiguration-scenedata" + ".json";
        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(generateEnemyConfiguration);              //序列化JSON
            File.WriteAllText(filePath, dataAsJson);                        //写入文件
        }
    }
    private void LoadAllJsonData()
    {
        LoadSceneJsonData();
    }
    private void SaveAllDataToJson()
    {
        SaveSceneDataToJson();
    }
}
