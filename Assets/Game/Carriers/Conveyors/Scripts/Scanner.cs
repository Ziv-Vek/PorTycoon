using System;
using System.Collections;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [SerializeField] public float scanningDuration = 1.5f;
    [SerializeField] private GameObject scanArea;
    public event Action OnScannerActivated;
    public event Action OnScannerDeactivated;

    IEnumerator OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Box")) yield break;

        OnScannerActivated?.Invoke();

        yield return StartCoroutine(ScanningAnim());

        OnScannerDeactivated?.Invoke();
    }

    IEnumerator ScanningAnim()
    {
        float counter = 0f;
        scanArea.SetActive(true);

        while (counter < scanningDuration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        scanArea.SetActive(false);

        yield return null;
    }
}