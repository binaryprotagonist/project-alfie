using System.Collections.Generic;
using Doozy.Engine.UI;
using DynamicBox.EventManagement;
using DynamicBox.EventManagement.GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicBox.UI.ViewControllers
{
	public class DialectsViewController : MonoBehaviour
	{
		[Header ("Links")] 
		[SerializeField] private Image soundButtonImage;
		[SerializeField] private Sprite soundOnSprite;
		[SerializeField] private Sprite soundOffSprite;
		[SerializeField] private TextMeshProUGUI languagePanelOpener;
		[SerializeField] private List<GameObject> languageButtons;

		private readonly List<string> languageButtonsTextList = new List<string> {"Egyptian", "Fosha", "Jordanian", "Lebanese"};
		private bool isLanguageButtonsShowing;

		private string mute;

		#region Unity Methods

		void OnEnable ()
		{
			languagePanelOpener.text = languageButtonsTextList[PlayerPrefs.GetInt ("DialectIndex")];

			if (IsMuted ())
			{
				AudioListener.volume = 0;
				mute = "yes";
				soundButtonImage.sprite = soundOffSprite;
			}
			else
			{
				AudioListener.volume = 1;
				mute = "no";
				soundButtonImage.sprite = soundOnSprite;
			}
		}

		#endregion

		public void LanguageButtonClicked ()
		{
			ShowLanguageButtons (!isLanguageButtonsShowing);

			isLanguageButtonsShowing = !isLanguageButtonsShowing;
		}

		public void SelectLanguage (int index)
		{
			isLanguageButtonsShowing = false;

			languagePanelOpener.text = languageButtonsTextList[index];

			ShowLanguageButtons (false);

			Debug.Log ("Selected dialect index = " + index);
			PlayerPrefs.SetInt ("DialectIndex", index);
			
			EventManager.Instance.Raise (new DialectSelectedEvent (index));
		}

		public void PlayUsingCurrentDialect ()
		{
			UIView.HideView ("Menu","Dialects");
			UIView.ShowView ("Menu","Letters");
		}

		private void ShowLanguageButtons (bool value)
		{
			foreach (GameObject languageButton in languageButtons)
			{
				languageButton.SetActive (value);
			}
		}

		public void MuteSound ()
		{
			if (IsMuted ())
			{
				AudioListener.volume = 1;
				mute = "no";
				soundButtonImage.sprite = soundOnSprite;
			}
			else
			{
				AudioListener.volume = 0;
				mute = "yes";
				soundButtonImage.sprite = soundOffSprite;
			}

			PlayerPrefs.SetString ("IsMuted", mute);
		}

		private bool IsMuted ()
		{
			mute = PlayerPrefs.GetString ("IsMuted");
			return mute.Equals ("yes");
		}
	}
}