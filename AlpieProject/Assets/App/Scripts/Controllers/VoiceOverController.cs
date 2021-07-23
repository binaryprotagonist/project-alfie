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

				AudioClip audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[0];
				audioSource.PlayOneShot (audioClip);

				await Task.Delay (3000, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
					return;

				audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[1];
				audioSource.PlayOneShot (audioClip);
			}
			catch (Exception e)
			{
				Debug.Log ($"Exception: {e}");
			}
		}

		private async Task LettersPageEnabledAsync ()
		{
			try
			{
				await Task.Delay (3000, cancellationToken);
				
				dialectIndex = PlayerPrefs.GetInt ("DialectIndex");

				AudioClip audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[2];
				audioSource.PlayOneShot (audioClip);

				await Task.Delay (3000, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
					return;

				audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[3];
				audioSource.PlayOneShot (audioClip);
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

				AudioClip audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[5];
				audioSource.PlayOneShot (audioClip);
				
				await Task.Delay (2000, cancellationToken);

				audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[7];
				audioSource.PlayOneShot (audioClip);
				
				await Task.Delay (10000, cancellationToken);
				
				if (cancellationToken.IsCancellationRequested)
					return;

				if (!isNewLetterSelected)
				{
					audioClip = dialectVoiceOverData[dialectIndex].VoiceOvers[4];
					audioSource.PlayOneShot (audioClip);
				}
			}
			catch (Exception e)
			{
				Debug.Log ($"Exception: {e}");
			}
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