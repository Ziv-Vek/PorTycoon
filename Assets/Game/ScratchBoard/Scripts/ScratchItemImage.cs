using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ScratchItemImage : MonoBehaviour
{
    private const string IMAGE_PATH = "items/New Sprites/";
    
    public void ChangeImage(string filename)
    {
        // Get image from Resources folder
        Sprite image = Resources.Load<Sprite>(IMAGE_PATH + filename);
        if (image == null)
        {
            Debug.LogError("Image not found: " + filename);
            return;
        }

        // Change image
        gameObject.GetComponent<Image>().sprite = image;
    }
}