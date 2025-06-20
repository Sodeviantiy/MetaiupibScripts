using IubipGame.ScriptsGame.Root;
using IubipGame.ScriptsGame.Root.Path;
using IubipGame.ScriptsGame.Settings;
using IubipGame.ScriptsGame.Settings.Scenes;
using IubipGame.ScriptsGame.Utility;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
public class RootGame
{
    public static RootGame Instance { get; private set; }

    private static UILoadingView _uiLoading;
    public static CoroutineUtility Coroutine { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PreInit()
    {
        Instance = new RootGame();
        Instance.InitScene();

#if DEBUG || UNITY_EDITOR
        foreach (var UserDebug in ScriptableGlobalService.DebugUsers)
        {
            DebugUtility.DebugGizmos += UserDebug.DebugOnGizmos;
        }
#endif
    }
    private RootGame()
    {
        Coroutine = new GameObject("[Coroutine]").AddComponent<CoroutineUtility>();
        Object.DontDestroyOnLoad(Coroutine.gameObject);

        var uiLoadingPrefab = Resources.Load<UILoadingView>(PathConst.UI + "UILoading");
        _uiLoading = Object.Instantiate(uiLoadingPrefab);
        Object.DontDestroyOnLoad(_uiLoading.gameObject);
    }

    private void InitScene()
    {
#if UNITY_EDITOR

        if (RootMonoBehaviour.TryGetCurrentProfile(out _))
        {
            var typeCurrentScene = RootMonoBehaviour.GetSceneType(SceneManager.GetActiveScene());

            switch (typeCurrentScene)
            {
                case TypeRoot.Gameplay:
                    Coroutine.StartCoroutine(LoadAndStartGameplay());
                    break;
                case TypeRoot.Menu:
                    Coroutine.StartCoroutine(LoadAndStartMenu());
                    break;
            }

            return;
        }

        Debug.LogError("НЕТ ПРОФИЛЯ ДЛЯ СЦЕН В ПАПКИ:  ProfileScene в Resources c названия GlobalProfile");
#endif
        Coroutine.StartCoroutine(LoadAndStartGameplay());
    }

    private IEnumerator LoadAndStartGameplay()
    {
        _uiLoading.ShowLoadingScreen();

        yield return LoadSceneRoot();
        yield return LoadScene(TypeRoot.Gameplay);

        yield return new WaitForSeconds(1);

        var sceneEntryPoint = Object.FindFirstObjectByType<RootMonoBehaviour>();
        yield return Coroutine.StartCoroutine(sceneEntryPoint.Run());

        _uiLoading.HideLoadingScreen();

    }

    private IEnumerator LoadAndStartMenu()
    {
        _uiLoading.ShowLoadingScreen();

        yield return LoadSceneRoot();
        yield return LoadScene(TypeRoot.Menu);

        yield return new WaitForSeconds(1);

        var sceneEntryPoint = Object.FindFirstObjectByType<RootMonoBehaviour>();
        yield return Coroutine.StartCoroutine(sceneEntryPoint.Run());

        _uiLoading.HideLoadingScreen();
    }

    private IEnumerator LoadScene(TypeRoot typeRoot)
    {
        //TODO: сделать глобальные выборщик профилей
        var scenesData = Resources.Load<LoadingScenes>(PathConst.ScenesForBuild).AllSceneProfile;

        if (!scenesData.Any())
            throw new ArgumentNullException($"НЕ УСТАНОВЛЕНЫ СЦЕНА ДЛЯ БИЛДА ПРОВЕРТИ ПУТЬ: {PathConst.ScenesForBuild}");

        var sceneInProfile = scenesData.Find((scene => scene.BootType == typeRoot));

        yield return SceneManager.LoadSceneAsync(sceneInProfile.Scene.name);
    }

    private IEnumerator LoadSceneRoot()
    {
        var sceneRoot = SceneManager.CreateScene("Root");
        yield return SceneManager.SetActiveScene(sceneRoot);
    }
}
