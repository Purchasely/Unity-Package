using System.IO;
using UnityEditor;
using UnityEngine;

namespace Purchasely.Editor
{
	public class PurchaselySettingsEditor : EditorWindow
	{
		private const string SettingsPath = "Purchasely/Settings";
		private const string SettingsFullPath = "Assets/Resources/" + SettingsPath + ".asset";

		[MenuItem("Window/Purchasely")]
		public static void ShowWindow()
		{
			GetWindow(typeof(PurchaselySettingsEditor));
		}

		private void OnGUI()
		{
			var purchaselySettings = Resources.Load<PurchaselySettings>(SettingsPath);
			if (purchaselySettings == null)
			{
				purchaselySettings = CreateInstance<PurchaselySettings>();

				var parentDir = Directory.GetParent(SettingsFullPath);

				if (!parentDir.Exists)
					parentDir.Create();

				AssetDatabase.CreateAsset(purchaselySettings, SettingsFullPath);
			}

			Selection.activeObject = purchaselySettings;
			Close();
		}
	}
}