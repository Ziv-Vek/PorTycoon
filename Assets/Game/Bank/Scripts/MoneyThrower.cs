using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoneyThrower : MonoBehaviour
{
    [SerializeField] GameObject moneyPrefab; // Prefab of the money pile
    [SerializeField] int numPiles = 5; // Number of money piles to throw
    [SerializeField] float minDistance = 5f; // Minimum distance from the throw point
    [SerializeField] float maxDistance = 10f; // Maximum distance from the throw point
    [SerializeField] float minAngle = 0f; // Minimum angle from the throw point (in degrees)
    [SerializeField] float maxAngle = 45f; // Maximum angle from the throw point (in degrees)

    private void HandleThrowProcess()
    {
        for (int i = 0; i < numPiles; i++)
        {
            float distance = Random.Range(minDistance, maxDistance);
            float angle = Random.Range(minAngle, maxAngle);

            float radAngle = angle * Mathf.Deg2Rad;
            float x = distance * Mathf.Cos(radAngle);
            float y = distance * Mathf.Sin(radAngle);

            Vector3 throwPosition = transform.position + new Vector3(x, y, 0f);
            Quaternion throwRotation = Quaternion.Euler(0f, 0f, angle);

            Instantiate(moneyPrefab, throwPosition, throwRotation);
        }
    }

    // FOR DEBUG ONLY
    public void ThrowForDebug()
    {
        HandleThrowProcess();
    }
}