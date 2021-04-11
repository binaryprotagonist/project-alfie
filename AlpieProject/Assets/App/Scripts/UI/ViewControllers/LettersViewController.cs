using System.Collections;
using Doozy.Engine.UI;
using DynamicBox.Controllers;
using DynamicBox.EventManagement;
using DynamicBox.EventManagement.GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace DynamicBox.UI.ViewControllers
{
	public class LettersViewController : MonoBehaviour
	{
		[Header ("Links")]
		[SerializeField] private SaveController saveController;
		[SerializeField] private DynamicBox.Managers.GameManager gameManager;
		[SerializeField] private GameObject agePanel;
		[SerializeField] private TMP_InputField dayInputField;
		[SerializeField] private TMP_InputField monthInputField;
		[SerializeField] private TMP_InputField yearInputField;
		[SerializeField] private Button confirmAgeButton;
		[SerializeField] private GameObject iapPanel;
		[SerializeField] private GameObject rawImage;
		[SerializeField] private GameObject rightPanel;
		[SerializeField] private Image letterImage;
		[SerializeField] private Button previousButton;
		[SerializeField] private Button nextButton;

		[Header ("Letter images")]
		[SerializeField] private Sprite[] letters;

		[Header ("Letter sounds")]
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private AudioClip[] egyptianSounds;
		[SerializeField] private AudioClip[] foshaSounds;
		[SerializeField] private AudioClip[] jordanianSounds;
		[SerializeField] private AudioClip[] lebaneseSounds;
		[SerializeField] private AudioClip[] currentSounds;

		[Header ("Letter videos")] 
		[SerializeField] private VideoPlayer videoPlayer;
		[SerializeField] private VideoClip[] backgroundVideos;

		[SerializeField] private int currentLetterIndex;

		private bool allowEnableConfirmAgeButton = true;

		#region Unity Methods

		void OnEnable ()
		{
			StartCoroutine (OnEnabledDelayed ());

			EventManager.Instance.AddListener<UnlockSuccessfulEvent> (UnlockSuccessfulEventHandler);
		}

		private void OnDisable ()
		{
			EventManager.Instance.RemoveListener<UnlockSuccessfulEvent> (UnlockSuccessfulEventHandler);
		}

		void Update ()
		{
			if (currentLetterIndex == 0)
			{
				previousButton.interactable = false;
			}
			else if (currentLetterIndex == letters.Length - 1)
			{
				nextButton.interactable = false;
			}
			else
			{
				previousButton.interactable = true;
				nextButton.interactable = true;
			}
			
			if (allowEnableConfirmAgeButton && dayInputField.text.Length > 0 && monthInputField.text.Length > 0 && yearInputField.text.Length > 3)
			{
				confirmAgeButton.interactable = true;
				
				allowEnableConfirmAgeButton = false;
			}
		}

		#endregion
		
		private IEnumerator OnEnabledDelayed ()
		{
			SetSounds ();
			
			yield return new WaitForSeconds (.2f);
			
			// refresh first unfinished letter index
			currentLetterIndex = saveController.FindFirstUnfinishedLetter ();
			
			ShowLetter ();
		}

		public void ToDialectsView ()
		{
			UIView.HideView ("Menu","Letters");
			UIView.ShowView ("Menu","Dialects");
		}

		public void CancelIAPDialog ()
		{
			iapPanel.SetActive (false);
			currentLetterIndex = 0;
			
			dayInputField.text = string.Empty;
			monthInputField.text = string.Empty;
			yearInputField.text = string.Empty;
			confirmAgeButton.interactable = false;
			
			ShowLetter ();
		}

		private void SetSounds ()
		{
			switch (PlayerPrefs.GetInt ("DialectIndex"))
			{
				case 0:
					currentSounds = egyptianSounds;
					break;
				case 1:
					currentSounds = foshaSounds;
					break;
				case 2:
					currentSounds = jordanianSounds;
					break;
				case 3:
					currentSounds = lebaneseSounds;
					break;
				default:
					Debug.LogError ("Unhandled case");
					break;
			}
		}

		public void ShowPreviousLetter ()
		{
			currentLetterIndex--;
			ShowLetter ();
		}

		public void ShowNextLetter ()
		{
			currentLetterIndex++;
			ShowLetter ();
		}

		public void ShowRandomLetter ()
		{
			bool isUnlockPurchased = PlayerPrefs.GetInt ("UnlockPurchased") == 1;
			int maxIndex = isUnlockPurchased ? letters.Length : 3;

			currentLetterIndex = Random.Range (0, maxIndex);
			ShowLetter ();
		}

		public void ConfirmAge ()
		{
			agePanel.SetActive (false);
			iapPanel.SetActive (true);
		}

		private void ShowLetter ()
		{
			if (currentLetterIndex == 3)
			{
				if (PlayerPrefs.GetInt ("UnlockPurchased") == 0)
				{
					Debug.Log ("All letters are not unlocked");
					agePanel.SetActive (true);
					return;
				}

				Debug.Log ("All letters are unlocked");
			}

			letterImage.sprite = letters[currentLetterIndex];

			if (audioSource.isPlaying)
			{
				audioSource.Stop ();
			}

			audioSource.PlayOneShot (currentSounds[currentLetterIndex]);

			// Check if selected current letter is finished or not
			bool isCompleted = saveController.CheckIfLetterFinished (currentLetterIndex);
			
			if (isCompleted)
			{
				SetBackgroundVideo (currentLetterIndex);
				gameManager.DestroyShape ();
			}
			else
			{
				rawImage.SetActive (false);
				rightPanel.SetActive (false);
				gameManager.CreateShape (currentLetterIndex);
			}
		}

		// Doozy Game Event
		public void PlaySoundOnShowView ()
		{
			currentLetterIndex = saveController.FirstUnfinishedLetterIndex;
		}

		public void SetLetterFinished (int index)
		{
			saveController.SetLetterFinished (index);
			SetLetterImage (index);
			SetBackgroundVideo (index);
		}

		private void SetLetterImage (int index)
		{
			letterImage.sprite = letters[index];
		}

		private void SetBackgroundVideo (int index)
		{
			videoPlayer.Stop ();

			videoPlayer.clip = backgroundVideos[index];

			rawImage.SetActive (true);
			rightPanel.SetActive (true);
			videoPlayer.Play ();
		}

		#region Event Handlers

		private void UnlockSuccessfulEventHandler (UnlockSuccessfulEvent eventDetails)
		{
			PlayerPrefs.SetInt ("UnlockPurchased", 1);
			iapPanel.SetActive (false);
			currentLetterIndex = 3;
			ShowLetter ();
		}

		#endregion
	}
}