using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DynamicBox.UI.ViewControllers
{
	public class DialectsViewController : MonoBehaviour
	{
		[Header("Links")]
		[SerializeField] private TMP_Dropdown dialectDropdown;
		[SerializeField] private Image soundButtonImage;
		[SerializeField] private Sprite soundOnSprite;
		[SerializeField] private Sprite soundOffSprite;
		[SerializeField] private GameObject dropdownBGImage;

		private string mute;

		#region Unity Methods

		void OnEnable ()
		{
			dialectDropdown.value = PlayerPrefs.GetInt ("DialectIndex");

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

		void Start ()
		{
			dialectDropdown.onValueChanged.AddListener (delegate { DropdownValueChanged (dialectDropdown); });
		}

		void Update ()
		{
			if (Input.GetMouseButtonDown (0))
			{
				// Check if the mouse was clicked over a UI element
				if (EventSystem.current.IsPointerOverGameObject ())
				{
					// Debug.Log ("Clicked on the UI");
					// Debug.Log ("name = " + EventSystem.current.currentSelectedGameObject.name);
					if (EventSystem.current.currentSelectedGameObject.name.Equals ("Blocker"))
					{
						dropdownBGImage.SetActive(false);
					}
				}
			}
			
			HideIfClickedOutside (dialectDropdown.gameObject);
		}

		#endregion
		
		private void HideIfClickedOutside(GameObject panel) 
		{
			if (Input.GetMouseButton(0) && panel.activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(panel.GetComponent<RectTransform>(), Input.mousePosition, Camera.main)) 
			{
				dropdownBGImage.SetActive(false);
			}
		}

		private void DropdownValueChanged (TMP_Dropdown change)
		{
			Debug.Log ("Selected dialect : " + change.value);
			PlayerPrefs.SetInt ("DialectIndex", change.value);
			
			dropdownBGImage.SetActive (false);
			
			UIView.HideView ("Menu","Dialects");
			UIView.ShowView ("Menu","Letters");
		}

		public void ShowLetters ()
		{
			// PlayerPrefs.SetInt ("LetterIndex", 0);
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