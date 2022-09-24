using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MultiplayersBuildAndRun
{
#if UNITY_EDITOR
    [MenuItem("Tools/Run Multiplayer/2 Players")]
    static void PerformWin64Build2()
    {
        PreformWin64Build(2);
    }
    [MenuItem("Tools/Run Multiplayer/3 Players")]
    static void PerformWin64Build3()
    {
        PreformWin64Build(3);
    }
    [MenuItem("Tools/Run Multiplayer/4 Players")]
    static void PerformWin64Build4()
    {
        PreformWin64Build(4);
    }
    static void PreformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        for(int i = 1; i <= playerCount; i++)
        {
            BuildPipeline.BuildPlayer(
                GetScenePaths(),
                "Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
                BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scene = new string[EditorBuildSettings.scenes.Length];

        for(int i = 0; i < scene.Length; i++)
        {
            scene[i] = EditorBuildSettings.scenes[i].path;
        }

        return scene;
    }
#endif
}
