using UnityEngine;
using System.Collections;
using IndieStd;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com
/// Escape or Back event
public class EscapeEvent : MonoBehaviour
{
	/// <summary>
	/// The name of the scene to be loaded.
	/// </summary>
	public string sceneName;

	/// <summary>
	/// Whether to leave the application on escape click.
	/// </summary>
	public bool leaveTheApplication;

	/// <summary>
	/// Whether to load the name of the scene in the shapes manager or not.
	/// </summary>
	public bool loadShapesManagerSceneName;

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			OnEscapeClick ();
		}
	}

	/// <summary>
	/// On Escape click event.
	/// </summary>
	public void OnEscapeClick ()
	{
		if (leaveTheApplication)
		{
			GameObject exitConfirmDialog = GameObject.Find ("ExitConfirmDialog");
			if (exitConfirmDialog != null)
			{
				Dialog exitDialogComponent = exitConfirmDialog.GetComponent<Dialog> ();
				if (!exitDialogComponent.animator.GetBool ("On"))
				{
					exitDialogComponent.Show ();
					//AdsManager.instance.ShowAdvertisment (AdsManager.AdAPI.AdEvent.Event.ON_SHOW_EXIT_DIALOG);
				}
			}
		}
		else
		{
			if (loadShapesManagerSceneName)
			{
				StartCoroutine (SceneLoader.LoadSceneAsync (GameObject.Find (ShapesManager.shapesManagerReference).GetComponent<ShapesManager> ().sceneName));
			}
			else
			{
				StartCoroutine (SceneLoader.LoadSceneAsync (sceneName));
			}
		}
	}
}