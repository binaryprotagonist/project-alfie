using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace DynamicBox.UI.ViewControllers
{
	public class LettersViewController : MonoBehaviour
	{
		[Header ("Links")] 
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

		[Header("Letter videos")]
		[SerializeField] private VideoPlayer videoPlayer;
		[SerializeField] private VideoClip[] backgroundVideos;

		[SerializeField] private int currentLetterIndex;

		#region Unity Methods

		void Start ()
		{
			SetBackgroundVideo ();
		}

		void OnEnable ()
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
			PlayerPrefs.SetInt ("LetterIndex", currentLetterIndex);
			letterImage.sprite = letters[currentLetterIndex];

			if (audioSource.isPlaying)
			{
				audioSource.Stop ();
			}
			audioSource.PlayOneShot (currentSounds[currentLetterIndex]);

			SetBackgroundVideo ();
		}

		// Doozy => Game Event
		public void PlaySoundOnShowView ()
		{
			// Debug.Log ("PlaySoundOnShowView");
			audioSource.PlayOneShot (currentSounds[currentLetterIndex]);
		}

		public void SetBackgroundVideo ()
		{
			videoPlayer.Play ();
			videoPlayer.clip = backgroundVideos[currentLetterIndex];
			videoPlayer.isLooping = true;
		}

	}
}