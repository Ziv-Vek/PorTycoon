using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Image LoadingImage;
    private bool isLoading;

    private void Update()
    {
        if (LoadingImage != null)
        {
            LoadingImage.transform.Rotate(new Vector3(0, 0, -50) * Time.deltaTime);
        }
    }

    public void LoadSceneByIndex(int index)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadScene(index));
        }
    }

    public IEnumerator LoadScene(int index)
    {
        isLoading = true;
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        while (!operation.isDone)
        {
            yield return null;
        }

        isLoading = false;
    }

    public IEnumerator LoadNextScene()
    {
        if (!isLoading)
        {
            isLoading = true;
            yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            isLoading = false;
        }
    }
}