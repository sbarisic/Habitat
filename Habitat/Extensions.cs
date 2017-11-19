using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Habitat {
	static class Extensions {
		public static T Value<K, T>(this Dictionary<K, object> Dict, K Key) {
			return (T)Dict[Key];
		}

		public static T Value<T>(this Dictionary<string,object>Dict, string Key) {
			return Dict.Value<string, T>(Key);
		}
	}
}
