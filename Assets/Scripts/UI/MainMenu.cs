using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // menu buttons
    public void StartGame()
    {
        SceneLoader.ChangeScene(SceneLoader.Scene.Gameplay);
    }
    public void Options()
    {

    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
