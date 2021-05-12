using System.Threading.Tasks;
using UnityEngine;

namespace DynamicBox.Helpers
{
	public class TimedDestroy : MonoBehaviour
	{
		[Header ("Links")]
		[SerializeField] private int waitTime;

		#region Unity Methods

		async void Start ()
		{
			await Task.Delay (waitTime * 1000);
		}

		#endregion
	}
}