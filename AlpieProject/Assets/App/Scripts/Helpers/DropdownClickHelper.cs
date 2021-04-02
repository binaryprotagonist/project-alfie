using UnityEngine;
using UnityEngine.EventSystems;

namespace DynamicBox.Helpers
{
	public class DropdownClickHelper : MonoBehaviour, IPointerDownHandler
	{
		[Header ("Links")]
		[SerializeField] private GameObject dropdownBGImage;

		private bool isImageActive;
		
		public void OnPointerDown (PointerEventData eventData)
		{
			Debug.Log ("Dropdown click");

			isImageActive = !isImageActive;
			
			dropdownBGImage.SetActive (isImageActive);
		}
	}
}