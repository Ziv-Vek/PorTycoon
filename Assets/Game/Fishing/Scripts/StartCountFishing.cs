using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartCountFishing : MonoBehaviour
{
    public void PlaySound(string ClipName)
    {
        AudioManager.Instance.Play("Fishing Start Count");
    }
    public void ChangeText(string text)
    {
        GetComponent<TextMeshProUGUI>().text = text;
    }
}
