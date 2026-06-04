#if UNITY_EDITOR
using UnityEngine;
using System;
using Steamworks;
using UnityEditor;

[CustomEditor(typeof(SteamAchivementManager))] 
public class SteamManagerEditor : Editor
{
    private string achievementIdInput = "TEST_ACH";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SteamAchivementManager script = (SteamAchivementManager)target;

        GUILayout.Space(10);
        GUILayout.Label("Steamworks Testing Tools", EditorStyles.boldLabel);

        achievementIdInput = EditorGUILayout.TextField("Achievement ID", achievementIdInput);
        
        if (GUILayout.Button("Unlock Achievement"))
        {
            script.UnlockAchivement(achievementIdInput);
        }

        if (GUILayout.Button("Check If Unlocked"))
        {
            script.IsThisAchievementUnlocked(achievementIdInput);
        }

        if (GUILayout.Button("Clear Achievement"))
        {
            script.ClearAchivement(achievementIdInput);
        }

        GUILayout.Space(5);
        
        if (GUILayout.Button("Clear ALL Achievements"))
        {
            script.ClearEveryAchivements(); 
        }
    }
}
#endif