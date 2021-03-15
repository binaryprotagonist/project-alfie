using DynamicBox.Domain;
using DynamicBox.SaveManagement;
using UnityEngine;

namespace DynamicBox.Controllers
{
	public class SaveController : MonoBehaviour
	{
		[Header ("Parameters")]
		[SerializeField] private int letterCount;

		private SaveManager saveManager;
		private SaveData saveData;

		private int firstUnfinishedLetterIndex;
		public int FirstUnfinishedLetterIndex => firstUnfinishedLetterIndex;

		#region Unity Methods

		void OnEnable ()
		{
			saveManager = new SaveManager (StorageMethod.JSON);

			CheckSaveDataFile ();
			firstUnfinishedLetterIndex = FindFirstUnfinishedLetter ();
		}

		#endregion

		private void CheckSaveDataFile ()
		{
			if (saveManager.FileExists ("SaveData"))
			{
				saveData = saveManager.LoadFromFile ("SaveData", new SaveData ());
			}
			else
			{
				saveData = saveManager.LoadFromFile ("SaveData", new SaveData ());

				for (int i = 0; i < letterCount; i++)
				{
					LetterData letterData = new LetterData (i, false);
					saveData.letterData.Add (letterData);
				}

				saveManager.SaveToFile (saveData, "SaveData");
			}
		}

		public int FindFirstUnfinishedLetter ()
		{
			foreach (LetterData letterDatum in saveData.letterData)
			{
				if (!letterDatum.isCompleted)
				{
					return letterDatum.index;
				}
			}
			
			Debug.LogError ("All letters completed");
			return 0;
		}

		public void SetLetterFinished (int index)
		{
			saveData.letterData[index].isCompleted = true;

			saveManager.SaveToFile (saveData, "SaveData");
		}

		public bool CheckIfLetterFinished (int index)
		{
			return saveData.letterData[index].isCompleted;
		}
	}
}