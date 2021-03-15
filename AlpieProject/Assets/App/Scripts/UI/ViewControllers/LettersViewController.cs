using System.Collections;
using DynamicBox.Controllers;
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

		#region Unity Methods
		
		void OnEnable ()
		{
			StartCoroutine (OnEnabledDelayed ());
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
			currentLetterIndex = Random.Range (0, letters.Length);
			ShowLetter ();
		}

		private void ShowLetter ()
		{
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

		public void SetLetterImage (int index)
		{
			letterImage.sprite = letters[index];
		}

		public void SetBackgroundVideo (int index)
		{
			videoPlayer.Stop ();

			videoPlayer.clip = backgroundVideos[index];

			rawImage.SetActive (true);
			rightPanel.SetActive (true);
			videoPlayer.Play ();
		}
	}
}