//-----------------------------------------------------------------------------
// Copyright 2012-2021 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX) && UNITY_2019_1_OR_NEWER

using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace RenderHeads.Media.AVProVideo.Editor
{
	public class PostProcessBuild_macOS
	{
		[PostProcessBuild]
		public static void ModifyProject(BuildTarget target, string path)
		{
			if (target != BuildTarget.StandaloneOSX)
				return;

#if AVPROVIDEO_SUPPORT_MACOSX_10_14_3_AND_OLDER
			string projectPath = path + "/project.pbxproj";
			if (!File.Exists(projectPath))
			{
				Debug.LogWarningFormat("Cannot find Xcode project with path: {0}", projectPath);
				return;
			}

			Debug.LogFormat("Reading Xcode project at: {0}", projectPath);
			PBXProject project = new PBXProject();
			project.ReadFromFile(projectPath);

			string targetGuid = project.TargetGuidByName(Application.productName);
			if (targetGuid == null)
			{
				Debug.LogWarning("Unable to locate target in the project file.\n");
				Debug.Log("You will need to manually set \"Always Embed Swift Standard Libraries\" to \"YES\" if you're targetting macOS versions prior to 10.14.4");
				return;
			}

			Debug.LogFormat("Setting build property \"Always Embed Swift Standard Libraries\" to \"YES\"");
			project.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

			Debug.LogFormat("Writing Xcode project");
			project.WriteToFile(projectPath);

			Debug.Log("Done");
#endif
		}
	}
}

#endif
