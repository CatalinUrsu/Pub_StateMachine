using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace StateMachine.Editor
{
[InitializeOnLoad]
public static class GitPackagesResolver
{
    static readonly string _manifestPath = "Packages/manifest.json";
    static readonly string _unitaskPackageName = "com.cysharp.unitask";
    static readonly string _unitaskPackageUrl = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";

    static GitPackagesResolver()
    {
        if (!TryGetManifest(out var manifest)) return;
        if (!TryGetDependencies(manifest, out var dependencies)) return;

        AddPackageIfNotExists(manifest, dependencies, _unitaskPackageName, _unitaskPackageUrl);
        AssetDatabase.Refresh();
    }

    static void AddPackageIfNotExists(JObject manifest, JObject dependencies, string package, string url)
    {
        if (dependencies.ContainsKey(package)) return;
        
        dependencies[package] = url;
        File.WriteAllText(_manifestPath, manifest.ToString());
        Debug.Log($"Added package {package} to manifest.json.");
    }

    static bool TryGetManifest(out JObject manifest)
    {
        manifest = null;
        if (!File.Exists(_manifestPath))
        {
            Debug.LogError("manifest.json not found at: " + _manifestPath);
            return false;
        }

        manifest = JObject.Parse(File.ReadAllText(_manifestPath));
        return true;
    }

    static bool TryGetDependencies(JObject manifest, out JObject dependencies)
    {
        dependencies = manifest["dependencies"] as JObject;
        if (dependencies == null)
        {
            Debug.LogError("Dependencies not found in manifest.json");
            return false;
        }

        return true;
    }
}
}