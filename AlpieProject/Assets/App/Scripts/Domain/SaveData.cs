using System;
using System.Collections.Generic;

namespace DynamicBox.Domain
{
	[Serializable]
	public class SaveData
	{
		public List<LetterData> letterData;
	}

	[Serializable]
	public class LetterData
	{
		public int index;
		public bool isCompleted;

		public LetterData (int index, bool isCompleted)
		{
			this.index = index;
			this.isCompleted = isCompleted;
		}
	}
}