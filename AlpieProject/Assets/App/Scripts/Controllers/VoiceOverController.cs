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
		
		#region Unity Methods

		void OnEnable ()
		{
			EventManager.Instance.AddListener<DialectSelectedEvent> (DialectSelectedHandler);
			
			EventManager.Instance.AddListener<LandingPageEnabledEvent> (LandingPageEnabledHandler);
		}

		void OnDisable ()
		{
			EventManager.Instance.RemoveListener<DialectSelectedEvent> (DialectSelectedHandler);
			
			EventManager.Instance.RemoveListener<LandingPageEnabledEvent> (LandingPageEnabledHandler);
		}

		#endregion

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

		#endregion

		private async Task LandingPageEnabledAsync ()
		{
			try
			{
				AudioClip audioClip = dialectVoiceOverData[activeDialectIndex].VoiceOvers[0];
				audioSource.PlayOneShot (audioClip);

				await Task.Delay (3000, cancellationToken);
			
				if (cancellationToken.IsCancellationRequested)
					return;
			
				audioClip = dialectVoiceOverData[activeDialectIndex].VoiceOvers[1];
				audioSource.PlayOneShot (audioClip);
			}
			catch (Exception e)
			{
				Debug.Log ($"Exception: {e}");
			}
		}
	}
}