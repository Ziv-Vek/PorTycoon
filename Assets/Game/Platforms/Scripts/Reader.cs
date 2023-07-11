using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Reader : MonoBehaviour
{
    [SerializeField] private float scanningDuration = 1.5f;
    [SerializeField] private Light scannerLight;
    
    public event Action OnReaderFull;
    public event Action<GameObject> OnReaderEmpty;


    IEnumerator OnTriggerEnter(Collider other)
    {
        Debug.Log("scanner trigger");
        if (!other.CompareTag("Box")) yield return null;

        Debug.Log("scanner ok");
        OnReaderFull?.Invoke();
        
        yield return StartCoroutine(ProcessScanning());
        
        OnReaderEmpty?.Invoke(other.gameObject);
    }

    IEnumerator ProcessScanning()
    {
        yield return StartCoroutine(ScanningAnim());
        
        yield return OnReaderFull;
    }

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
