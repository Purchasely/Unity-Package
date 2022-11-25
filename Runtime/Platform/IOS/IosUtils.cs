using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Purchasely
{
	internal static class IosUtils
	{
		public static IntPtr GetPointer(this object obj)
		{
			return obj == null ? IntPtr.Zero : GCHandle.ToIntPtr(GCHandle.Alloc(obj));
		}

		internal static T Cast<T>(this IntPtr instancePtr)
		{
			var instanceHandle = GCHandle.FromIntPtr(instancePtr);
			if (!(instanceHandle.Target is T))
			{
				throw new InvalidCastException("Failed to cast IntPtr to " + nameof(T));
			}

			var castedTarget = (T) instanceHandle.Target;
			return castedTarget;
		}

		internal delegate void VoidCallbackDelegate(IntPtr actionPtr);

		[MonoPInvokeCallback(typeof(VoidCallbackDelegate))]
		internal static void VoidCallback(IntPtr actionPtr)
		{
			if (Debug.isDebugBuild)
				Debug.Log("VoidCallback");

			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action>();
				action();
			}
		}

		internal delegate void StartCallbackDelegate(IntPtr actionPtr, bool success, string error);

		[MonoPInvokeCallback(typeof(StartCallbackDelegate))]
		internal static void StartCallback(IntPtr actionPtr, bool success, string error)
		{
			if (Debug.isDebugBuild)
				Debug.Log("StartCallback");

			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<bool, string>>();
				action(success, error);
			}
		}

		internal delegate void BoolCallbackDelegate(IntPtr actionPtr, bool result);

		[MonoPInvokeCallback(typeof(BoolCallbackDelegate))]
		internal static void BoolCallback(IntPtr actionPtr, bool result)
		{
			if (Debug.isDebugBuild)
				Debug.Log("BoolCallback");

			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<bool>>();
				action(result);
			}
		}

		internal delegate void EventCallbackDelegate(IntPtr actionPtr, string name, string propertiesJson);

		[MonoPInvokeCallback(typeof(EventCallbackDelegate))]
		internal static void EventCallback(IntPtr actionPtr, string name, string propertiesJson)
		{
			if (Debug.isDebugBuild)
				Debug.Log("EventCallback");

			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<string, string>>();
				action(name, propertiesJson);
			}
		}

		internal delegate void PresentationResultCallbackDelegate(IntPtr actionPtr, int result, IntPtr planPointer);

		[MonoPInvokeCallback(typeof(PresentationResultCallbackDelegate))]
		internal static void PresentationResultCallback(IntPtr actionPtr, int result, IntPtr planPointer)
		{
			if (Debug.isDebugBuild)
				Debug.Log("PresentationResultCallback");

			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<int, IntPtr>>();
				action(result, planPointer);
			}
		}
	}
}