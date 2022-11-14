using System.IO;
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
			var purchaselySettings = Resources.Load<PurchaselySettings>(Purchasely.SettingsPath);
			if (purchaselySettings == null)
			{
				purchaselySettings = CreateInstance<PurchaselySettings>();

				var parentDir = Directory.GetParent(Purchasely.SettingsFullPath);

				if (!parentDir.Exists)
					parentDir.Create();

				AssetDatabase.CreateAsset(purchaselySettings, Purchasely.SettingsFullPath);
			}

			Selection.activeObject = purchaselySettings;
			Close();
		}
	}
}