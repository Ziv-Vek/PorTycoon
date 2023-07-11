using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavingWrapper: MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            Debug.Log("saving scene");
            GameManager.Instance.SaveData();
        }

        if (Input.GetKeyDown("l"))
        {
            Debug.Log("reloading scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown("q"))
        {
            Debug.Log("deleted PlayerPrefs('boxes')");
            Debug.Log("deleted PlayerPrefs('carriers')");
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.Boxes);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.SaveableGameObjects);
        }
    }
}
