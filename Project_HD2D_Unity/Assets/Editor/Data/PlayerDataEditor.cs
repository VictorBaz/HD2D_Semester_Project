using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerData))]
public class PlayerDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); 

        PlayerData data = (PlayerData)target;
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("VISUALISATION DES TIMINGS", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        DrawParryTimeline(data.Combat);

        EditorGUILayout.Space(10);

        DrawComboTimelines(data.Combat);
    }

    private void DrawParryTimeline(CombatSettings combat)
    {
        if (combat.ParryAnimationClip == null) return;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        float animLen = combat.ParryAnimationClip.length;
        EditorGUILayout.LabelField($"PARADE : {combat.ParryAnimationClip.name}", EditorStyles.miniBoldLabel);
    
        DrawTimelineRow("Normale", animLen, combat.ParryHitboxStartOffset, combat.ParryActiveDuration, new Color(1f, 0.8f, 0f));
    
        DrawTimelineRow("Parfaite", animLen, combat.PerfectParryStartOffset, combat.PerfectParryDuration, Color.white);
    
        if (combat.PerfectParryStartOffset < combat.ParryHitboxStartOffset || 
            (combat.PerfectParryStartOffset + combat.PerfectParryDuration) > (combat.ParryHitboxStartOffset + combat.ParryActiveDuration))
        {
            EditorGUILayout.HelpBox("Attention : La fenêtre parfaite est hors des limites de la parade normale !", MessageType.Warning);
        }
    
        EditorGUILayout.EndVertical();
    }

    private void DrawComboTimelines(CombatSettings combat)
    {
        if (combat.ComboHits == null || combat.ComboHits.Length == 0)
        {
            EditorGUILayout.HelpBox("Combos : La liste est vide.", MessageType.None);
            return;
        }

        for (int i = 0; i < combat.ComboHits.Length; i++)
        {
            var hit = combat.ComboHits[i];
            if (hit.Clip == null) continue;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            float animLen = hit.Clip.length;
            EditorGUILayout.LabelField($"COMBO {i} : {hit.Clip.name} ({animLen:F2}s)", EditorStyles.miniBoldLabel);

            DrawTimelineRow("Dash", animLen, hit.DashStartOffset, hit.DashDuration, Color.cyan);
            DrawTimelineRow("Hitbox", animLen, hit.HitboxStartOffset, hit.HitboxActiveDuration, Color.green);

            if (hit.DashStartOffset + hit.DashDuration > animLen)
                EditorGUILayout.HelpBox("Dash trop long !", MessageType.Warning);
            if (hit.HitboxStartOffset + hit.HitboxActiveDuration > animLen)
                EditorGUILayout.HelpBox("Hitbox hors animation !", MessageType.Error);

            EditorGUILayout.EndVertical();
            GUILayout.Space(2);
        }
    }

    private void DrawTimelineRow(string label, float total, float start, float duration, Color barColor)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        
        Rect labelRect = new Rect(rect.x, rect.y, rect.width * 0.15f, rect.height);
        EditorGUI.LabelField(labelRect, label, EditorStyles.miniLabel);

        Rect barAreaRect = new Rect(rect.x + rect.width * 0.15f, rect.y, rect.width * 0.85f, rect.height);
        EditorGUI.DrawRect(barAreaRect, new Color(0.1f, 0.1f, 0.1f)); 

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
        
        string timingText = $"{start:F2}s -> {(start + duration):F2}s";
        EditorGUI.LabelField(barAreaRect, "  " + timingText, EditorStyles.whiteMiniLabel);
    }
}