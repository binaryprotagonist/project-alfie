using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
		[SerializeField] private AudioClip[] sounds;

		[SerializeField] private int currentLetterIndex;

		#region Unity Methods

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
			audioSource.PlayOneShot (sounds[currentLetterIndex]);
		}

		public void OpenGameScene ()
		{
			SceneManager.LoadScene ("App/Scenes/Game");
		}

		public void PlaySoundOnShowView ()
		{
			audioSource.PlayOneShot (sounds[currentLetterIndex]);
		}
	}
}