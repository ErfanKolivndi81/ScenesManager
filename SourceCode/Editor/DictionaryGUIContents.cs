using Texture = UnityEngine.Texture;
using Content = UnityEngine.GUIContent;

namespace IRK.Unity.ScenesManager
{
	public class DC : System.Collections.Generic.Dictionary<string, Content>
	{ 
		public DC() : base()
		{

		}

		public void Add(string key,string text,string tooltip,Texture image)
		{
			Add(key, new Content(text, image,tooltip));
		}

		public void Add(string key, string text, string tooltip)
		{
			Add(key, new Content(text, tooltip));
		}

		public void Add(string key, string text, Texture image)
		{
			Add(key, new Content(text, image));
		}

		public void Add(string key, Texture image,string tooltip)
		{
			Add(key, new Content(image, tooltip));
		}

		public void Add(string key, string text)
		{
			base.Add(key, new Content(text));
		}

		public void Add(string key,Texture image)
		{
			base.Add(key, new Content(image));
		}
	}
}
