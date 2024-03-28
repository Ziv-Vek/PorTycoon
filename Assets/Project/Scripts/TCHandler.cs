using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TCHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private LevelLoader levelLoader;

    private const string LATEST_PP_DATE = "PP16012024";
    
    public void Init()
    {
        Debug.Log("called init");
        // if (PlayerPrefs.HasKey(LATEST_PP_DATE) == false || PlayerPrefs.GetInt(LATEST_PP_DATE, 0) == 0)
        // {
        //     PlayerPrefs.SetInt(LATEST_PP_DATE, 0);
        // }
        // else
        // {
        //     AcceptAndContinue();
        // }
    }

    public void AcceptAndContinue()
    {
        PlayerPrefs.SetInt(LATEST_PP_DATE, 1);
        StartCoroutine(levelLoader.LoadNextScene());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var textComp = GetComponent<TMP_Text>();
        
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComp, new Vector3(eventData.position.x, eventData.position.y, Camera.main != null ? Camera.main.nearClipPlane : 0), null);
        if (linkIndex != -1)
        {
            Debug.Log("found");
            switch (textComp.textInfo.linkInfo[linkIndex].GetLinkID())
            {
                case "pp":
                {
                    Debug.Log("ppp clicked");
                    OpenPrivacyPolicy();
                    break;
                }
                default:
                {
                    break;
                }
                
            }
        }
            
    }
    
    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("http://www.hoppa-play.com/privacy-policy");
    }
}
