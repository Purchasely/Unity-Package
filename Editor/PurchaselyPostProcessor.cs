using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using System.IO;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

namespace Purchasely.Editor
{
	public class PurchaselyPostProcessor
	{
		[PostProcessBuild]
		public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
		{
			if (buildTarget == BuildTarget.iOS)
			{
#if UNITY_IOS
				var projPath = PBXProject.GetPBXProjectPath(buildPath);
				var project = new PBXProject();
				project.ReadFromFile(projPath);

				var unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
				var targetGuid = project.GetUnityMainTargetGuid();

				//// Configure build settings
				project.SetBuildProperty(unityFrameworkTargetGuid, "ENABLE_BITCODE", "NO");
				project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

				var frameworkPath = Path.Combine("com.purchasely.unity/Native/IOS", "Purchasely.framework");
				var fileGuid = project.AddFile(frameworkPath, Path.Combine("Frameworks", frameworkPath));
				project.AddFileToEmbedFrameworks(targetGuid, fileGuid);
				project.AddFileToEmbedFrameworks(unityFrameworkTargetGuid, fileGuid);
				
				project.AddBuildProperty(unityFrameworkTargetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks $(PROJECT_DIR)/lib/$(CONFIGURATION) $(inherited)");
				project.AddBuildProperty(unityFrameworkTargetGuid, "FRAMEWORK_SEARCH_PATHS", "$(inherited) $(PROJECT_DIR) $(PROJECT_DIR)/Frameworks");
				project.AddBuildProperty(unityFrameworkTargetGuid, "DYLIB_INSTALL_NAME_BASE", "@rpath");
				project.AddBuildProperty(unityFrameworkTargetGuid, "LD_DYLIB_INSTALL_NAME", "@executable_path/../Frameworks/$(EXECUTABLE_PATH)");

				project.WriteToFile(projPath);
#endif
			}
		}
	}
}