using TMPro;
using UnityEngine;

namespace DynamicBox.UI.ViewControllers
{
	public class DialectsViewController : MonoBehaviour
	{
		[SerializeField] private TMP_Dropdown dialectDropdown;
		[SerializeField] private TextMeshProUGUI soundText;

		private string mute;

		#region Unity Methods

		void OnEnable ()
		{
			dialectDropdown.value = PlayerPrefs.GetInt ("DialectIndex");

			if (IsMuted ())
			{
				AudioListener.volume = 0;
				mute = "yes";
				soundText.text = "Sound: Off";
			}
			else
			{
				AudioListener.volume = 1;
				mute = "no";
				soundText.text = "Sound: On";
			}
		}

		void Start ()
		{
			dialectDropdown.onValueChanged.AddListener (delegate { DropdownValueChanged (dialectDropdown); });
		}

		#endregion

		private void DropdownValueChanged (TMP_Dropdown change)
		{
			Debug.Log ("Selected dialect : " + change.value);
			PlayerPrefs.SetInt ("DialectIndex", change.value);
		}

		public void ShowLetters ()
		{
			PlayerPrefs.SetInt ("LetterIndex", 0);
		}

		public void MuteSound ()
		{
			if (IsMuted ())
			{
				AudioListener.volume = 1;
				mute = "no";
				soundText.text = "Sound: On";
			}
			else
			{
				AudioListener.volume = 0;
				mute = "yes";
				soundText.text = "Sound: Off";
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