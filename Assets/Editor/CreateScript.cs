///////////////////////////////////
/// Press 'Ctrl + Alt + N' 
/// to create new C# script file
/// in selected path.
/// Then hit 'Enter'
///////////////////////////////////

using UnityEditor;
using UnityEngine;

public class CreateScript: EditorWindow
{

	[MenuItem("Component/Scripts/New Script %&n")]
	public static void ShowWindow()
	{
		GetWindow(typeof(CreateScript));
	}

	private bool autoFocus = true;

	private string scriptName = "NewScript";


	private void OnGUI()
	{
		GUILayout.Label("Custom Script Creator", EditorStyles.boldLabel);

		EditorGUI.BeginChangeCheck();

		GUI.SetNextControlName(scriptName);

		scriptName = EditorGUILayout.TextField("Script Name", scriptName);

		if (autoFocus)
		{
			EditorGUI.FocusTextInControl("NewScript");
			autoFocus = false; // Disable autofocus after the first frame
		}

		if (GUILayout.Button("Create Script") || Event.current.keyCode == KeyCode.Return)
		{
			EditorGUI.EndChangeCheck();
			CreateNewScript();
		}
	}

	private void CreateNewScript()
	{
		// Get the currently selected folder in the Unity Editor
		string selectedFolder = "Assets/Scripts";
		Object selectedObject = Selection.activeObject;
		if (selectedObject != null)
		{
			string selectedPath = "Assets/Scripts";
			if (!string.IsNullOrEmpty(selectedPath))
			{
				selectedFolder = selectedPath;
				if (!System.IO.Directory.Exists(selectedPath))
				{
					selectedFolder = System.IO.Path.GetDirectoryName(selectedPath);
				}
			}
		}

		string scriptPath = System.IO.Path.Combine("Assets/Scripts", scriptName + ".cs");

		// Create the script file with a default template
		string template = "using UnityEngine;\n\npublic class {0} : MonoBehaviour\n{{\n\t// Start is called before the first frame update\n\tvoid Start()\n\t{{\n\n\t}}\n\n\t// Update is called once per frame\n\tvoid Update()\n\t{{\n\n\t}}\n}}";
		string scriptContent = string.Format(template, scriptName.Replace(" ", ""), scriptName);

		System.IO.File.WriteAllText(scriptPath, scriptContent);
		AssetDatabase.Refresh();

		// Select the newly created script in the Unity Editor
		Object createdScript = AssetDatabase.LoadAssetAtPath(scriptPath, typeof(Object));
		Selection.activeObject = createdScript;
		EditorGUIUtility.PingObject(createdScript);

		Close();
	}

}
