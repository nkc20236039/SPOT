using System.Collections;
using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class AutoSave
{
	public static readonly string manualSaveKey = "autosave@manualSave";

	static double nextTime = 0;
	static bool isChangedHierarchy = false;

	static AutoSave ()
	{
		IsManualSave = true;
		EditorApplication.playmodeStateChanged += () =>
		{
			if ( IsAutoSave && !EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) {

				IsManualSave = false;

				if (IsSavePrefab)
					AssetDatabase.SaveAssets ();
				if (IsSaveScene) {
					Debug.Log ("シーンを保存しました " + System.DateTime.Now);
					EditorApplication.SaveScene ();
				}
				IsManualSave = true;
			}
			isChangedHierarchy = false;
		};

		nextTime = EditorApplication.timeSinceStartup + Interval;
		EditorApplication.update += () =>
		{
			if (isChangedHierarchy && nextTime < EditorApplication.timeSinceStartup) {
				nextTime = EditorApplication.timeSinceStartup + Interval;

				IsManualSave = false;

				if (IsSaveSceneTimer && IsAutoSave && !EditorApplication.isPlaying) {
					if (IsSavePrefab)
						AssetDatabase.SaveAssets ();
					if (IsSaveScene) {
						Debug.Log ("シーンを保存しました " + System.DateTime.Now);
						EditorApplication.SaveScene ();
					}
				}
				isChangedHierarchy = false;
				IsManualSave = true;
			}
		};

		EditorApplication.hierarchyWindowChanged += ()=>
		{
			if(! EditorApplication.isPlaying)
				isChangedHierarchy = true;
		};
	}

	public static bool IsManualSave {
		get {
			return EditorPrefs.GetBool (manualSaveKey);
		}
		private set {
			EditorPrefs.SetBool (manualSaveKey, value);
		}
	}


	private static readonly string autoSave = "auto save";
	static bool IsAutoSave {
		get {
			string value = EditorUserSettings.GetConfigValue (autoSave);
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			return!string.IsNullOrEmpty (value) && value.Equals ("True");
		}
		set {
			EditorUserSettings.SetConfigValue (autoSave, value.ToString ());
		}
	}

	private static readonly string autoSavePrefab = "auto save prefab";
	static bool IsSavePrefab {
		get {
			string value = EditorUserSettings.GetConfigValue (autoSavePrefab);
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            return !string.IsNullOrEmpty (value) && value.Equals ("True");
		}
		set {
			EditorUserSettings.SetConfigValue (autoSavePrefab, value.ToString ());
		}
	}

	private static readonly string autoSaveScene = "auto save scene";
	static bool IsSaveScene {
		get {
			string value = EditorUserSettings.GetConfigValue (autoSaveScene);
			return!string.IsNullOrEmpty (value) && value.Equals ("True");
		}
		set {
			EditorUserSettings.SetConfigValue (autoSaveScene, value.ToString ());
		}
	}

	private static readonly string autoSaveSceneTimer = "auto save scene timer";
	static bool IsSaveSceneTimer {
		get {
			string value = EditorUserSettings.GetConfigValue (autoSaveSceneTimer);
			return!string.IsNullOrEmpty (value) && value.Equals ("True");
		}
		set {
			EditorUserSettings.SetConfigValue (autoSaveSceneTimer, value.ToString ());
		}
	}

    private static readonly string autoSaveTimeInterval = "savetimeIndex";
    static int TimeIndex
    {
        get
        {
            string value = EditorUserSettings.GetConfigValue(autoSaveTimeInterval);
            if (value == null)
            {
                value = "2";
            }
            return int.Parse(value);
        }
        set
        {
            EditorUserSettings.SetConfigValue(autoSaveTimeInterval, value.ToString());
        }
    }


    private static readonly string autoSaveInterval = "save scene interval";
	static int Interval {
		get {

			string value = EditorUserSettings.GetConfigValue (autoSaveInterval);
			if (value == null) {
				value = (60 * 15).ToString();
			}
			return int.Parse (value);
		}
		set {
			EditorUserSettings.SetConfigValue (autoSaveInterval, value.ToString ());
		}
	}

	static string[] TimeArrayText = { "1min", "5min", "15min", "30min", "60min" };
	static int[] TimeArray = { 60 * 1, 60 * 5, 60 * 15, 60 * 30, 60 * 60 };

    /// <summary>
    /// 暗めの白
    /// </summary>
    static Color LiteWhite = new Color(0.7686f, 0.7686f, 0.7686f, 1.0f);
    /// <summary>
    /// 明るめの黒
    /// </summary>
    static Color LiteBlack = new Color(0.007843138f, 0.007843138f, 0.007843138f, 1.0f);

    [PreferenceItem("Auto Save")] 
	static void ExampleOnGUI ()
	{
		bool isAutoSave = EditorGUILayout.BeginToggleGroup ("auto save", IsAutoSave);

		IsAutoSave = isAutoSave;
		EditorGUILayout.Space ();

		IsSavePrefab = EditorGUILayout.ToggleLeft ("save prefab", IsSavePrefab);
		IsSaveScene = EditorGUILayout.ToggleLeft ("save scene", IsSaveScene);
        IsSaveSceneTimer = EditorGUILayout.BeginToggleGroup("save scene interval", IsSaveSceneTimer); //時間
        EditorGUI.BeginChangeCheck();
		TimeIndex = EditorGUILayout.Popup(TimeIndex, TimeArrayText);
        if (EditorGUI.EndChangeCheck())
		{
			Interval = TimeArray[TimeIndex];
			Debug.Log($"{Interval / 60}分起きに設定しました。");
        }

		EditorGUILayout.EndToggleGroup ();
		EditorGUILayout.EndToggleGroup ();

        EditorGUILayout.Space(10);
        GUILayout.FlexibleSpace();
        GUIStyle linkLabel = new GUIStyle();

		linkLabel.alignment = TextAnchor.MiddleRight;
        linkLabel.normal.textColor = EditorGUIUtility.isProSkin ? LiteWhite : LiteBlack;
        linkLabel.richText = true;

        GUILayout.Label("Creator Twitter", linkLabel); //リンクラベル
        Rect rect3 = GUILayoutUtility.GetLastRect();
        EditorGUIUtility.AddCursorRect(rect3, MouseCursor.Link);
        Event nowEvent = Event.current;
        if (nowEvent.type == EventType.MouseDown && rect3.Contains(nowEvent.mousePosition))
        {
            Help.BrowseURL("https://twitter.com/71s9");
        }

    }

	[MenuItem("File/Backup/Backup")]
	public static void Backup ()
	{
		string expoertPath = "Backup/" + EditorApplication.currentScene;

		Directory.CreateDirectory (Path.GetDirectoryName (expoertPath));

		if( string.IsNullOrEmpty(EditorApplication.currentScene))
			return;

		byte[] data = File.ReadAllBytes (EditorApplication.currentScene);
		File.WriteAllBytes (expoertPath, data);
	}

	[MenuItem("File/Backup/Rollback")]
	public static void RollBack ()
	{
		string expoertPath = "Backup/" + EditorApplication.currentScene;
		
		byte[] data = File.ReadAllBytes (expoertPath);
		File.WriteAllBytes (EditorApplication.currentScene, data);
		AssetDatabase.Refresh (ImportAssetOptions.Default);
	}

}