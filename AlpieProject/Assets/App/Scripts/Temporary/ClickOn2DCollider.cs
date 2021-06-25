using DynamicBox.EventManagement;
using DynamicBox.EventManagement.GameEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DynamicBox.Temporary
{
	public class ClickOn2DCollider : MonoBehaviour //, IPointerEnterHandler, IPointerExitHandler
	{
		private bool isEventRaised;

		void Update ()
		{
			// if (Input.GetMouseButton (0))
			// {
			// 	Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			// 	RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
			//
			// 	if (hit.collider != null && hit.collider.CompareTag ("LetterCollider"))
			// 	{
			// 		Debug.Log ("Click on LetterCollider");
			//
			// 		EventManager.Instance.Raise (new ClickOnLetterEvent (true));
			// 		
			// 		// if (!isEventRaised)
			// 		// {
			// 		// 	isEventRaised = true;
			// 		// 	EventManager.Instance.Raise (new ClickOnLetterEvent (true));
			// 		// }
			// 	}
			// 	else
			// 	{
			// 		Debug.Log ("Click not on LetterCollider");
			//
			// 		EventManager.Instance.Raise (new ClickOnLetterEvent (false));
			// 		
			// 		// if (!isEventRaised)
			// 		// {
			// 		// 	isEventRaised = true;
			// 		// 	EventManager.Instance.Raise (new ClickOnLetterEvent (false));
			// 		// }
			// 	}
			// }
			// else
			// {
			// 	EventManager.Instance.Raise (new ClickOnLetterEvent (false));
			// }
		}

		// public void OnPointerEnter (PointerEventData eventData)
		// {
		// 	Debug.Log ("OnPointerEnter");
		// }
		//
		// public void OnPointerExit (PointerEventData eventData)
		// {
		// 	Debug.Log ("OnPointerExit");
		// }
	}
}