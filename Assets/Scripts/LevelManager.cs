using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public bool levelToggled;

    public void toggleWorld()
    {
        Debug.Log("Q pressed");
        if (!levelToggled)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            levelToggled = true;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            levelToggled = false;
        }
    }
}
