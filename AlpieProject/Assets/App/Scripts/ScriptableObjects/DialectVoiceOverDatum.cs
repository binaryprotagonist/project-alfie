using UnityEngine;

namespace DynamicBox.ScriptableObjects
{
	[CreateAssetMenu (menuName = "DynamicBox/Sound/Dialect Voice Over", order = 1301)]
	public class DialectVoiceOverDatum : ScriptableObject
	{
		[Header ("Parameters")]
		[SerializeField] private AudioClip[] _voiceOvers;
		public AudioClip[] VoiceOvers => _voiceOvers;
	}
}