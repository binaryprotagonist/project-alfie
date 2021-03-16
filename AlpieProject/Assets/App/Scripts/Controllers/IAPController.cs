using DynamicBox.EventManagement;
using DynamicBox.EventManagement.GameEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using DynamicBox.ScriptableObjects;

namespace DynamicBox.Controllers
{
	public class IAPController : MonoBehaviour, IStoreListener
	{
		[Header ("Parameters")]
		[SerializeField] private IAPData macAppStoreIAPData;

		[Space] 
		[SerializeField] private bool isDebugOn = true;

		[Header ("Callbacks")] 
		[SerializeField] private UnityEvent<bool> OnInitialize;
		[SerializeField] private UnityEvent<string> OnPurchaseSucceed;
		[SerializeField] private UnityEvent<string> OnPurchaseFail;

		private static IStoreController m_StoreController;          // The Unity Purchasing system.
		private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

		private int m_SelectedItemIndex = -1; // -1 == no product    

		#region Unity Methods

		void Start ()
		{
			InitializeIAP ();
		}

		void OnEnable ()
		{
			EventManager.Instance.AddListener<BuyIAPItemEvent> (BuyIAPItemEventHandler);
		}

		void OnDisable ()
		{
			EventManager.Instance.RemoveListener<BuyIAPItemEvent> (BuyIAPItemEventHandler);
		}

		#endregion

		public void InitializeIAP ()
		{
			ConfigurationBuilder builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());

			for (int i = 0; i < macAppStoreIAPData.IApData.Length; i++)
			{
				Debug.Log("Add product: " + macAppStoreIAPData.IApData[i].StoreID);

				builder.AddProduct (
					macAppStoreIAPData.IApData[i].StoreID,
					macAppStoreIAPData.IApData[i].IsConsumable ? ProductType.Consumable : ProductType.NonConsumable,
					new IDs
					{
						{macAppStoreIAPData.IApData[i].StoreID, MacAppStore.Name}
					});
			}

			UnityPurchasing.Initialize (this, builder);
		}

		private bool IsInitialized ()
		{
			// Only say we are initialized if both the Purchasing references are set.
			return m_StoreController != null && m_StoreExtensionProvider != null;
		}

		public void OnInitializeFailed (InitializationFailureReason error)
		{
			Debug.Log ("Billing failed to initialize!");
			switch (error)
			{
				case InitializationFailureReason.AppNotKnown:
					Debug.LogError ("Is your App correctly uploaded on the relevant publisher console?");
					break;
				case InitializationFailureReason.PurchasingUnavailable:
					// Ask the user if billing is disabled in device settings.
					Debug.LogWarning ("Billing disabled!");
					break;
				case InitializationFailureReason.NoProductsAvailable:
					// Developer configuration error; check product metadata.
					Debug.LogWarning ("No products available for purchase!");
					break;
			}

			OnInitialize.Invoke (false);
		}

		/// <summary>
		/// This will be called when a purchase completes.
		/// </summary>
		public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
		{
			if (isDebugOn)
			{
				Debug.Log ("Purchase OK: " + e.purchasedProduct.definition.id);
				Debug.Log ("Receipt: " + e.purchasedProduct.receipt);
			}

			OnPurchaseSucceed.Invoke (e.purchasedProduct.definition.id);

			// Indicate we have handled this purchase, we will not be informed of it again.
			return PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed (Product item, PurchaseFailureReason r)
		{
			if (isDebugOn)
			{
				Debug.Log ("Purchase failed: " + item.definition.id);
				Debug.Log (r);
			}

			OnPurchaseFail.Invoke (item.definition.id);
		}

		/// <summary>
		/// This will be called when Unity IAP has finished initialising.
		/// </summary>
		public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
		{
			// Purchasing has succeeded initializing. Collect our Purchasing references.
			Debug.Log("OnInitialized: PASS");

			// Overall Purchasing system, configured with products for this application.
			m_StoreController = controller;
			// Store specific subsystem, for accessing device-specific store features.
			m_StoreExtensionProvider = extensions;

			if (isDebugOn)
			{
				Debug.Log ("Available items:");

				foreach (Product item in controller.products.all)
				{
					if (item.availableToPurchase)
					{
						Debug.Log (string.Join (" - ",
							new[]
							{
								item.metadata.localizedTitle,
								item.metadata.localizedDescription,
								item.metadata.isoCurrencyCode,
								item.metadata.localizedPrice.ToString (),
								item.metadata.localizedPriceString
							}));
					}
				}
			}

			if (null != m_StoreController)
			{
				// Prepare model for purchasing
				if (m_StoreController.products.all.Length > 0)
				{
					m_SelectedItemIndex = 0;
				}

				// Populate the product menu now that we have Products
				for (int t = 0; t < m_StoreController.products.all.Length; t++)
				{
					Product item = m_StoreController.products.all[t];
					string description = $"{item.metadata.localizedTitle} - {item.metadata.localizedPriceString}";

					// NOTE: my options list is created in InitUI
					// TODO: Populate products
					// FindObjectOfType<IAPHandler>().SetIAPButton (item.metadata.localizedTitle, item.metadata.localizedPriceString, item.definition.storeSpecificId);
				}

				OnInitialize.Invoke (true);
			}
		}

		public void BuyProduct (string productId)
		{
			if (m_StoreController != null)
			{
				Product product = m_StoreController.products.WithID (productId);
				if (product != null && product.availableToPurchase)
				{
					// if (isDebugOn)
					Debug.Log ($"Purchasing product asynchronously: '{product.definition.id}'");

					m_StoreController.InitiatePurchase (product);
				}
				else
				{
					// if (isDebugOn)
					Debug.Log ("BuyProduct FAILED. Not purchasing product, either is not found or is not available for purchase");
				}
			}
			else
			{
				// if (isDebugOn)
				Debug.Log ("BuyProduct FAILED. Not initialized.");
			}
		}

		// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
		// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
		public void RestorePurchases ()
		{
			// If Purchasing has not yet been set up ...
			if (!IsInitialized ())
			{
				// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
				Debug.Log ("RestorePurchases FAIL. Not initialized.");
				return;
			}

			// If we are running on an Apple device ... 
			if (Application.platform == RuntimePlatform.IPhonePlayer ||
			    Application.platform == RuntimePlatform.OSXPlayer)
			{
				// ... begin restoring purchases
				Debug.Log ("RestorePurchases started ...");

				// Fetch the Apple store-specific subsystem.
				IAppleExtensions apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions> ();
				// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
				// the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
				apple.RestoreTransactions (result =>
				{
					// The first phase of restoration. If no more responses are received on ProcessPurchase then 
					// no purchases are available to be restored.
					Debug.Log ("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
				});
			}
			// Otherwise ...
			else
			{
				// We are not running on an Apple device. No work is necessary to restore purchases.
				Debug.Log ("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
		}

		#region Event Handlers

		private void BuyIAPItemEventHandler (BuyIAPItemEvent eventDetails)
		{
			BuyProduct (eventDetails.ProductID);
		}

		#endregion
	}
}