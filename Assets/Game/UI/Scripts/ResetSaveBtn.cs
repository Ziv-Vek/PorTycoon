using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetSaveBtn : MonoBehaviour
{
    public Button btn; 

    void Start()
    {
        btn.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        UserDataManager.Instance.ResetUserData();
        // hide button
        btn.gameObject.SetActive(false);
    }
}
