using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class About : MonoBehaviour
{
    public void OpenTG()
    {
        Process.Start("tg://magic3000");
    }
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
