using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using Doozy.Engine.Soundy;
using Doozy.Engine.UI;
using DynamicBox.Controllers;
using DynamicBox.EventManagement;
using DynamicBox.EventManagement.GameEvents;
using DynamicBox.EventManagement.GameEvents.VoiceOver;
using DynamicBox.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace DynamicBox.UI.ViewControllers
{
	public class LettersViewController : MonoBehaviour
	{
		[Header ("Parameters")]
		[SerializeField] private Color[] letterFlagColors;

		[Space]
		[SerializeField] private AudioClip finishLetterSound;
		
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
		[SerializeField] private GameObject iapLockedPanel;

		[Space]
		[SerializeField] private GameObject rightPanel;
		[SerializeField] private Image letterBackground;
		[SerializeField] private Image letterImage;
		
		[Space]
		[SerializeField] private Button previousButton;
		[SerializeField] private Button nextButton;

		[Space]
		[SerializeField] private HapticFeedback hapticFeedback;
		[SerializeField] private ParticleSystem confettiParticlePrefab;

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
		public int CurrentLetterIndex => currentLetterIndex;

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
			// currentLetterIndex = 0;
			
			dayInputField.text = string.Empty;
			monthInputField.text = string.Empty;
			yearInputField.text = string.Empty;
			confirmAgeButton.interactable = false;
			
			// ShowLetter ();
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
			bool isUnlocked = PlayerPrefs.GetInt ("UnlockPurchased") != 0;
			if (currentLetterIndex == 3)
			{
				if (!isUnlocked)
				{
					Debug.Log ("All letters are not unlocked");
					agePanel.SetActive (true);
					
					// return;
				}
				else
				{
					Debug.Log ("All letters are unlocked");
				}
			}

			if (currentLetterIndex >= 3 && !isUnlocked)
			{
				iapLockedPanel.SetActive (true);
				
				HidePathsAsync ();
			}
			else
			{
				iapLockedPanel.SetActive (false);
			}

			letterImage.sprite = letters[currentLetterIndex];

			if (audioSource.isPlaying)
			{
				audioSource.Stop ();
			}

			audioSource.PlayOneShot (currentSounds[currentLetterIndex]);
			
			EventManager.Instance.Raise (new NewLetterSelectedEvent (true));

			// Check if selected current letter is finished or not
			bool isCompleted = saveController.CheckIfLetterFinished (currentLetterIndex);
			
			if (isCompleted)
			{
				SetBackgroundVideo (currentLetterIndex);
				gameManager.DestroyShape ();
			}
			else
			{
				ShowCurrentLetter ();
			}
		}

		private async Task HidePathsAsync ()
		{
			await Task.Yield ();
			
			GameObject.Find ("Paths").SetActive (false);
		}

		public void ShowCurrentLetter ()
		{
			rawImage.SetActive (false);
			rightPanel.SetActive (false);
			gameManager.CreateShape (currentLetterIndex);
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
			
			hapticFeedback.Vibrate ();
			Instantiate (confettiParticlePrefab, transform);
			
			rawImage.GetComponent<RawImage> ().color = Color.clear;
			rawImage.GetComponent<RawImage> ().DOColor (Color.white, 1);
		}

		private void SetLetterImage (int index)
		{
			letterImage.sprite = letters[index];
		}

		private void SetBackgroundVideo (int index)
		{
			videoPlayer.Stop ();

			videoPlayer.clip = backgroundVideos[index];
			letterBackground.color = letterFlagColors[index];

			rawImage.SetActive (true);
			rightPanel.SetActive (true);
			videoPlayer.Play ();

			SoundyManager.Play (finishLetterSound, Vector3.zero);
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