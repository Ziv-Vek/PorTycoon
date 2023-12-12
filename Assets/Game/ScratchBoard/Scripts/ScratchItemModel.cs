using UnityEngine;

public class ScratchItemModel : MonoBehaviour
{
    private const string IMAGE_PATH = "items/Collectibles/";

    public void ChangeModel(string filename)
    {
        // Get model from Resources folder
        GameObject model = Resources.Load<GameObject>(IMAGE_PATH + filename);
        if (model == null)
        {
            Debug.LogError("Image not found: " + filename);
            return;
        }

        // Change image
        Instantiate(model, transform.position - new Vector3(0, 14f, 0), model.transform.rotation, transform);
    }
}