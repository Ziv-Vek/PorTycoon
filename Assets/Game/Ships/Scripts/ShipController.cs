using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ShipCargoHandler))]
public class ShipController : MonoBehaviour
{
    public Transform targetPoint;
    public Transform dockingPoint;
    public float waitTimeAtSeaTarget = 3f;
    public float shipSpeed = 5f;
    private ShipCargoHandler cargoHandler;

    private void Awake()
    {
        cargoHandler = GetComponent<ShipCargoHandler>();
    }

    private void OnEnable()
    {
        cargoHandler.onCargoTransferCompleted += ProcessCargoTransferCompletion;
    }

    private void OnDisable()
    {
        cargoHandler.onCargoTransferCompleted -= ProcessCargoTransferCompletion;
    }

    private void Start()
    {
        StartCoroutine(GoToTargetAndWaitAndReturn());
    }

    private void ProcessCargoTransferCompletion()
    {
        StartCoroutine(GoToTargetAndWaitAndReturn());
    }

    private IEnumerator GoToTargetAndWaitAndReturn()
    {
        // Move to the target point at sea
        yield return StartCoroutine(MoveToPosition(targetPoint.position, shipSpeed));
   
        // Instantiate new cargo on the ship
        cargoHandler.InstantiateCargo();
        
        // Wait for the specified time
        yield return new WaitForSeconds(waitTimeAtSeaTarget);

        // Move back to the docking point at pier
        yield return StartCoroutine(MoveToPosition(dockingPoint.position, shipSpeed));

        // Transfer cargo to pier platform
        yield return cargoHandler.StartCoroutine(cargoHandler.HandleCargoTransfer(0));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        float timeToTravel = distance / speed;
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;

        while (elapsedTime < timeToTravel)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / timeToTravel;
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }
    }
}