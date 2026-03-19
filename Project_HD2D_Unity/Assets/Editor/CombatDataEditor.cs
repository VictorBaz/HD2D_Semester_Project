using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatData))]
public class CombatDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CombatData data = (CombatData)target;

        if (data.ComboHits == null) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Validation des Timings", EditorStyles.boldLabel);

        for (int i = 0; i < data.ComboHits.Length; i++)
        {
            var hit = data.ComboHits[i];
            
            if (hit.Clip == null)
            {
                EditorGUILayout.HelpBox($"Combo {i} : Pas d'AnimationClip assigné.", MessageType.Warning);
                continue;
            }

            float animLength = hit.Clip.length;
            float totalHitboxTime = hit.HitboxStartOffset + hit.HitboxActiveDuration;

            if (totalHitboxTime > animLength)
            {
                EditorGUILayout.HelpBox(
                    $"ERREUR Combo {i} : La hitbox finit à {totalHitboxTime}s, mais l'anim ne dure que {animLength}s !", 
                    MessageType.Error);
            }

            if (hit.DashDuration > animLength)
            {
                EditorGUILayout.HelpBox(
                    $"WARNING Combo {i} : Le dash dure plus longtemps ({hit.DashDuration}s) que l'animation ({animLength}s).", 
                    MessageType.Warning);
            }
            
            Rect rect = GUILayoutUtility.GetRect(10, 20);
            GUILayout.Space(5);
            DrawTimeline(rect, animLength, hit.HitboxStartOffset, hit.HitboxActiveDuration);
        }
    }

    private void DrawTimeline(Rect rect, float total, float start, float duration)
    {
        EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f)); 
        
        float widthStart = (start / total) * rect.width;
        float widthActive = (duration / total) * rect.width;
        
        Rect activeRect = new Rect(rect.x + widthStart, rect.y, widthActive, rect.height);
        
        Color barColor = (start + duration > total) ? Color.red : Color.green;
        EditorGUI.DrawRect(activeRect, barColor);
        
        EditorGUI.LabelField(rect, $"  Timeline: {total:F2}s", EditorStyles.miniLabel);
    }
}