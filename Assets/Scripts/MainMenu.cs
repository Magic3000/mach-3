using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject quitGame;
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void ShowLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }
    public void ShowAbout()
    {
        SceneManager.LoadScene("About");
    }
    public void Quit()
    {
        mainMenu.SetActive(false);
        quitGame.SetActive(true);
    }
    public void QuitGame(int option)
    {
        if (option == 0)
        {
            mainMenu.SetActive(true);
            quitGame.SetActive(false);
        }
        else
        {
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call<bool>("moveTaskToBack", true);
        }
    }
}
