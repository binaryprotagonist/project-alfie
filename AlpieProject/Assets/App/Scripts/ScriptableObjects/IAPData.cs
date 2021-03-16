using DynamicBox.Domain;
using UnityEngine;

namespace DynamicBox.ScriptableObjects
{
	[CreateAssetMenu (menuName = "DynamicBox/IAP/IAPData", order = 1300)]
	public class IAPData : ScriptableObject
	{
		[SerializeField] private IAPDatum[] _iapData;
		public IAPDatum[] IApData => _iapData;
	}
}