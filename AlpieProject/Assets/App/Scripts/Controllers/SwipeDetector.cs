using System;
using TMPro;
using UnityEngine;

namespace DynamicBox.Controllers
{
	public class SwipeDetector : MonoBehaviour
	{
		public TextMeshProUGUI text;
		public Transform rocket;

		private Vector2 fingerDown;
		private Vector2 fingerUp;
		public bool detectSwipeOnlyAfterRelease = false;

		public float swipeThreshold = 20f;
		public float distanceMagnitudeThreshold = 1;
		public float turnSpeed;

		private Vector3 oldPos;
		private Vector3 oldDirection;

		void Update ()
		{
			// if (Input.GetMouseButton (0))
			// {
			// 	if ((Input.mousePosition - oldPos).normalized.magnitude < distanceMagnitudeThreshold)
			// 	{
			// 		float angle = Mathf.Atan2 (oldPos.y - Input.mousePosition.y, oldPos.x - Input.mousePosition.x) * Mathf.Rad2Deg;
			//
			// 		// Debug.Log ("angle = " + angle);
			//
			// 		rocket.transform.eulerAngles = new Vector3 (0, 0, angle);
			//
			// 		// rocket.transform.rotation = Quaternion.Slerp (rocket.transform.rotation, new Quaternion (0, 0, angle, float.Epsilon), turnSpeed * Time.deltaTime);
			// 		// rocket.transform.rotation = Quaternion.Euler (0,0, Mathf.Lerp (oldAngle, angle, turnSpeed * Time.deltaTime));
			//
			// 		// Quaternion targetRotation = Quaternion.Euler (new Vector3 ((Input.mousePosition - oldPos).x, (Input.mousePosition - oldPos).y, angle));
			// 		// rocket.transform.rotation = Quaternion.Slerp (rocket.transform.rotation, targetRotation, Time.time * turnSpeed);
			// 		// Debug.Log ("targetRotation = " + targetRotation);
			//
			// 		// oldAngle = angle;
			// 		oldPos = Input.mousePosition;
			// 		oldDirection = Input.mousePosition - oldPos;
			// 	}
			// }

			foreach (Touch touch in Input.touches)
			{
				if (touch.phase == TouchPhase.Began)
				{
					fingerUp = touch.position;
					fingerDown = touch.position;
				}
			
				//Detects Swipe while finger is still moving
				if (touch.phase == TouchPhase.Moved)
				{
					if (!detectSwipeOnlyAfterRelease)
					{
						fingerDown = touch.position;
						checkSwipe ();
					}
				}
			
				//Detects swipe after finger is released
				if (touch.phase == TouchPhase.Ended)
				{
					fingerDown = touch.position;
					checkSwipe ();
				}
			}
		}

		void checkSwipe ()
		{
			//Check if Vertical swipe
			if (verticalMove () > swipeThreshold && verticalMove () > horizontalValMove ())
			{
				//Debug.Log("Vertical");
				if (fingerDown.y - fingerUp.y > 0) //up swipe
				{
					OnSwipeUp ();
				}
				else if (fingerDown.y - fingerUp.y < 0) //Down swipe
				{
					OnSwipeDown ();
				}

				fingerUp = fingerDown;
			}

			//Check if Horizontal swipe
			else if (horizontalValMove () > swipeThreshold && horizontalValMove () > verticalMove ())
			{
				//Debug.Log("Horizontal");
				if (fingerDown.x - fingerUp.x > 0) //Right swipe
				{
					OnSwipeRight ();
				}
				else if (fingerDown.x - fingerUp.x < 0) //Left swipe
				{
					OnSwipeLeft ();
				}

				fingerUp = fingerDown;
			}

			//No Movement at-all
			else
			{
				//Debug.Log("No Swipe!");
			}
		}

		float verticalMove ()
		{
			return Mathf.Abs (fingerDown.y - fingerUp.y);
		}

		float horizontalValMove ()
		{
			return Mathf.Abs (fingerDown.x - fingerUp.x);
		}

		// Callback functions
		void OnSwipeUp ()
		{
			Debug.Log ("Swipe Up");
			// text.text = "Swipe Up";
			rocket.transform.eulerAngles = new Vector3 (0, 0, -90);
		}

		void OnSwipeDown ()
		{
			Debug.Log ("Swipe Down");
			// text.text = "Swipe Down";
			rocket.transform.eulerAngles = new Vector3 (0, 0, 90);
		}

		void OnSwipeLeft ()
		{
			Debug.Log ("Swipe Left");
			// text.text = "Swipe Left";
			rocket.transform.eulerAngles = new Vector3 (0, 0, 0);
		}

		void OnSwipeRight ()
		{
			Debug.Log ("Swipe Right");
			// text.text = "Swipe Right";
			rocket.transform.eulerAngles = new Vector3 (0, 0, 180);
		}
	}
}