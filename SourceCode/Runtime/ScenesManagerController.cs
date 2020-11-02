using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IRK.Unity.ScenesManager
{
	/// <summary>
	/// A class to store all transitions and nodes(ScenesData)
	/// </summary>
	public class ScenesManagerController : ScriptableObject
	{

#if UNITY_EDITOR
		public bool visibleGrid;

		/// <summary>
		/// A class to hold the position of the SceneNodes and the scene index.(Only UnityEditor)
		/// </summary>
		[System.Serializable]
		public class SceneNodeData
		{
			public int buildIndex;
			public string id;
			public float x;
			public float y;

			public Vector2 position { get { return new Vector2(x, y); } }

			public SceneNodeData(GUID guid,int index, Vector2 pos)
			{
				this.id = guid.ToString();
				buildIndex = index;
				x = pos.x;
				y = pos.y;
			}
		}

		/// <summary>
		/// See: <seealso cref="SceneNodeData"/>
		/// </summary>
		public List<SceneNodeData> nodesData;

		/// <summary>
		/// Set the duration of all TransitionEffects.(Only UnityEditor)
		/// </summary>
		/// <param name="duration">Run time of TransitionEffect.</param>
		public void SetDurationAllTransitionEffects(float duration)
		{
			for (int i = 0; i < transitions.Count; i++)
			{
				if (!transitions[i].effectLoad.GetType().IsDefined(typeof(CustomDurationAttribute), false))
				{
					transitions[i].effectLoad.duration = duration;
				}

				if (!transitions[i].effectUnload.GetType().IsDefined(typeof(CustomDurationAttribute), false))
				{
					transitions[i].effectUnload.duration = duration;
				}
			}
		}

		/// <summary>
		/// Set the sleep of all TransitionEffects.(Only UnityEditor)
		/// </summary>
		/// <param name="sleep">Sleep duration transition effect.</param>
		public void SetSleepAllTransitionEffects(float sleep)
		{
			for (int i = 0; i < transitions.Count; i++)
			{
				if (!transitions[i].effectLoad.GetType().IsDefined(typeof(CustomDurationAttribute), false))
				{
					transitions[i].effectLoad.sleep = sleep;
				}

				if (!transitions[i].effectUnload.GetType().IsDefined(typeof(CustomDurationAttribute), false))
				{
					transitions[i].effectUnload.sleep = sleep;
				}
			}
		}
#endif
		/// <summary>
		/// See: <seealso cref="TransitionData"/>
		/// </summary>
		public List<TransitionData> transitions;

		/// <summary>
		/// Makes a blank controller.
		/// </summary>
		/// <returns>EmptyController</returns>
		public static ScenesManagerController CreateEmptyController()
		{
			ScenesManagerController emptyController = CreateInstance<ScenesManagerController>();
			emptyController.name = "EmptyController";

#if UNITY_EDITOR
			SceneNodeData[] empty = new SceneNodeData[2]
			{
				new SceneNodeData(GUID.Generate(),0,new Vector2(100.0f,300.0f)),
				new SceneNodeData(GUID.Generate(),1,new Vector2(400.0f,200.0f))
			};
			emptyController.nodesData = new List<ScenesManagerController.SceneNodeData>(empty);
#endif

			emptyController.transitions = new List<TransitionData>();
			return emptyController;
		}
	}
}
