using UnityEngine;
using TMPro;

public class BoxProduct : MonoBehaviour
{
    public int Price = 000;

    GameConfig gameConfig;

    // Start is called before the first frame update
    private void OnEnable()
    {
        transform.Find("BuyButton").GetChild(0).GetComponent<TextMeshProUGUI>().text = Price.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("Parent Box").Rotate(new Vector3(0, 20, 0) * Time.deltaTime);
    }
}