using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEditor;
using UnityEngine;

using Assembly = System.Reflection.Assembly;

namespace IRK.Unity.ScenesManager
{
    public static class SMEditorUtility
    {
        public const string pathResourceFolder = "Assets/SceneManager/Editor/Resource/";

        public static List<string> GetNameTypesInAssemblies(Type type,Assembly[] assemblies)
        {
            List<string> result = new List<string>();

            foreach (Assembly assembly in assemblies)
            {
                result.AddRange( assembly.GetTypes()
                   .Where(t => t.BaseType == type && !t.IsAbstract)
                   .Select(t => t.Name)
                   .OrderBy(e => e)
                   .ToArray());
            }

            return result;
        }

        public static List<Assembly> GetAssembliesWithType(Type type)
        {
            var assembliesResult = new List<Assembly>();

            var assembliesInProject = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies);
            var assembliesInUnityApp = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assembliesInUnityApp)
            {
                if (assembliesInProject.Any(f => f.name == item.GetName().Name) && item.GetTypes().Any(t => t.BaseType == type))
                {
                    assembliesResult.Add(item);
                }
            }

            return assembliesResult;
        }

        public static string[] GetNameScenesBuild()
        {
            string[] result = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetNameSceneBuild(i);
            }
            return result;
        }

        public static string GetNameSceneBuild(int buildIndex, bool showIndex = true)
        {
            if (CheckIsError(buildIndex))
                return string.Empty;

            string path = EditorBuildSettings.scenes[buildIndex].path;
            int startIndex = path.LastIndexOf('/') + 1;
            int endIndex = path.LastIndexOf('.');
            return path.Substring(startIndex, endIndex - startIndex) + (showIndex ? string.Format("({0})", buildIndex) : string.Empty);
        }

        public static bool CheckIsError(int buildIndex)
        {
            if(EditorBuildSettings.scenes.Length == 0)
            {
                Debug.LogError("[ScenesManager]The list of scenes in build is empty.File > BuildSettings > ScenesInBuild");
                return true;
            }

            if (EditorBuildSettings.scenes.Length <= buildIndex || buildIndex < 0)
            {
                Debug.LogErrorFormat("[ScenesManager]The build  index({0}) of this scene is not in the list of ScenesInBuild", buildIndex);
                return true;
            }

            return false;
        }
    }
}