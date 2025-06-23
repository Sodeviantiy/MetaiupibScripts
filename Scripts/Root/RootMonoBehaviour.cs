using IubipGame.ScriptsGame.Root.Path;
using IubipGame.ScriptsGame.Settings;
using IubipGame.ScriptsGame.Settings.Scenes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class RootMonoBehaviour : MonoBehaviour
{
    public abstract TypeRoot TypeRoot { get; }

    public abstract IEnumerator Run();

    public static TypeRoot GetSceneType(Scene currentScene)
    {
        TryGetCurrentProfile(out List<DataScene> scenes);
        foreach (var ElementInScene in scenes)
        {
            if (ElementInScene.Scene.name == currentScene.name)
                return ElementInScene.BootType;
        }

        throw new ArgumentNullException("СЦЕНА НЕ ДОБАВЛЕНА В GLOBALPROFILE");
    }

    public static bool TryGetCurrentProfile(out List<DataScene> dataScenes)
    {
        var profile = Resources.Load<LoadingScenes>(PathConst.ScenesForBuild).AllSceneProfile;
        if (profile.Any())
        {
            dataScenes = profile;
            return true;
        }

        Debug.LogError($"ERROR: СЦЕНЫ ДЛЯ БИЛДА НА УСТАНОВЛЕНЫ ПО ПУТИ {PathConst.ScenesForBuild}");

        dataScenes = null;
        return false;
    }
}
