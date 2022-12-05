using System.IO;
using PurchaselyRuntime;
using UnityEditor;
using UnityEngine;

namespace Purchasely.Editor
{
	public class PurchaselySettingsEditor : EditorWindow
	{
		

		[MenuItem("Window/Purchasely")]
		public static void ShowWindow()
		{
			GetWindow(typeof(PurchaselySettingsEditor));
		}

		private void OnGUI()
		{
			var purchaselySettings = Resources.Load<Settings>(PurchaselyRuntime.Purchasely.SettingsPath);
			if (purchaselySettings == null)
			{
				purchaselySettings = CreateInstance<Settings>();

				var parentDir = Directory.GetParent(PurchaselyRuntime.Purchasely.SettingsFullPath);

				if (!parentDir.Exists)
					parentDir.Create();

				AssetDatabase.CreateAsset(purchaselySettings, PurchaselyRuntime.Purchasely.SettingsFullPath);
			}

			Selection.activeObject = purchaselySettings;
			Close();
		}
	}
}