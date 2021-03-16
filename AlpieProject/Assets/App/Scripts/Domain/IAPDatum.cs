using System;
using UnityEngine;

namespace DynamicBox.Domain
{
	[Serializable]
	public class IAPDatum
	{
		[SerializeField] private string _title;
		public string Title => _title;
		
		[SerializeField] private string _price;
		public string Price => _price;
		
		[SerializeField] private string _storeID;
		public string StoreID => _storeID;

		[SerializeField] private bool _isConsumable;
		public bool IsConsumable => _isConsumable;
	}
}