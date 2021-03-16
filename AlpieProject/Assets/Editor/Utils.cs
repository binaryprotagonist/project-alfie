using UnityEditor;
using UnityEngine;

public class Utils : Editor
{
	[MenuItem ("DynamicBox/Clear PlayerPrefs")]
	static void ClearPlayerPrefs ()
	{
		// Debug.Log ("PlayerPrefs cleared");
		PlayerPrefs.DeleteAll ();
	}

	[MenuItem ("DynamicBox/Open Save Folder")]
	static void OpenSaveFolder ()
	{
		EditorUtility.RevealInFinder (Application.persistentDataPath /*+ "/Bounce/"*/);
	}

	[MenuItem ("DynamicBox/Delete Save Data")]
	static void DeleteSomething ()
	{
		FileUtil.DeleteFileOrDirectory (Application.persistentDataPath + "/SaveData.json");
	}
}