using System;
using System.Collections;
using System.Collections.Generic;
using EnglishTracingBook;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DynamicBox.Managers
{
	public class GameManager : MonoBehaviour
	{
		[Header ("Parameters")]
		[SerializeField] private bool isTestMode;

		[Header ("Links")] 
		[SerializeField] private GameObject testShapePrefab;
		[SerializeField] private Color paintColor;
		[SerializeField] private GameObject[] letterPrefabs;

		/// <summary>
		/// Whether the script is running or not.
		/// </summary>
		public bool isRunning = true;

		/// <summary>
		/// The path.
		/// </summary>
		private Path path;

		/// <summary>
		/// The shape parent.
		/// </summary>
		public Transform shapeParent;

		/// <summary>
		/// The shape reference.
		/// </summary>
		/*[HideInInspector]*/ public Shape shape;

		/// <summary>
		/// The path fill image.
		/// </summary>
		private Image pathFillImage;

		/// <summary>
		/// The click postion.
		/// </summary>
		private Vector3 clickPostion;

		/// <summary>
		/// The direction between click and shape.
		/// </summary>
		private Vector2 direction;

		/// <summary>
		/// The current angle , angleOffset and fill amount.
		/// </summary>
		private float angle, angleOffset, fillAmount;

		/// <summary>
		/// The clock wise sign.
		/// </summary>
		private float clockWiseSign;

		/// <summary>
		/// The hand reference.
		/// </summary>
		public Transform hand;

		/// <summary>
		/// The default size of the cursor.
		/// </summary>
		private Vector3 cursorDefaultSize;

		/// <summary>
		/// The click size of the cursor.
		/// </summary>
		private Vector3 cursorClickSize;

		/// <summary>
		/// The target quarter of the radial fill.
		/// </summary>
		private float targetQuarter;

		/// <summary>
		/// The effects audio source.
		/// </summary>
		private AudioSource effectsAudioSource;

		/// <summary>
		/// The hit2d reference.
		/// </summary>
		private RaycastHit2D hit2d;

		/// <summary>
		/// The shapes manager reference.
		/// </summary>
		private ShapesManager shapesManager;

		/// <summary>
		/// The compound shape reference.
		/// </summary>
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

		/// <summary>
		/// On the start hit collider event.
		/// </summary>
		/// <param name="hit2d">Hit2d.</param>
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

		/// <summary>
		/// Create new shape.
		/// </summary>
		public void CreateShape ()
		{
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
					letterIndex = PlayerPrefs.GetInt ("LetterIndex");

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

		/// <summary>
		/// Draw the hand.
		/// </summary>
		/// <param name="clickPosition">Click position.</param>
		private void DrawHand (Vector3 clickPosition)
		{
			if (hand == null)
			{
				return;
			}

			hand.transform.position = clickPosition;
		}

		/// <summary>
		/// Set the size of the hand to default size.
		/// </summary>
		private void SetHandDefaultSize ()
		{
			hand.transform.localScale = cursorDefaultSize;
		}

		/// <summary>
		/// Set the size of the hand to click size.
		/// </summary>
		private void SetHandClickSize ()
		{
			hand.transform.localScale = cursorClickSize;
		}

		/// <summary>
		/// Get the current platform click position.
		/// </summary>
		/// <returns>The current platform click position.</returns>
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

		/// <summary>
		/// Radial the fill method.
		/// </summary>
		private void RadialFill ()
		{
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

		/// <summary>
		/// Linear fill method.
		/// </summary>
		private void LinearFill ()
		{
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

		/// <summary>
		/// Point fill.
		/// </summary>
		private void PointFill ()
		{
			pathFillImage.fillAmount = 1;
			CheckPathComplete ();
		}

		/// <summary>
		/// Checks wehther path completed or not.
		/// </summary>
		private void CheckPathComplete ()
		{
			// Debug.Log ("fillAmount = " + fillAmount);
			if (fillAmount >= path.completeOffset)
			{
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

				shape.ShowPathNumbers (shape.GetCurrentPathIndex ());

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

		/// <summary>
		/// Check whether the shape completed or not.
		/// </summary>
		/// <returns><c>true</c>, if shape completed, <c>false</c> otherwise.</returns>
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

		/// <summary>
		/// On shape completed event.
		/// </summary>
		private void OnShapeComplete ()
		{
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
		}

		/// <summary>
		/// Save the status of the shape(stars,path colors) .
		/// </summary>
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

		/// <summary>
		/// Reset the shape.
		/// </summary>
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

		/// <summary>
		/// Starts the auto tracing for the current path.
		/// </summary>
		/// <param name="s">Shape Reference.</param>
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

		/// <summary>
		/// Play the correct SFX.
		/// </summary>
		public void PlayCorrectSFX ()
		{
			// if (correctSFX != null && effectsAudioSource != null)
			// {
			// 	CommonUtil.PlayOneShotClipAt (correctSFX, Vector3.zero, effectsAudioSource.volume);
			// }
		}

		/// <summary>
		/// Reset the path.
		/// </summary>
		private void ResetPath ()
		{
			if (path != null)
				path.Reset ();
			ReleasePath ();
			ResetTargetQuarter ();
		}

		/// <summary>
		/// Reset the target quarter.
		/// </summary>
		public void ResetTargetQuarter ()
		{
			targetQuarter = 90;
		}

		/// <summary>
		/// Release the path.
		/// </summary>
		private void ReleasePath ()
		{
			path = null;
			pathFillImage = null;
		}

		/// <summary>
		/// Enable the hand.
		/// </summary>
		public void EnableHand ()
		{
			// hand.GetComponent<SpriteRenderer> ().enabled = true;
		}

		/// <summary>
		/// Disable the hand.
		/// </summary>
		public void DisableHand ()
		{
			// hand.GetComponent<SpriteRenderer> ().enabled = false;
		}

		/// <summary>
		/// Enable the game manager.
		/// </summary>
		private void EnableGameManager ()
		{
			isRunning = true;
		}

		public void OpenMenuScene ()
		{
			SceneManager.LoadScene ("App/Scenes/Menu");
		}
	}
}