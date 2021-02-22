using TMPro;
using UnityEngine;

namespace DynamicBox.UI.ViewControllers
{
	public class DialectsViewController : MonoBehaviour
	{
		[SerializeField] private TMP_Dropdown dialectDropdown;

		#region Unity Methods

		void Start ()
		{
			dialectDropdown.onValueChanged.AddListener (delegate { DropdownValueChanged (dialectDropdown); });
		}

		#endregion

		private void DropdownValueChanged (TMP_Dropdown change)
		{
			Debug.Log ("Selected dialect : " + change.value);
		}
	}
}