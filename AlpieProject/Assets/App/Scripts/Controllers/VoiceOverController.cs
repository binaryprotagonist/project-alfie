using System;
using System.Threading;
using System.Threading.Tasks;
using DynamicBox.EventManagement;
using DynamicBox.EventManagement.GameEvents;
using DynamicBox.EventManagement.GameEvents.VoiceOver;
using DynamicBox.ScriptableObjects;
using UnityEngine;

namespace DynamicBox.Controllers
{
	public class VoiceOverController : MonoBehaviour
	{
		[Header ("Links")] 
		[SerializeField] private AudioSource audioSource;

		[Header ("Data")] 
		[SerializeField] private DialectVoiceOverDatum[] dialectVoiceOverData;

		private int activeDialectIndex;

		private CancellationToken cancellationToken;

		private int dialectIndex;

		private bool isNewLetterSelected;

		#region Unity Methods

		void OnEnable ()
		{
			EventManager.Instance.AddListener<DialectSelectedEvent> (DialectSelectedHandler);
			EventManager.Instance.AddListener<LandingPageEnabledEvent> (LandingPageEnabledHandler);
			EventManager.Instance.AddListener<LettersPageEnabledEvent> (LettersPageEnabledEventHandler);
			EventManager.Instance.AddListener<LetterFinishedEvent> (LetterFinishedEventHandler);
			EventManager.Instance.AddListener<NewLetterSelectedEvent> (NewLetterSelectedEventHandler);
		}

		void OnDisable ()
		{
			EventManager.Instance.RemoveListener<DialectSelectedEvent> (DialectSelectedHandler);
			EventManager.Instance.RemoveListener<LandingPageEnabledEvent> (LandingPageEnabledHandler);
			EventManager.Instance.RemoveListener<LettersPageEnabledEvent> (LettersPageEnabledEventHandler);
			EventManager.Instance.RemoveListener<LetterFinishedEvent> (LetterFinishedEventHandler);
			EventManager.Instance.RemoveListener<NewLetterSelectedEvent> (NewLetterSelectedEventHandler);
		}

		async void Start ()
		{
			await Task.Delay (100, cancellationToken);
			EventManager.Instance.Raise (new LandingPageEnabledEvent ());
		}

		// void Update ()
		// {
		// 	if (Input.GetKey (KeyCode.A))
		// 	{
		// 		Debug.Log (PlayerPrefs.GetInt ("DialectIndex"));
		// 	}
		// }

		#endregion

		private async Task LandingPageEnabledAsync ()
		{
			try
			{
				dialectIndex = PlayerPrefs.GetInt ("DialectIndex");

				// Hi
				// Debug.Log ("0");
				AudioClip audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[0];
				PlayAudio (audioClip);
				
				await Task.Delay (3500, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
					return;

				// Let's play together
				// Debug.Log ("1");
				audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[1];
				PlayAudio (audioClip);
			}
			catch (Exception e)
			{
				Debug.Log ($"Exception: {e}");
			}
		}

		private async Task LettersPageEnabledAsync ()
		{
			Debug.Log ("LettersPageEnabledAsync");
			
			try
			{
				await Task.Delay (3000, cancellationToken);
				
				dialectIndex = PlayerPrefs.GetInt ("DialectIndex");

				// Let's pick a letter
				// Debug.Log ("2");
				AudioClip audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[2];
				PlayAudio (audioClip);

				await Task.Delay (3500, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
					return;

				// Let's trace a letter
				// Debug.Log ("3");
				audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[3];
				PlayAudio (audioClip);
			}
			catch (Exception e)
			{
				Debug.Log ($"Exception: {e}");
			}
		}

		private async Task LetterFinishedAsync ()
		{
			try
			{
				await Task.Delay (2000, cancellationToken);
				
				dialectIndex = PlayerPrefs.GetInt ("DialectIndex");

				// Bravo
				// Debug.Log ("5");
				AudioClip audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[5];
				PlayAudio (audioClip);
				
				await Task.Delay (2500, cancellationToken);

				// Let's try another letter
				// Debug.Log ("7");
				audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[7];
				PlayAudio (audioClip);
				
				await Task.Delay (10000, cancellationToken);
				
				if (cancellationToken.IsCancellationRequested)
					return;

				if (!isNewLetterSelected)
				{
					// Yalla
					// Debug.Log ("4");
					audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[4];
					PlayAudio (audioClip);
				}
			}
			catch (Exception e)
			{
				Debug.Log ($"Exception: {e}");
			}
		}

		private void PlayAudio (AudioClip audioClip)
		{
			audioSource.Stop ();
			audioSource.PlayOneShot (audioClip);
		}

		#region Event Handlers

		private void DialectSelectedHandler (DialectSelectedEvent eventDetails)
		{
			activeDialectIndex = eventDetails.ActiveDialectIndex;
		}

		private void LandingPageEnabledHandler (LandingPageEnabledEvent eventDetails)
		{
#pragma warning disable 4014

			LandingPageEnabledAsync ();

#pragma warning restore 4014
		}

		private void LettersPageEnabledEventHandler (LettersPageEnabledEvent eventDetails)
		{
#pragma warning disable 4014

			LettersPageEnabledAsync ();

#pragma warning restore 4014
		}
		
		private void LetterFinishedEventHandler (LetterFinishedEvent eventDetails)
		{
#pragma warning disable 4014

			LetterFinishedAsync ();

#pragma warning restore 4014
		}
		
		private void NewLetterSelectedEventHandler (NewLetterSelectedEvent eventDetails)
		{
			isNewLetterSelected = eventDetails.Value;
		}

		#endregion
	}
}