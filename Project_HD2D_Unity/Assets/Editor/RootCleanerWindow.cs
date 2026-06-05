using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RootCleanerWindow : EditorWindow
{

    private struct CleanReport
    {
        public Root   root;
        public string rootName;
        public int    duplicateContainers;
        public int    orphanBranches;
        public bool   wasAlreadyClean;
    }

    private struct DeepCleanReport
    {
        public string goName;
        public string parentName;
        public string fullPath;
    }

    private List<Root>            foundRoots      = new List<Root>();
    private List<CleanReport>     lastReport      = new List<CleanReport>();
    private List<DeepCleanReport> deepCleanReport = new List<DeepCleanReport>();
    private Vector2               scrollRoots;
    private Vector2               scrollReport;
    private Vector2               scrollDeep;
    private bool                  hasScanned      = false;
    private bool                  hasRun          = false;
    private bool                  hasDeepRun      = false;


    private GUIStyle styleTitle;
    private GUIStyle styleSection;
    private GUIStyle styleBold;
    private GUIStyle styleClean;
    private GUIStyle styleDirty;
    private GUIStyle styleRed;
    private bool     stylesInitialized;


    [MenuItem("Tools/Root Cleaner")]
    public static void Open()
    {
        RootCleanerWindow win = GetWindow<RootCleanerWindow>("Root Cleaner");
        win.minSize = new Vector2(460, 560);
    }


    private void OnGUI()
    {
        InitStyles();

        DrawHeader();
        EditorGUILayout.Space(6);

        DrawScanSection();
        EditorGUILayout.Space(4);

        if (hasScanned)
        {
            DrawFoundRoots();
            EditorGUILayout.Space(4);
            DrawActionButtons();
            EditorGUILayout.Space(4);
        }

        if (hasRun)
            DrawReport();

        if (hasDeepRun)
            DrawDeepReport();
    }


    private void DrawHeader()
    {
        Rect headerRect = EditorGUILayout.GetControlRect(false, 38);
        EditorGUI.DrawRect(headerRect, new Color(0.12f, 0.12f, 0.12f));
        GUI.Label(new Rect(headerRect.x + 10, headerRect.y + 8, headerRect.width, 26),
            "ROOT CLEANER", styleTitle);
    }


    private void DrawScanSection()
    {
        GUI.backgroundColor = new Color(0.3f, 0.6f, 1f);
        if (GUILayout.Button("SCAN SCENE", GUILayout.Height(30)))
            ScanScene();
        GUI.backgroundColor = Color.white;

        if (hasScanned)
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField($"Found : {foundRoots.Count} Root(s)", styleBold);
        }
    }


    private void DrawFoundRoots()
    {
        EditorGUILayout.LabelField("ROOTS IN SCENE", styleSection);
        DrawSeparator();

        if (foundRoots.Count == 0)
        {
            EditorGUILayout.HelpBox("No Root component found in the scene.", MessageType.Warning);
            return;
        }

        scrollRoots = EditorGUILayout.BeginScrollView(scrollRoots,
            GUILayout.MaxHeight(Mathf.Min(foundRoots.Count * 22f + 8f, 130f)));

        foreach (Root root in foundRoots)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
            if (GUILayout.Button("►", GUILayout.Width(22), GUILayout.Height(18)))
            {
                Selection.activeGameObject = root.gameObject;
                EditorGUIUtility.PingObject(root.gameObject);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField(root.name, GUILayout.Height(18));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }


    private void DrawActionButtons()
    {
        if (foundRoots.Count == 0) return;

        EditorGUILayout.LabelField("ACTIONS", styleSection);
        DrawSeparator();

        GUI.backgroundColor = new Color(1f, 0.85f, 0f);
        if (GUILayout.Button("CLEAN ALL DUPLICATES", GUILayout.Height(34)))
            RunCleanAll();

        EditorGUILayout.Space(4);

        GUI.backgroundColor = new Color(1f, 0.35f, 0.35f);
        if (GUILayout.Button("DEEP CLEAN  —  Toutes les Branch_ sans parent valide", GUILayout.Height(34)))
        {
            if (EditorUtility.DisplayDialog(
                "Deep Clean",
                "Supprime TOUS les GameObjects dont le nom commence par 'Branch_' " +
                "et dont le parent ne s'appelle pas 'Generated_Visual_Branches'.\n\nContinuer ?",
                "Supprimer", "Annuler"))
            {
                RunDeepClean();
            }
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.Space(2);

        EditorGUILayout.HelpBox(
            "CLEAN ALL DUPLICATES — supprime les containers en doublon et les Branch_ orphelins directs.\n" +
            "DEEP CLEAN — scan toute la scène, supprime tout Branch_ dont le parent n'est pas 'Generated_Visual_Branches'.",
            MessageType.Info);
    }


    private void DrawReport()
    {
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("RAPPORT — CLEAN ALL", styleSection);
        DrawSeparator();

        int totalCleaned = 0;
        foreach (var r in lastReport)
            if (!r.wasAlreadyClean) totalCleaned++;

        string summary = totalCleaned == 0
            ? "✓  Tout était déjà propre."
            : $"✓  {totalCleaned} Root(s) nettoyé(s).";

        EditorGUILayout.LabelField(summary, styleBold);
        EditorGUILayout.Space(4);

        scrollReport = EditorGUILayout.BeginScrollView(scrollReport,
            GUILayout.MaxHeight(160f));

        foreach (var report in lastReport)
        {
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = new Color(0.85f, 0.85f, 0.85f);
            if (report.root != null &&
                GUILayout.Button("►", GUILayout.Width(22), GUILayout.Height(18)))
            {
                Selection.activeGameObject = report.root.gameObject;
                EditorGUIUtility.PingObject(report.root.gameObject);
            }
            GUI.backgroundColor = Color.white;

            if (report.wasAlreadyClean)
            {
                EditorGUILayout.LabelField(
                    $"{report.rootName}  —  déjà propre ✓", styleClean);
            }
            else
            {
                EditorGUILayout.LabelField(
                    $"{report.rootName}  —  " +
                    $"{report.duplicateContainers} container(s), " +
                    $"{report.orphanBranches} branche(s) supprimé(s)",
                    styleDirty);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }


    private void DrawDeepReport()
    {
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("RAPPORT — DEEP CLEAN", styleSection);
        DrawSeparator();

        if (deepCleanReport.Count == 0)
        {
            EditorGUILayout.LabelField("✓  Aucune Branch_ invalide trouvée.", styleBold);
            return;
        }

        EditorGUILayout.LabelField(
            $"⚠  {deepCleanReport.Count} Branch_ supprimée(s) :", styleBold);
        EditorGUILayout.Space(4);

        scrollDeep = EditorGUILayout.BeginScrollView(scrollDeep,
            GUILayout.MaxHeight(160f));

        foreach (var entry in deepCleanReport)
        {
            EditorGUILayout.LabelField(
                $"✕  {entry.goName}  (parent : '{entry.parentName}')  [{entry.fullPath}]",
                styleRed);
        }

        EditorGUILayout.EndScrollView();
    }


    private void ScanScene()
    {
        foundRoots.Clear();
        lastReport.Clear();
        deepCleanReport.Clear();
        hasRun     = false;
        hasDeepRun = false;
        hasScanned = true;

        Root[] roots = FindObjectsByType<Root>(FindObjectsSortMode.None);
        foundRoots.AddRange(roots);

        Repaint();
    }


    private void RunCleanAll()
    {
        lastReport.Clear();
        hasRun     = true;
        hasDeepRun = false;

        foreach (Root root in foundRoots)
        {
            if (root == null) continue;

            CleanReport report = BuildReport(root);

            Undo.RecordObject(root, "Clean Root Duplicates");
            root.CleanDuplicates();
            EditorUtility.SetDirty(root);

            lastReport.Add(report);
        }

        Repaint();
    }


    private void RunDeepClean()
    {
        deepCleanReport.Clear();
        hasDeepRun = true;
        hasRun     = false;

        GameObject[] allObjects = FindObjectsByType<GameObject>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        List<GameObject> toDelete = new List<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (!go.name.StartsWith("Branch_")) continue;

            string parentName = go.transform.parent != null
                ? go.transform.parent.name
                : "(aucun parent)";

            if (parentName != "Generated_Visual_Branches")
            {
                deepCleanReport.Add(new DeepCleanReport
                {
                    goName   = go.name,
                    parentName = parentName,
                    fullPath = GetFullPath(go.transform)
                });
                toDelete.Add(go);
            }
        }
        
        foreach (GameObject go in toDelete)
        {
            if (go == null) continue;
            Undo.DestroyObjectImmediate(go);
        }

        if (toDelete.Count > 0)
            EditorUtility.DisplayDialog(
                "Deep Clean terminé",
                $"{toDelete.Count} Branch_ invalide(s) supprimée(s).\nVoir le rapport dans la fenêtre.",
                "OK");

        Repaint();
    }


    private CleanReport BuildReport(Root root)
    {
        CleanReport report = new CleanReport
        {
            root     = root,
            rootName = root.name,
        };

        ArrayCurveSplineMesh splineScript =
            root.GetComponentInChildren<ArrayCurveSplineMesh>(true);

        if (splineScript == null)
        {
            report.wasAlreadyClean = true;
            return report;
        }

        string containerName = splineScript.SubContainerName;
        if (string.IsNullOrEmpty(containerName))
            containerName = "Generated_Visual_Branches";

        Transform splineT    = splineScript.transform;
        bool      firstFound = false;

        for (int i = 0; i < splineT.childCount; i++)
        {
            Transform child = splineT.GetChild(i);
            if (child.name != containerName) continue;

            if (!firstFound) firstFound = true;
            else             report.duplicateContainers++;
        }

        ScanOrphanBranches(root.transform, containerName, ref report);

        report.wasAlreadyClean =
            report.duplicateContainers == 0 && report.orphanBranches == 0;

        return report;
    }

    private void ScanOrphanBranches(Transform current, string containerName,
                                    ref CleanReport report)
    {
        for (int i = 0; i < current.childCount; i++)
        {
            Transform child = current.GetChild(i);

            if (child.name == containerName)
                continue;

            if (child.name.StartsWith("Branch_"))
            {
                report.orphanBranches++;
                continue;
            }

            ScanOrphanBranches(child, containerName, ref report);
        }
    }


    private static string GetFullPath(Transform t)
    {
        if (t.parent == null) return t.name;
        return GetFullPath(t.parent) + "/" + t.name;
    }

    private void DrawSeparator()
    {
        Rect r = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(r, new Color(0.35f, 0.35f, 0.35f));
        EditorGUILayout.Space(2);
    }

    private void InitStyles()
    {
        if (stylesInitialized) return;

        styleTitle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize  = 15,
            normal    = { textColor = new Color(0.9f, 0.85f, 0.2f) },
            alignment = TextAnchor.MiddleLeft
        };

        styleSection = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 10,
            normal   = { textColor = new Color(0.55f, 0.55f, 0.55f) }
        };

        styleBold = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            normal    = { textColor = new Color(0.85f, 0.85f, 0.85f) }
        };

        styleClean = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = new Color(0.4f, 0.85f, 0.4f) }
        };

        styleDirty = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            normal    = { textColor = new Color(1f, 0.75f, 0.2f) }
        };

        styleRed = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = new Color(1f, 0.4f, 0.4f) }
        };

        stylesInitialized = true;
    }
}