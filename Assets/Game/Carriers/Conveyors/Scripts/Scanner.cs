using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Scanner : MonoBehaviour
{
    [SerializeField] public float scanningDuration = 1.5f;
    [SerializeField] private Light scannerLight;
    
    public event Action OnScannerActivated;
    public event Action OnScannerDeactivated;

    IEnumerator OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Box")) yield break;

        OnScannerActivated?.Invoke();
        
        yield return StartCoroutine(ScanningAnim());
        
        OnScannerDeactivated?.Invoke();
    }

    /*
    IEnumerator ProcessScanning()
    {
        yield return StartCoroutine(ScanningAnim());
        
        yield return OnReaderFull;
    }*/

    IEnumerator ScanningAnim()
    {
        float counter = 0f;

        scannerLight.enabled = true;

        while (counter < scanningDuration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        scannerLight.enabled = false;
        
        yield return null;
    }
}
