using System;
using DynamicBox.EventManagement;
using DynamicBox.EventManagement.GameEvents;
using DynamicBox.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace DynamicBox.Controllers
{
	public class IAPHandler : MonoBehaviour
	{
		[Header ("Parameters")] 
		[SerializeField] private IAPData macAppStoreIAPData;

		[SerializeField] private TextMeshProUGUI unlockButtonText;

		private const string kUnlockAllLetters = "net.dynamicbox.alpie.unlock_all_letters";

		private string resultInfo;

		#region Unity Methods

		void Start ()
		{
			unlockButtonText.text = $"{macAppStoreIAPData.IApData[0].Title} {macAppStoreIAPData.IApData[0].Price}$";
		}

		#endregion
		
		public void UnlockAllLetters ()
		{
			EventManager.Instance.Raise (new BuyIAPItemEvent (kUnlockAllLetters));
		}

		#region Callbacks

		public void OnInitialized (bool value)
		{
			if (value)
			{
				Debug.Log ("Initialize successful");
			}
			else
			{
				Debug.LogError ("Initialize failed");
			}
		}

		public void OnPurchaseSucceed (string productId)
		{
			switch (productId)
			{
				case kUnlockAllLetters:
					PlayerPrefs.SetInt ("UnlockPurchased", 1);
					resultInfo = "Unlock all letters purchase successful";
					Debug.Log (resultInfo);
					
					EventManager.Instance.Raise (new UnlockSuccessfulEvent());
					
					break;
				default:
					throw new NotImplementedException ();
			}
		}

		public void OnPurchaseFailed (string productId)
		{
			switch (productId)
			{
				case kUnlockAllLetters:
					resultInfo = "Unlock all letters purchase failed";
					Debug.Log (resultInfo);

					break;
				default:
					throw new NotImplementedException ();
			}
		}

		#endregion
	}
}