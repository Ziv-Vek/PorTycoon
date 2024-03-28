using System;
using SupersonicWisdomSDK;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    private void Awake()
    {
        // Subscribe
        SupersonicWisdom.Api.AddOnReadyListener(OnSupersonicWisdomReady);

        // Then initialize
        SupersonicWisdom.Api.Initialize();
    }

    private void OnSupersonicWisdomReady()
    {
        FindObjectOfType<TCHandler>().Init();
    }
}