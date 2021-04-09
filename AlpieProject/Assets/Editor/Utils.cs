using UnityEditor;
using UnityEngine;

public class Utils : Editor
{
	[MenuItem ("DynamicBox/Delete all data")]
	static void DeleteAllData ()
	{
		Debug.Log ("Deleted all data");
		ClearPlayerPrefs ();
		DeleteSaveDataFile ();
	}
	
	[MenuItem ("DynamicBox/Clear PlayerPrefs")]
	static void ClearPlayerPrefs ()
	{
		PlayerPrefs.DeleteAll ();
	}

	[MenuItem ("DynamicBox/Delete Save Data")]
	static void DeleteSaveDataFile ()
	{
		FileUtil.DeleteFileOrDirectory (Application.persistentDataPath + "/SaveData.json");
	}
	
	[MenuItem ("DynamicBox/Open Save Folder")]
	static void OpenSaveFolder ()
	{
		EditorUtility.RevealInFinder (Application.persistentDataPath);
	}

	
}