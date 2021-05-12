using UnityEngine;

namespace DynamicBox.Helpers
{
	public class HapticFeedback : MonoBehaviour
	{
		public void Vibrate ()
		{
			Handheld.Vibrate();
		}
	}
}