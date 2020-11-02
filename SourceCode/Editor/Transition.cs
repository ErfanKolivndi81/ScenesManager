using UnityEngine;

namespace IRK.Unity.ScenesManager
{
	//A interface between 'TransitionData' and 'TransitionEdga'
	//And to selection a transition
	public class Transition : ScriptableObject
	{
		[SerializeField]
		private TransitionData _data;

		public TransitionData data
		{
			get { return _data; }
			set { _data = value; }
		}

		public override string ToString()
		{
			return data.ToString();
		}
	}
}