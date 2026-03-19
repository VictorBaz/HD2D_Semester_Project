using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatData))]
public class CombatDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); 

        CombatData data = (CombatData)target;
        if (data.ComboHits == null || data.ComboHits.Length == 0) return;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Visualisation des Timings", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        for (int i = 0; i < data.ComboHits.Length; i++)
        {
            var hit = data.ComboHits[i];
            string label = $"Combo {i} : {(hit.Clip != null ? hit.Clip.name : "Pas d'anim")}";
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel);

            if (hit.Clip != null)
            {
                GUILayout.Space(5);
                
                float animLen = hit.Clip.length;
                
                DrawTimelineRow("Dash", animLen, hit.DashStartOffset, hit.DashDuration,Color.darkBlue);

                DrawTimelineRow("Hitbox", animLen, hit.HitboxStartOffset, hit.HitboxActiveDuration, Color.darkGreen);

                CheckErrors(hit, animLen, i);
            }
            else
            {
                EditorGUILayout.HelpBox("Veuillez assigner un Animation Clip.", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
    }

    private void DrawTimelineRow(string label, float total, float start, float duration, Color barColor)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        
        Rect labelRect = new Rect(rect.x, rect.y, rect.width * 0.15f, rect.height);
        EditorGUI.LabelField(labelRect, label, EditorStyles.miniLabel);

        Rect barAreaRect = new Rect(rect.x + rect.width * 0.15f, rect.y, rect.width * 0.85f, rect.height);
        
        EditorGUI.DrawRect(barAreaRect, new Color(0.15f, 0.15f, 0.15f)); 

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
        
        EditorGUI.LabelField(barAreaRect, $"  {start:F2}s -> {(start+duration):F2}s / {total:F2}s", EditorStyles.whiteMiniLabel);
    }

    private void CheckErrors(CombatHitData hit, float animLen, int index)
    {
        if (hit.DashStartOffset + hit.DashDuration > animLen)
            EditorGUILayout.HelpBox($"ALERTE : Le Dash dépasse la fin de l'animation !", MessageType.Error);

        if (hit.HitboxStartOffset + hit.HitboxActiveDuration > animLen)
            EditorGUILayout.HelpBox($"ERREUR : La Hitbox frappe dans le vide (après l'anim) !", MessageType.Error);
        
    }
}