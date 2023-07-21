using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoneyThrower : MonoBehaviour
{
    [SerializeField] GameObject moneyPrefab;   // Prefab of the money pile
    [SerializeField] int numPiles = 5;         // Number of money piles to throw
    [SerializeField] float minDistance = 5f;   // Minimum distance from the throw point
    [SerializeField] float maxDistance = 10f;  // Maximum distance from the throw point
    [SerializeField] float minAngle = 0f;      // Minimum angle from the throw point (in degrees)
    [SerializeField] float maxAngle = 45f;     // Maximum angle from the throw point (in degrees)
    public float radius = 0.5f;
    public float angle = 90f;
    
    private void OnEnable()
    {
        /*if (TryGetComponent<CargoTransferActionZone>(out var actionZone))
        {
         actionZone.onCargoTransferCompleted += HandleThrowProcess;   
        }
        else
        {
            throw new Exception($"CargoTransferActionZone component missing on {this.gameObject.name}");
        }*/
    }

    private void OnDisable()
    {
        /*if (TryGetComponent<CargoTransferActionZone>(out var actionZone))
        {
            actionZone.onCargoTransferCompleted -= HandleThrowProcess;   
        }*/
    }

    private void HandleThrowProcess()
    {
        float amountToThrow = Bank.Instance.GetAmountOfMoneyToThrow();
        
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

    private void OnDrawGizmosSelected()
    {
        // Set gizmo color
        Gizmos.color = Color.yellow;

        // Calculate start and end angles in radians
        float startAngle = transform.eulerAngles.y - angle / 2f;
        float endAngle = transform.eulerAngles.y + angle / 2f;

        // Calculate start and end positions
        Vector3 startPosition = transform.position + Quaternion.Euler(0f, startAngle, 0f) * (Vector3.forward * radius);
        Vector3 endPosition = transform.position + Quaternion.Euler(0f, endAngle, 0f) * (Vector3.forward * radius);

        // Draw the sector
        Gizmos.DrawLine(transform.position, startPosition);
        Gizmos.DrawLine(transform.position, endPosition);
        Gizmos.DrawWireSphere(transform.position, radius);
        Handles.DrawWireArc(transform.position, Vector3.up, startPosition - transform.position, angle, radius);
    }

    // FOR DEBUG ONLY
    public void ThrowForDebug()
    {
        HandleThrowProcess();
    }
    
}
