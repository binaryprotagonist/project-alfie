using System;
using DynamicBox.EventManagement;
using DynamicBox.EventManagement.GameEvents;
using DynamicBox.ScriptableObjects;
using UnityEngine;

namespace DynamicBox.Controllers
{
	public class IAPHandler : MonoBehaviour
	{
		[Header ("Parameters")] 
		[SerializeField] private IAPData macAppStoreIAPData;

		private const string kUnlockAllLetters = "net.dynamicbox.alpieproject.unlock_all_letters";

		private string resultInfo;
		
		public void UnlockAllLetters ()
		{
			EventManager.Instance.Raise (new BuyIAPItemEvent (kUnlockAllLetters));
		}

		public void RestorePurchases ()
		{
			EventManager.Instance.Raise (new RestorePurchasesEvent ());
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