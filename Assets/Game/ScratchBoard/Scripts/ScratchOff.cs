using UnityEngine;
using UnityEngine.UI;

public class ScratchOff : MonoBehaviour
{
    public int scratchRadius = 10;

    private Texture2D maskTexture;
    public Camera camera = null;
    public RectTransform rectTransform;

    public int width;
    public int height;

    private Image imageComponent;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        var sizeDelta = rectTransform.sizeDelta;
        width = (int)sizeDelta.x;
        height = (int)sizeDelta.y;
        imageComponent = GetComponent<Image>();
        Reset();
    }

    private void Reset()
    {
        maskTexture = new Texture2D(width, height);
        Color32[] cols = maskTexture.GetPixels32();
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i] = new Color32(0, 0, 0, 255);
        }

        maskTexture.SetPixels32(cols);
        imageComponent.material.mainTexture = maskTexture;

        maskTexture.Apply(false);
    }

    private void Scratch(int xCenter, int yCenter)
    {
        Debug.Log("Scratch");
        int xOffset;
        Color32[] tempArray = maskTexture.GetPixels32();
        bool hasChanged = false;

        for (xOffset = -scratchRadius; xOffset <= scratchRadius; xOffset++)
        {
            var yRange = (int)Mathf.Ceil(Mathf.Sqrt(scratchRadius * scratchRadius - xOffset * xOffset));
            int yOffset;
            for (yOffset = -yRange; yOffset <= yRange; yOffset++)
            {
                var xPos = xCenter + xOffset;
                var yPos = yCenter + yOffset;
                hasChanged = TryScratchPixel(xPos, yPos, ref tempArray) || hasChanged;
            }
        }

        if (hasChanged)
        {
            Debug.Log("Changed");
            maskTexture.SetPixels32(tempArray);
            maskTexture.Apply(false);
        }
    }

    public bool TryScratchPixel(int xPos, int yPos, ref Color32[] pixels)
    {
        Debug.Log("TryScratchPixel");
        Debug.Log(xPos);
        Debug.Log(yPos);
        
        if (xPos < 0 || xPos >= width || yPos < 0 || yPos >= height)
        {
            return false;
        }

        int index = yPos * width + xPos;
        if (pixels[index].a == 0)
        {
            return false;
        }

        pixels[index].a = 0;
        return true;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, camera,
                out Vector2 localPoint);
            Scratch((int)localPoint.x, (int)localPoint.y);
        }
    }
}