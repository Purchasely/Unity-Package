using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

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
				var project = new PBXProject();
				project.ReadFromFile(projPath);

				var targetGuid = project.GetUnityFrameworkTargetGuid();

				//// Configure build settings
				project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

				var frameworkPath = Path.Combine("com.purchasely.unity/Native/IOS", "Purchasely.framework");
				var fileGuid = project.AddFile(frameworkPath, Path.Combine("Frameworks", frameworkPath), PBXSourceTree.Sdk);
				project.AddFileToEmbedFrameworks(targetGuid, fileGuid);
				
				project.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks $(PROJECT_DIR)/lib/$(CONFIGURATION) $(inherited)");
				project.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(inherited) $(PROJECT_DIR) $(PROJECT_DIR)/Frameworks");
				project.AddBuildProperty(targetGuid, "DYLIB_INSTALL_NAME_BASE", "@rpath");
				project.AddBuildProperty(targetGuid, "LD_DYLIB_INSTALL_NAME", "@executable_path/../Frameworks/$(EXECUTABLE_PATH)");

				project.WriteToFile(projPath);
			}
		}
	}
}