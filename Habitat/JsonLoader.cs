using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Habitat {
	static class JsonLoader {
		static JavaScriptSerializer Serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };

		public static object Deserialize(string JsonString) {
			return Serializer.DeserializeObject(JsonString);
		}

		public static T Deserialize<T>(string JsonString) {
			return Serializer.Deserialize<T>(JsonString);
		}
	}
}