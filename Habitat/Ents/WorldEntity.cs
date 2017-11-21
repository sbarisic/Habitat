using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;

namespace Habitat.Ents {
	class WorldEntity : Entity {
		Dictionary<string, string> Properties = new Dictionary<string, string>();

		public string EntityName;
		public string EntityID;
		public float Width, Height;

		public WorldEntity() {
			EntityID = Rand.Int(1000000, 9999999).ToString();
		}

		public void SetProperty(string Name, string Value) {
			if (!Properties.ContainsKey(Name))
				Properties.Add(Name, Value);
			else
				Properties[Name] = Value;
		}

		public T GetProperty<T>(string Name) {
			if (typeof(T) == typeof(string))
				return (T)(object)Properties[Name];
			else if (typeof(T) == typeof(float))
				return (T)(object)float.Parse(Properties[Name], CultureInfo.InvariantCulture);
			else if (typeof(T) == typeof(Color)) {
				string Clr = Properties[Name].Substring(1);
				Clr = Clr.Substring(2) + Clr.Substring(0, 2);
				return (T)(object)new Color(Clr);
			} else if (typeof(T) == typeof(bool))
				return (T)(object)(Properties[Name].ToLower() == "true");
			else throw new Exception("Unsupported type " + typeof(T));
		}

		public T GetPropertyOrDefault<T>(string Name, T Default) {
			if (Properties.ContainsKey(Name))
				return GetProperty<T>(Name);

			return Default;
		}

		public virtual void Use() {
			GCon.WriteLine("USED: {0}", ToString());
		}

		public override string ToString() {
			string Pos = string.Format("({0}, {1})", X, Y);
			string Props = "[" + string.Join(", ", Properties.Select((KV) => string.Format("{0} = `{1}´", KV.Key, KV.Value))) + "]";
			return string.Format("{0} - {1}{2}{3}", EntityID, EntityName, Pos, Props);
		}
	}
}
