using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace IRK.Unity.ScenesManager
{
	public enum SceneNodePortType
	{
		Exit = 1,
		Load = 2,
		Unload = -2,
	}

	public class SceneNodePort : Port
	{
		private class TransitionEdgeConnectorListener : IEdgeConnectorListener
		{
			GraphViewChange _graphViewChange;
			List<Edge> _edgesToCreate;
			List<GraphElement> _edgesToDelete;

			public TransitionEdgeConnectorListener()
			{
				_edgesToCreate = new List<Edge>();
				_edgesToDelete = new List<GraphElement>();
				
				_graphViewChange.edgesToCreate = _edgesToCreate;
			}
		
			public void OnDrop(GraphView graphView, Edge edge)
			{
				_edgesToCreate.Clear();
				_edgesToCreate.Add(edge);

				_edgesToDelete.Clear();

				TransitionEdge trEdge = (TransitionEdge)edge;

				foreach (Edge e in edge.input.connections)
				{
					if (edge.input.capacity == Capacity.Single && e != edge)
						_edgesToDelete.Add(e);

					if (trEdge.Equals((TransitionEdge)e))
					{
						return;
					}
				}

				foreach (Edge e in edge.output.connections)
				{
					if (edge.output.capacity == Capacity.Single && e != edge)
						_edgesToDelete.Add(e);
				}

				
				if (_edgesToDelete.Count > 0)
					graphView.DeleteElements(_edgesToDelete);

				if(graphView.graphViewChanged != null)
				{
					_edgesToCreate = graphView.graphViewChanged(_graphViewChange).edgesToCreate;
				}

				foreach (Edge e in _edgesToCreate)
				{
					TransitionEdge transitionEdge = ((TransitionEdge)e);
					graphView.AddElement(transitionEdge);
					edge.input.Connect(transitionEdge);
					edge.output.Connect(transitionEdge);
					transitionEdge.Init();
				}
			}

			public void OnDropOutsidePort(Edge edge, Vector2 position)
			{
			}
		}

		private SceneNodePortType _portType;

		public SceneNodePortType sceneNodePortType { get { return _portType; } }

		public SceneNodePort(SceneNodePortType type) : base(
			Orientation.Horizontal,
			((int)type) > 0 ? Direction.Input : Direction.Output,
			Capacity.Multi,
			typeof(int))
		{
			TransitionEdgeConnectorListener connectorListener = new TransitionEdgeConnectorListener();
			m_EdgeConnector = new EdgeConnector<TransitionEdge>(connectorListener);

			this.AddManipulator(m_EdgeConnector);

			portName = type.ToString();

			switch (type)
			{
				case SceneNodePortType.Exit:
					portColor = new Color(0.5f, 0.5f, 0.5f);
					break;
				case SceneNodePortType.Load:
					portColor = Color.green;
					break;
				case SceneNodePortType.Unload:
					portColor = Color.red;
					break;
			}

			_portType = type;
		}

		public override void Connect(Edge edge)
		{
			base.Connect(edge);
		}
	}
}
