using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    public enum Scene
    {
        MainMenu,
        Gameplay,
        Loading,
    }

    private static Action onLoaderCallback;

    public static void ChangeScene(Scene targetScene)
    {
        // set loader callback to load target scene
        onLoaderCallback = () =>
        {
            SceneManager.LoadScene(targetScene.ToString());
        };

        // load the loading screen
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallback()
    {
        // trigger loader callback (waits for next update so loadingScene shows up visually before target scene is done loading)
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
