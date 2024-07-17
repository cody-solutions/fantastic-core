using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using Newtonsoft.Json.Linq;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime;
using FantasticCore.Runtime.Debug;

namespace FantasticCore.Editor.Package
{
    internal static class FantasticDependenciesSyncer
    {
        [MenuItem("FantasticCore/Dependencies/Sync")]
        public static async void SyncDependencies()
        {
            EditorUtility.DisplayProgressBar(FantasticProperties.PackageName,
                "Sync dependencies...", 0.5f);
            List<string> dependencies = await GetDependencies();
            EditorUtility.ClearProgressBar();
            InstallDependencies(dependencies);
            FantasticDebug.Logger.LogMessage("Fantastic dependencies was synced successfully!");
        }

        private static async UniTask<List<string>> GetDependencies()
        {
            var dependencies = new List<string>();
            try
            {
                if (await FantasticInstance.IsFantasticEditorClientMode())
                {
                    ListRequest listRequest = Client.List();
                    while (!listRequest.IsCompleted)
                    {
                        await UniTask.Delay(100);
                    }

                    if (listRequest.Status != StatusCode.Success)
                    {
                        throw new Exception("Failed resolve packages... Please try again!");
                    }

                    foreach (PackageInfo package in listRequest.Result)
                    {
                        if (!package.name.Equals("com.west_tech.fantastic.core"))
                        {
                            continue;
                        }

                        dependencies.AddRange(package.dependencies
                            .Select(dependency => $"{dependency.name}@{dependency.version}"));
                        break;
                    }
                }
                else
                {
                    string packageJsonPath = GetFantasticPackageJsonPath();
                    JObject json = JObject.Parse(await File.ReadAllTextAsync(packageJsonPath));
                    var jsonDependencies = (JObject)json["dependencies"];
                    foreach (KeyValuePair<string, JToken> dependency in jsonDependencies!)
                    {
                        dependencies.Add($"{dependency.Key}@{dependency.Value}");
                    }   
                }
            }
            catch (Exception exception)
            {
                FantasticDebug.Logger.LogMessage($"Get dependencies failed! Exception: {exception}", FantasticLogType.ERROR);
            }
            
            return dependencies;
        }

        private static string GetFantasticPackageJsonPath()
        {
            string projectFolder = Path.GetFullPath(Path.Combine(Application.dataPath, "../"));
            return $"{projectFolder}/Assets/FantasticCore/package.json";
        }

        private static void InstallDependencies(List<string> dependencies)
        {
            if (dependencies is null or { Count: 0 })
            {
                FantasticDebug.Logger.LogMessage("Dependencies is empty or not valid!", FantasticLogType.WARN);
                return;
            }

            foreach (string dependency in dependencies)
            {
                FantasticDebug.Logger.LogMessage($"Checking {dependency} dependency...");
            }

            Client.AddAndRemove(dependencies.ToArray());
        }
    }
}