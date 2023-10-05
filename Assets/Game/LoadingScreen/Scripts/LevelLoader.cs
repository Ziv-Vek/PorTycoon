using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelLoader : MonoBehaviour
{
    public Image LoadingImage;
    private void Update()
    {
        LoadingImage.transform.Rotate(new Vector3(0, 0,-50) * Time.deltaTime);
    }
    public void LoadScendByIndex(int index)
    {
        StartCoroutine(LoadScene(index));
    }
    IEnumerator LoadScene (int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
