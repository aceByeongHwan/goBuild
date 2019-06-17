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

class Unity3dBuilder1
{
    static string[] SCENES = FindEnabledEditorScenes();
    /*
    [MenuItem("Build/Mac/DEV")]
    static void PerformMacBuildDEV()
    {
        BuildOptions opt = BuildOptions.None;

        PlayerSettings.productName = "CANDY_CRUSH DEV";

        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1024;
        PlayerSettings.defaultScreenHeight = 768;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.runInBackground = true;
        PlayerSettings.forceSingleInstance = true;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "DEV_SERVER");

        string BUILD_TARGET_PATH = "Build/Mac/DEV";
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        GenericBuild(SCENES, BUILD_TARGET_PATH + "/CANDY_CRUSH", BuildTarget.StandaloneOSX, opt);
    }

    [MenuItem("Build/Mac/QA")]
    static void PerformMacBuildQA()
    {
        BuildOptions opt = BuildOptions.None;

        PlayerSettings.productName = "CANDY_CRUSH QA";

        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1024;
        PlayerSettings.defaultScreenHeight = 768;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.runInBackground = true;
        PlayerSettings.forceSingleInstance = true;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "QA_SERVER");

        string BUILD_TARGET_PATH = "Build/Mac/QA";
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        GenericBuild(SCENES, BUILD_TARGET_PATH + "/CANDY_CRUSH", BuildTarget.StandaloneOSX, opt);
    }

    [MenuItem("Build/iOS/Simulator/DEV")]
    static void PerformIOSBuildSimulator()
    {
        BuildOptions opt = BuildOptions.None;
        PlayerSettings.renderingPath = RenderingPath.VertexLit;

        PlayerSettings.productName = "CANDY_CRUSH IOS_SIMULATOR_DEV";
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true; 
        //PlayerSettings.iPhoneBundleIdentifier = "candy_crush.ios_simulator";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.test.candycrush-ios-dev");

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2);
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
        PlayerSettings.iOS.appleEnableAutomaticSigning = false;
        PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Custom;
        PlayerSettings.iOS.backgroundModes = iOSBackgroundMode.Fetch | iOSBackgroundMode.RemoteNotification;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "IOS_SIMULATOR_DEV");
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2, UnityEngine.Rendering.GraphicsDeviceType.Metal });

        string BUILD_TARGET_PATH = "Build/iOS/Simulator/DEV";
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        GenericBuild(SCENES, BUILD_TARGET_PATH + "/CANDY_CRUSH", BuildTarget.iOS, opt);
    }

    [MenuItem("Build/iOS/Simulator/QA")]
    static void PerformIOSBuildSimulatorQA()
    {
        BuildOptions opt = BuildOptions.None;

        PlayerSettings.productName = "CANDY_CRUSH IOS_SIMULATOR_QA";
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        //PlayerSettings.iPhoneBundleIdentifier = "candy_crush.ios_simulator";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.test.candycrush-ios-qa");

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2);
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
        PlayerSettings.iOS.appleEnableAutomaticSigning = false;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "IOS_SIMULATOR_QA");
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2, UnityEngine.Rendering.GraphicsDeviceType.Metal });

        string BUILD_TARGET_PATH = "Build/iOS/Simulator/QA";
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        GenericBuild(SCENES, BUILD_TARGET_PATH + "/CANDY_CRUSH", BuildTarget.iOS, opt);
    }

    [MenuItem("Build/android/internal")]
    static void PerformAndroidBuildInternal()
    {
        BuildOptions opt = BuildOptions.None;

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;  

        PlayerSettings.productName = "CANDY_CRUSH ANDROID_INTERNAL";
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.test.candycrushAndroidInternal");

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel27;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "ANDROID_INTERNAL");
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2, UnityEngine.Rendering.GraphicsDeviceType.Metal });

        string BUILD_TARGET_PATH = "Build/android/script/internal";
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        GenericBuild(SCENES, BUILD_TARGET_PATH + "/CANDY_CRUSH.apk", BuildTarget.Android, opt);
    }


    [MenuItem("Build/android/gradle")]
    static void PerformAndroidBuildGradle()
    {
        BuildOptions opt = BuildOptions.None;

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;    
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;  

        PlayerSettings.productName = "CANDY_CRUSH_ANDROID_GRADLE";
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.test.candycrushAndroidQA");

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel16;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel27;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "ANDROID_GRADLE");
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2, UnityEngine.Rendering.GraphicsDeviceType.Metal });

        string BUILD_TARGET_PATH = "Build/android/script/gradle64";
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        GenericBuild(SCENES, BUILD_TARGET_PATH + "/CANDY_CRUSH", BuildTarget.Android, opt);
    }
    */

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
