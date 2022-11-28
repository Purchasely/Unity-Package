using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Purchasely.Editor
{
	public class PurchaselyPostProcessor
	{
		[PostProcessBuild]
		public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
		{
			if (buildTarget == BuildTarget.iOS)
			{
				var projPath = PBXProject.GetPBXProjectPath(buildPath);
				var proj = new PBXProject();
				proj.ReadFromFile(projPath);

				//// Configure build settings
				proj.SetBuildProperty(proj.GetUnityMainTargetGuid(), "ENABLE_BITCODE", "NO");
				proj.SetBuildProperty(proj.GetUnityFrameworkTargetGuid(), "ENABLE_BITCODE", "NO");

				proj.WriteToFile(projPath);
			}
		}
	}
}