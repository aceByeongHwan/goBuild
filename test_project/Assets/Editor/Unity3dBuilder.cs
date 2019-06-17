using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

//BuildOptions opt = BuildOptions.SymlinkLibraries |
//                  BuildOptions.Development |
//                  BuildOptions.ConnectWithProfiler |
//                  BuildOptions.AllowDebugging |
//                  BuildOptions.Development |
//                  BuildOptions.AcceptExternalModificationsToPlayer;

class Unity3dBuilder
{
    static string[] SCENES = FindEnabledEditorScenes();

    [MenuItem("Build/Mac")]
    static void PerformMacBuild()
    {
        BuildOptions opt = BuildOptions.None;

        PlayerSettings.productName = "CANDY_CRUSH";

        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1024;
        PlayerSettings.defaultScreenHeight = 768;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.runInBackground = true;
        PlayerSettings.forceSingleInstance = true;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "MacOS");

        string BUILD_TARGET_PATH = "Build/Mac";
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        GenericBuild(SCENES, BUILD_TARGET_PATH + "/CANDY_CRUSH", BuildTarget.StandaloneOSX, opt);
    }

    [MenuItem("Build/iOS")]
    static void PerformIOSBuild()
    {
        BuildOptions opt = BuildOptions.AcceptExternalModificationsToPlayer;

        PlayerSettings.productName = "CANDY_CRUSH";
        PlayerSettings.iPhoneBundleIdentifier = "com.test.candycrushIOS";

        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.test.candycrushIOS");

        //xcode setting
        PlayerSettings.iOS.appleEnableAutomaticSigning = false;
        PlayerSettings.iOS.appleDeveloperTeamID = "9EN6WKMF8R";
        PlayerSettings.iOS.iOSManualProvisioningProfileID = "afe79f64-2ec4-4788-93a9-e044d7a70b13";
        PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Development;

        PlayerSettings.stripEngineCode = false;
        PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Custom;
        PlayerSettings.iOS.backgroundModes = iOSBackgroundMode.Fetch | iOSBackgroundMode.RemoteNotification;

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2);
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.iOS.appleEnableAutomaticSigning = false;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "IOS");
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2, UnityEngine.Rendering.GraphicsDeviceType.Metal });

        string BUILD_TARGET_PATH = "Build/iOS/Xcode";
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        GenericBuild(SCENES, BUILD_TARGET_PATH + "/CANDY_CRUSH", BuildTarget.iOS, opt);
    }

    [MenuItem("Build/android")]
    static void PerformAndroidBuild()
    {
        BuildOptions opt = BuildOptions.None;

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;    
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;  

        PlayerSettings.productName = "CANDY_CRUSH";
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.test.candycrushAndroid");

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel27;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "android");
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2, UnityEngine.Rendering.GraphicsDeviceType.Metal });

        string BUILD_TARGET_PATH = "Build/android";
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        GenericBuild(SCENES, BUILD_TARGET_PATH + "/CANDY_CRUSH", BuildTarget.Android, opt);
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
            {
                continue;
            }

            EditorScenes.Add(scene.path);
        }

        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_filename, BuildTarget build_target, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);

#if UNITY_2018
        UnityEditor.Build.Reporting.BuildReport res = BuildPipeline.BuildPlayer(scenes, target_filename, build_target, build_options);
        if (res.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            throw new Exception("BuildPlayer failure: " + res.ToString());
        }
#else
        string res = BuildPipeline.BuildPlayer(scenes, target_filename, build_target, build_options).ToString();
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
#endif
    }
}
