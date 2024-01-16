using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Image LoadingImage;

    private void Update()
    {
        if (LoadingImage != null)
        {
            LoadingImage.transform.Rotate(new Vector3(0, 0, -50) * Time.deltaTime);
        }
    }

    public void LoadScendByIndex(int index)
    {
        StartCoroutine(LoadScene(index));
    }

    public IEnumerator LoadScene(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        while (!operation.isDone)
        {
            yield return null;
        }
    }
    
    public IEnumerator LoadNextScene()
    {
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}