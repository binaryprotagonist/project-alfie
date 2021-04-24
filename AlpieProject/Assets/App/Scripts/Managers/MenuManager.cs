using System.Collections;
using UnityEngine;

namespace DynamicBox.Managers
{
	public class MenuManager : MonoBehaviour
	{
		[Header("Links")]
		[SerializeField] private GameManager gameManager;

		public void ShowGameView ()
		{
			StartCoroutine (CreateShapeDelayed ());
		}

		private IEnumerator CreateShapeDelayed ()
		{
			yield return new WaitForSeconds (0.2f);
			
			gameManager.SwitchBackToShape ();
		}
	}
}