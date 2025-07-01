using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class MainMenu : MonoBehaviour
{

    public void NewGame()
    {
        SaveManager.Instance.DelayedNewGame();

    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void LoadGame()
    {

        SaveManager.Instance.DelayedLoadGame();
    }




}
