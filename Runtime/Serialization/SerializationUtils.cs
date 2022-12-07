using System;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

namespace PurchaselyRuntime
{
	public static class SerializationUtils
	{
		internal static T Deserialize<T>(string text) where T : class, new()
		{
			if (text == null)
				return new T();

			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore
			};

			try
			{
				return JsonConvert.DeserializeObject<T>(text, settings);
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
				return new T();
			}
		}

		internal static string Serialize(object obj)
		{
			if (obj == null)
				return string.Empty;

			var settings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
			};

			return JsonConvert.SerializeObject(obj, settings);
		}

		public static string ToIso8601Format(this DateTime dateTime)
		{
			return dateTime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ssZ");
		}

		public static DateTime FromIso8601Format(this string dateTimeString)
		{
			if (!DateTime.TryParse(dateTimeString, null, DateTimeStyles.RoundtripKind, out var result))
				return DateTime.MinValue;

			return result;
		}
	}
}