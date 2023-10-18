#if UNITY_IOS
using System;
#endif
using System.Collections.Generic;
#if UNITY_ANDROID
using UnityEngine;
#endif

namespace PurchaselyRuntime
{
	public class Presentation
	{
		public string id;
		public string language;
		public string placementId;
		public string audienceId;
		public string abTestId;
		public string abTestVariantId;
		public string type;
		public List<PresentationPlan> plans;
		public Dictionary<string, object> metadata;
		public PresentationType presentationType = PresentationType.Unknown;

#if UNITY_ANDROID
		internal AndroidJavaObject presentationAjo;
#elif UNITY_IOS
		internal IntPtr iosPresentation;
#endif
	}
}