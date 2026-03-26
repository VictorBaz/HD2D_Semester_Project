using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParryData))]
public class ParryDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); 

        ParryData data = (ParryData)target;
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Visualisation de la Fenêtre de Parade", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if (data.ParryAnimationClip != null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            float animLen = data.ParryAnimationClip.length;
            string label = $"Animation : {data.ParryAnimationClip.name} ({animLen:F2}s)";
            EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel);
            
            GUILayout.Space(5);

            DrawTimelineRow("Parry Window", animLen, data.ParryHitboxStartOffset, data.ParryActiveDuration, new Color(1f, 0.8f, 0f));

            CheckErrors(data, animLen);

            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.HelpBox("Veuillez assigner un Animation Clip pour voir la Timeline.", MessageType.Info);
        }
    }

    private void DrawTimelineRow(string label, float total, float start, float duration, Color barColor)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 24);
        
        Rect labelRect = new Rect(rect.x, rect.y, rect.width * 0.20f, rect.height);
        EditorGUI.LabelField(labelRect, label, EditorStyles.miniLabel);

        Rect barAreaRect = new Rect(rect.x + rect.width * 0.20f, rect.y, rect.width * 0.80f, rect.height);
        
        EditorGUI.DrawRect(barAreaRect, new Color(0.12f, 0.12f, 0.12f)); 

        float startFactor = Mathf.Clamp01(start / total);
        float durationFactor = Mathf.Clamp01(duration / total);
        
        if (startFactor + durationFactor > 1f) durationFactor = 1f - startFactor;

        Rect activeBarRect = new Rect(
            barAreaRect.x + (startFactor * barAreaRect.width),
            barAreaRect.y + 2,
            durationFactor * barAreaRect.width,
            barAreaRect.height - 4
        );

        EditorGUI.DrawRect(activeBarRect, barColor);
        
        string timingText = $"  {start:F2}s -> {(start + duration):F2}s / {total:F2}s";
        EditorGUI.LabelField(barAreaRect, timingText, EditorStyles.whiteMiniLabel);
    }

    private void CheckErrors(ParryData data, float animLen)
    {
        float totalTime = data.ParryHitboxStartOffset + data.ParryActiveDuration;

        if (totalTime > animLen)
        {
            EditorGUILayout.HelpBox($"ERREUR : La fenêtre de Parry finit APRÈS l'animation ({totalTime:F2}s > {animLen:F2}s) !", MessageType.Error);
        }
        
        if (data.ParryHitboxStartOffset < 0 || data.ParryActiveDuration <= 0)
        {
            EditorGUILayout.HelpBox("ATTENTION : Les timings doivent être positifs.", MessageType.Warning);
        }
    }
}