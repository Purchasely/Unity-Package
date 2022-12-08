using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_IOS
using System.IO;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

namespace Purchasely.Editor
{
	public class PurchaselyPostProcessor : IPostGenerateGradleAndroidProject
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

		public int callbackOrder => 999;

		public void OnPostGenerateGradleAndroidProject(string path)
		{
			AddPurchaselyDependencies(path);
			AddUseAndroidX(path);
		}

		private void AddPurchaselyDependencies(string builtProjectPath)
		{
			const string ERROR_MESSAGE = "Could not add Purchasely dependencies to build.gradle file.";

			var buildGradleFilePath = Path.Combine(builtProjectPath, "build.gradle");
			if (File.Exists(buildGradleFilePath))
			{
				try
				{
					var buildGradleText = File.ReadAllText(buildGradleFilePath);

					const string DEPENDENCIES = "dependencies {";
					var dependenciesIndex = buildGradleText.IndexOf(DEPENDENCIES, StringComparison.InvariantCulture);
					if (dependenciesIndex >= 0)
					{
						buildGradleText = buildGradleText.Insert(dependenciesIndex + DEPENDENCIES.Length,
							"\n\timplementation \'io.purchasely:core:1.1.0-alpha02\'\n\timplementation \'io.purchasely:player:1.1.0-alpha02\'\n\timplementation \'io.purchasely:google-play:1.1.0-alpha02\'\n");
						File.WriteAllText(buildGradleFilePath, buildGradleText);
						Debug.Log("Purchasely dependencies were successfully added to build.gradle file.");
					}
					else
					{
						Debug.LogError(ERROR_MESSAGE);
					}
				}
				catch (Exception e)
				{
					Debug.LogError(ERROR_MESSAGE);
					Debug.LogException(e);
				}
			}
			else
			{
				Debug.LogError(ERROR_MESSAGE);
			}
		}

		private void AddUseAndroidX(string builtProjectPath)
		{
			const string ERROR_MESSAGE = "Could not add UseAndroidX to gradle.properties file.";

			var gradlePropertiesPath = builtProjectPath.Replace("unityLibrary", "gradle.properties");
			if (File.Exists(gradlePropertiesPath))
			{
				try
				{
					var gradlePropertiesLines = File.ReadAllLines(gradlePropertiesPath).ToList();

					const string USE_ANDROID_X_TAG = "android.useAndroidX";
					const string USE_ANDROID_X_TAG_WITH_VALUE = "android.useAndroidX=true";

					var lineIndex = -1;
					for (var i = 0; i < gradlePropertiesLines.Count; i++)
					{
						var line = gradlePropertiesLines[i];
						if (line.Contains(USE_ANDROID_X_TAG))
							lineIndex = i;
					}

					if (lineIndex == -1)
						gradlePropertiesLines.Insert(0, USE_ANDROID_X_TAG_WITH_VALUE);
					else
						gradlePropertiesLines[lineIndex] = USE_ANDROID_X_TAG_WITH_VALUE;

					File.WriteAllLines(gradlePropertiesPath, gradlePropertiesLines);

					Debug.Log("UseAndroidX was successfully added to gradle.properties file.");
				}
				catch (Exception e)
				{
					Debug.LogError(ERROR_MESSAGE);
					Debug.LogException(e);
				}
			}
			else
			{
				Debug.LogError(ERROR_MESSAGE);
			}
		}
	}
}