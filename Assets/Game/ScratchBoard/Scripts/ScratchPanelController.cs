using UnityEngine;

public class ScratchPanelController : MonoBehaviour
{
    public GameObject panel; // Assign your Panel in the inspector

    // panel bottom image
    public GameObject panelBottomImage;

    // panel top image
    public GameObject panelTopImage;

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}