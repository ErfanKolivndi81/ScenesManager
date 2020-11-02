using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IRK.Unity.ScenesManager
{
	public static class ScenesManagerUtility 
	{
		public static void StretchSize(this RectTransform rectTr)
		{
			rectTr.position = Vector2.zero;
			rectTr.anchorMin = Vector2.zero;
			rectTr.anchorMax = Vector2.one;
			rectTr.pivot = Vector2.one / 2.0f;
			rectTr.sizeDelta = Vector2.zero;
			rectTr.anchoredPosition = Vector2.zero;
		}

		public static void StretchSize(this UnityEngine.UI.Image img)
		{
			img.rectTransform.StretchSize();
		}

		/// <summary>
		/// Swap the location of the x and y components
		/// </summary>
		public static Vector2 SwapXYVector2(Vector2 main)
		{
			return new Vector2()
			{
				x = main.y,
				y = main.x,
			};
		}

		/// <summary>
		/// Swap the location of the x and y components
		/// </summary>
		public static Vector2 SwapXY(this Vector2 vec)
		{
			return SwapXYVector2(vec);
		}

		/// <summary>
		/// Get build index scene by name scene.Returns -1 if not available.
		/// </summary>
		/// <param name="name">Name scene</param>
		/// <returns>Build index scene</returns>
		public static int GetBuildIndexScene(string name)
		{
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				//Get the Scene name from Scene path
				string nameScene = GetNameFromPathScene(SceneUtility.GetScenePathByBuildIndex(i));
				if(nameScene == name)
					return i;
			}

			return -1;
		}

		public static string GetSceneName(int buildIndex)
		{
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				if (i == buildIndex)
					return GetNameFromPathScene(SceneUtility.GetScenePathByBuildIndex(i));
				
			}

			return string.Empty;
		}

		public static string GetNameFromPathScene(string path)
		{
			int startIndex = path.LastIndexOf('/') + 1;
			int endIndex = path.LastIndexOf('.');
			return path.Substring(startIndex, endIndex - startIndex);
		}
	}
}
