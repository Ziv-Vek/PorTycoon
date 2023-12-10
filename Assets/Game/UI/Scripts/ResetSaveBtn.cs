using UnityEngine;
using UnityEngine.UI;

public class ResetSaveBtn : MonoBehaviour
{
    private Button btn;

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        UserDataManager.Instance.ResetUserData();
        // hide button
        btn.gameObject.SetActive(false);
    }
}