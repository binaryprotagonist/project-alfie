using System;
using System.Collections.Generic;
using DynamicBox.Controllers;
using DynamicBox.EventManagement;
using DynamicBox.EventManagement.GameEvents.VoiceOver;
using DynamicBox.UI.ViewControllers;
using EnglishTracingBook;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicBox.Managers
{
	public class GameManager : MonoBehaviour
	{
		[Header ("Parameters")]
		[SerializeField] private bool isTestMode;

		[Header ("Links")]
		[SerializeField] private LettersViewController lettersViewController;
		[SerializeField] private Canvas myCanvas;
		[SerializeField] private GameObject rocket;
		
		[Space]
		[SerializeField] private GameObject testShapePrefab;
		[SerializeField] private Color paintColor;
		[SerializeField] private GameObject[] letterPrefabs;

		// Whether the script is running or not.
		public bool isRunning = true;

		// The path.
		private Path path;

		// The shape parent.
		public Transform shapeParent;

		// The shape reference.
		[HideInInspector] public Shape shape;

		// The path fill image.
		private Image pathFillImage;

		///The click position.
		private Vector3 clickPostion;

		// The direction between click and shape.
		private Vector2 direction;

		// The current angle , angleOffset and fill amount.
		private float angle, angleOffset, fillAmount;

		// The clock wise sign.
		private float clockWiseSign;

		// The hand reference.
		public Transform hand;

		// The default size of the cursor.
		private Vector3 cursorDefaultSize;

		// The click size of the cursor.
		private Vector3 cursorClickSize;

		// The target quarter of the radial fill.
		private float targetQuarter;

		// The effects audio source.
		private AudioSource effectsAudioSource;

		// The hit2d reference.
		private RaycastHit2D hit2d;

		// The shapes manager reference.
		private ShapesManager shapesManager;

		// The compound shape reference.
		public static CompoundShape compoundShape;

		private int letterIndex;

		void Awake ()
		{
			//Initiate values and setup the references
			cursorDefaultSize = hand.transform.localScale;
			cursorClickSize = cursorDefaultSize / 1.2f;

			ShapesManager.shapesManagerReference = "SShapesManager";
			if (!string.IsNullOrEmpty (ShapesManager.shapesManagerReference))
			{
				shapesManager = GameObject.Find (ShapesManager.shapesManagerReference).GetComponent<ShapesManager> ();
			}
			else
			{
				Debug.LogErrorFormat ("You have to start the game from the Main scene");
			}

			ResetTargetQuarter ();
			if (isTestMode)
			{
				CreateShape ();
			}
		}

		void Start ()
		{
			//Initiate values and setup the references
			if (effectsAudioSource == null)
			{
				effectsAudioSource = GameObject.Find ("AudioSources").GetComponents<AudioSource> ()[1];
			}
		}

		void Update ()
		{
			//Game Logic is here

			DrawHand (GetCurrentPlatformClickPosition (Camera.main));

			if (shape == null)
			{
				return;
			}

			if (shape.completed)
			{
				return;
			}

			if (Input.GetMouseButtonDown (0))
			{
				hit2d = Physics2D.Raycast (GetCurrentPlatformClickPosition (Camera.main), Vector2.zero);
				if (hit2d.collider != null)
				{
					if (hit2d.transform.CompareTag ("Start"))
					{
						OnStartHitCollider (hit2d);
						shape.CancelInvoke ();
						shape.DisableTracingHand ();
						EnableHand ();
					}
					else if (hit2d.transform.CompareTag ("Collider"))
					{
						shape.DisableTracingHand ();
						EnableHand ();
					}
				}
			}
			else if (Input.GetMouseButtonUp (0))
			{
				DisableHand ();
				shape.Invoke ("EnableTracingHand", 1);
				ResetPath ();
				rocket.SetActive (false);
			}

			if (!isRunning || path == null || pathFillImage == null)
			{
				return;
			}

			if (path.completed)
			{
				return;
			}

			/*
			hit2d = Physics2D.Raycast (GetCurrentPlatformClickPosition (Camera.main), Vector2.zero);
			if (hit2d.collider == null) {
				if (wrongSFX != null && effectsAudioSource != null) {
					CommonUtil.PlayOneShotClipAt (wrongSFX, Vector3.zero, effectsAudioSource.volume);
				}
				ResetPath ();
				return;
			}*/

			switch (path.fillMethod)
			{
				case Path.FillMethod.Radial:
					RadialFill ();
					break;
				case Path.FillMethod.Linear:
					LinearFill ();
					break;
				case Path.FillMethod.Point:
					PointFill ();
					break;
			}
		}

		// On the start hit collider event.
		private void OnStartHitCollider (RaycastHit2D hit2d)
		{
			path = hit2d.transform.GetComponentInParent<Path> ();

			pathFillImage = CommonUtil.FindChildByTag (path.transform, "Fill").GetComponent<Image> ();

			if (path.completed || !shape.IsCurrentPath (path))
			{
				ReleasePath ();
			}
			else
			{
				path.StopAllCoroutines ();
				CommonUtil.FindChildByTag (path.transform, "Fill").GetComponent<Image> ().color = paintColor;
			}

			if (path != null)
				if (!path.shape.enablePriorityOrder)
				{
					shape = path.shape;
				}
		}

		public void SwitchBackToShape ()
		{
			lettersViewController.ShowCurrentLetter ();
		}
		
		// Create new shape.
		public void CreateShape (int index = 0)
		{
			letterIndex = index;

			CompoundShape currentCompoundShape = FindObjectOfType<CompoundShape> ();
			if (currentCompoundShape != null)
			{
				DestroyImmediate (currentCompoundShape.gameObject);
			}
			else
			{
				Shape shapeComponent = FindObjectOfType<Shape> ();
				if (shapeComponent != null)
				{
					DestroyImmediate (shapeComponent.gameObject);
				}
			}

			try
			{
				GameObject shapeGameObject;
				if (isTestMode)
				{
					shapeGameObject = Instantiate (testShapePrefab, Vector3.zero, Quaternion.identity);
				}
				else
				{
					// letterIndex = PlayerPrefs.GetInt ("LetterIndex");

					shapeGameObject = Instantiate (letterPrefabs[letterIndex], Vector3.zero, Quaternion.identity);
				}

				shapeGameObject.transform.SetParent (shapeParent);
				shapeGameObject.transform.localPosition = Vector3.zero;
				shapeGameObject.transform.localRotation = Quaternion.identity;
				shapeGameObject.transform.localScale = Vector3.one;

				compoundShape = FindObjectOfType<CompoundShape> ();
				if (compoundShape != null)
				{
					shape = compoundShape.shapes[0];
					StartAutoTracing (shape);
				}
				else
				{
					shape = FindObjectOfType<Shape> ();
				}
			}
			catch (Exception ex)
			{
				//Catch the exception or display an alert
				Debug.LogError (ex.Message);
			}

			if (shape == null)
			{
				return;
			}

			// shape.Spell ();

			EnableGameManager ();
		}

		public void DestroyShape ()
		{
			Shape shapeComponent = FindObjectOfType<Shape> ();
			if (shapeComponent != null)
			{
				DestroyImmediate (shapeComponent.gameObject);
			}
		}

		// Draw the hand.
		private void DrawHand (Vector3 clickPosition)
		{
			if (hand == null)
			{
				return;
			}

			hand.transform.position = clickPosition;
		}

		// Set the size of the hand to default size.
		private void SetHandDefaultSize ()
		{
			hand.transform.localScale = cursorDefaultSize;
		}

		// Set the size of the hand to click size.
		private void SetHandClickSize ()
		{
			hand.transform.localScale = cursorClickSize;
		}

		// Get the current platform click position.
		private Vector3 GetCurrentPlatformClickPosition (Camera camera)
		{
			Vector3 clickPosition = Vector3.zero;

			if (Application.isMobilePlatform)
			{
				//current platform is mobile
				if (Input.touchCount != 0)
				{
					Touch touch = Input.GetTouch (0);
					clickPosition = touch.position;
				}
			}
			else
			{
				//others
				clickPosition = Input.mousePosition;
			}

			clickPosition = camera.ScreenToWorldPoint (clickPosition); //get click position in the world space
			clickPosition.z = 0;
			return clickPosition;
		}

		// Radial the fill method.
		private void RadialFill ()
		{
			rocket.SetActive (true);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out Vector2 pos);
			rocket.transform.position = myCanvas.transform.TransformPoint(pos);
			
			if (!RectTransformUtility.RectangleContainsScreenPoint(shape.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
			{
				rocket.SetActive(false);
			}
			
			clickPostion = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			direction = clickPostion - path.transform.position;

			angleOffset = 0;
			clockWiseSign = (pathFillImage.fillClockwise ? 1 : -1);

			switch (pathFillImage.fillOrigin)
			{
				case 0:
					//Bottom
					angleOffset = 0;
					break;
				case 1:
					//Right
					angleOffset = clockWiseSign * 90;
					break;
				case 2:
					//Top
					angleOffset = -180;
					break;
				case 3:
					//left
					angleOffset = -clockWiseSign * 90;
					break;
			}

			angle = Mathf.Atan2 (-clockWiseSign * direction.x, -direction.y) * Mathf.Rad2Deg + angleOffset;

			if (angle < 0)
				angle += 360;

			angle = Mathf.Clamp (angle, 0, 360);
			angle -= path.radialAngleOffset;

			if (path.quarterRestriction)
			{
				if (!(angle >= 0 && angle <= targetQuarter))
				{
					pathFillImage.fillAmount = 0;
					return;
				}

				if (angle >= targetQuarter / 2)
				{
					targetQuarter += 90;
				}
				else if (angle < 45)
				{
					targetQuarter = 90;
				}

				targetQuarter = Mathf.Clamp (targetQuarter, 90, 360);
			}

			fillAmount = Mathf.Abs (angle / 360.0f);
			pathFillImage.fillAmount = fillAmount;
			CheckPathComplete ();
		}

		// Linear fill method.
		private void LinearFill ()
		{
			rocket.SetActive (true);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out Vector2 pos);
			rocket.transform.position = myCanvas.transform.TransformPoint(pos);
			
			if (!RectTransformUtility.RectangleContainsScreenPoint(shape.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
			{
				rocket.SetActive(false);
			}
			
			clickPostion = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			Vector3 rotation = path.transform.eulerAngles;
			rotation.z -= path.offset;

			Rect rect = CommonUtil.RectTransformToScreenSpace (path.GetComponent<RectTransform> ());

			Vector3 pos1 = Vector3.zero, pos2 = Vector3.zero;

			if (path.type == Path.ShapeType.Horizontal)
			{
				pos1.x = path.transform.position.x - Mathf.Sin (rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;
				pos1.y = path.transform.position.y - Mathf.Cos (rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;

				pos2.x = path.transform.position.x + Mathf.Sin (rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;
				pos2.y = path.transform.position.y + Mathf.Cos (rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;
			}
			else
			{
				pos1.x = path.transform.position.x - Mathf.Cos (rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;
				pos1.y = path.transform.position.y - Mathf.Sin (rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;

				pos2.x = path.transform.position.x + Mathf.Cos (rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;
				pos2.y = path.transform.position.y + Mathf.Sin (rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;
			}

			pos1.z = path.transform.position.z;
			pos2.z = path.transform.position.z;

			if (path.flip)
			{
				Vector3 temp = pos2;
				pos2 = pos1;
				pos1 = temp;
			}

			clickPostion.x = Mathf.Clamp (clickPostion.x, Mathf.Min (pos1.x, pos2.x), Mathf.Max (pos1.x, pos2.x));
			clickPostion.y = Mathf.Clamp (clickPostion.y, Mathf.Min (pos1.y, pos2.y), Mathf.Max (pos1.y, pos2.y));
			fillAmount = Vector2.Distance (clickPostion, pos1) / Vector2.Distance (pos1, pos2);
			pathFillImage.fillAmount = fillAmount;
			CheckPathComplete ();
		}

		// Point fill.
		private void PointFill ()
		{
			pathFillImage.fillAmount = 1;
			CheckPathComplete ();
		}

		// Checks wehther path completed or not.
		private void CheckPathComplete ()
		{
			// Debug.Log ("fillAmount = " + fillAmount);
			if (fillAmount >= path.completeOffset)
			{
				rocket.SetActive (false);
				path.completed = true;
				path.AutoFill ();
				path.SetNumbersVisibility (false);
				ReleasePath ();
				if (CheckShapeComplete ())
				{
					shape.completed = true;
					OnShapeComplete ();
				}
				else
				{
					PlayCorrectSFX ();
				}

				if (shape != null)
				{
					shape.ShowPathNumbers (shape.GetCurrentPathIndex ());
				}

				hit2d = Physics2D.Raycast (GetCurrentPlatformClickPosition (Camera.main), Vector2.zero);
				if (hit2d.collider != null)
				{
					if (hit2d.transform.CompareTag ("Start"))
					{
						if (shape.IsCurrentPath (hit2d.transform.GetComponentInParent<Path> ()))
						{
							OnStartHitCollider (hit2d);
						}
					}
				}
			}
		}

		// Check whether the shape completed or not.
		private bool CheckShapeComplete ()
		{
			bool shapeCompleted = true;
			Path[] paths = shape.GetComponentsInChildren<Path> ();
			foreach (Path path in paths)
			{
				if (!path.completed)
				{
					shapeCompleted = false;
					break;
				}
			}

			return shapeCompleted;
		}

		// On shape completed event.
		private void OnShapeComplete ()
		{
			lettersViewController.SetLetterFinished (letterIndex);
			
			EventManager.Instance.Raise (new LetterFinishedEvent ());

			Debug.Log ("Shape completed");

			bool allDone = true;

			List<Shape> shapes = new List<Shape> ();

			if (compoundShape != null)
			{
				shapes = compoundShape.shapes;
				allDone = compoundShape.IsCompleted ();

				if (!allDone)
				{
					shape = compoundShape.shapes[compoundShape.GetCurrentShapeIndex ()];
					StartAutoTracing (shape);
				}
			}
			else
			{
				shapes.Add (shape);
			}

			if (allDone)
			{
				DisableHand ();

				foreach (Shape s in shapes)
				{
					Animator shapeAnimator = s.GetComponent<Animator> ();
					shapeAnimator.SetBool (s.name, false);
					shapeAnimator.SetTrigger ("Completed");
				}

				Area.Show ();

				AdsManager.instance.HideAdvertisment ();
				AdsManager.instance.ShowAdvertisment (AdPackage.AdEvent.Event.ON_SHOW_WIN_DIALOG);
			}
			else
			{
				PlayCorrectSFX ();
			}

			DestroyShape ();
		}

		// Save the status of the shape(stars,path colors).
		private void SaveShapeStatus (List<Shape> shapes)
		{
			DataManager.SaveShapeStars (TableShape.selectedShape.ID, CommonUtil.GetTableShapeStars (FindObjectOfType<Progress> ().starsNumber), shapesManager);
			if (TableShape.selectedShape.ID + 1 <= shapesManager.shapes.Count)
			{
				DataManager.SaveShapeLockedStatus (TableShape.selectedShape.ID + 1, false, shapesManager);
			}

			int compundID = 0;

			foreach (Shape s in shapes)
			{
				if (compoundShape != null)
				{
					compundID = compoundShape.GetShapeIndexByInstanceID (s.GetInstanceID ());
				}

				List<Transform> paths = CommonUtil.FindChildrenByTag (s.transform.Find ("Paths"), "Path");
				int from, to;
				string[] slices;
				foreach (Transform p in paths)
				{
					slices = p.name.Split ('-');
					from = int.Parse (slices[1]);
					to = int.Parse (slices[2]);
					DataManager.SaveShapePathColor (TableShape.selectedShape.ID, compundID, from, to, CommonUtil.FindChildByTag (p, "Fill").GetComponent<Image> ().color, shapesManager);
				}
			}
		}

		// Reset the shape.
		public void ResetShape ()
		{
			List<Shape> shapes = new List<Shape> ();
			if (compoundShape != null)
			{
				shapes = compoundShape.shapes;
			}
			else
			{
				shapes.Add (shape);
			}

			// completeEffect.emit = false;
			// GameObject.Find ("NextButton").GetComponent<Animator> ().SetBool ("Select", false);
			Area.Hide ();

			foreach (Shape s in shapes)
			{
				if (s == null)
					continue;

				s.completed = false;
				s.GetComponent<Animator> ().SetBool ("Completed", false);
				s.CancelInvoke ();
				s.DisableTracingHand ();
				Path[] paths = s.GetComponentsInChildren<Path> ();
				foreach (Path path in paths)
				{
					path.Reset ();
				}

				if (compoundShape == null)
				{
					StartAutoTracing (s);
				}
				else if (compoundShape.GetShapeIndexByInstanceID (s.GetInstanceID ()) == 0)
				{
					shape = compoundShape.shapes[0];
					StartAutoTracing (shape);
				}

				// s.Spell ();
			}
		}

		// Starts the auto tracing for the current path.
		public void StartAutoTracing (Shape s)
		{
			if (s == null)
			{
				return;
			}

			//Hide Numbers for other shapes , if we have compound shape
			if (compoundShape != null)
			{
				foreach (Shape ts in compoundShape.shapes)
				{
					if (s.GetInstanceID () != ts.GetInstanceID ())
						ts.ShowPathNumbers (-1);
				}
			}

			s.Invoke ("EnableTracingHand", 2);
			s.ShowPathNumbers (s.GetCurrentPathIndex ());
		}

		// Play the correct SFX.
		public void PlayCorrectSFX ()
		{
			// if (correctSFX != null && effectsAudioSource != null)
			// {
			// 	CommonUtil.PlayOneShotClipAt (correctSFX, Vector3.zero, effectsAudioSource.volume);
			// }
		}

		// Reset the path.
		private void ResetPath ()
		{
			// Debug.Log ("Shape name = " + shape.name);
			//
			// switch (shape.name)
			// {
			// 	case "20. fa(Clone)":
			// 		ResetShape ();
			// 		return;
			// 		// break;
			// }

			if (path != null)
				path.Reset ();
			ReleasePath ();
			ResetTargetQuarter ();
		}

		// Reset the target quarter.
		public void ResetTargetQuarter ()
		{
			targetQuarter = 90;
		}

		// Release the path.
		private void ReleasePath ()
		{
			path = null;
			pathFillImage = null;
		}

		// Enable the hand.
		public void EnableHand ()
		{
			// hand.GetComponent<SpriteRenderer> ().enabled = true;
		}

		// Disable the hand.
		public void DisableHand ()
		{
			// hand.GetComponent<SpriteRenderer> ().enabled = false;
		}

		// Enable the game manager.
		private void EnableGameManager ()
		{
			isRunning = true;
		}
	}
}