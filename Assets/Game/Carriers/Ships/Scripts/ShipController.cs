using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ShipCarrier))]
public class ShipController : MonoBehaviour
{
    // TODO: add save and restore position and movement direction from save
    
    public Transform targetPoint;
    public Transform dockingPoint;
    public float waitTimeAtSeaTarget = 3f;
    public float shipSpeed = 5f;
    private ShipCarrier shipCarrier;
    public GameObject ShipEffects;

    private void Awake()
    {
        shipCarrier = GetComponent<ShipCarrier>();
    }

    private void Start()
    {
        StartCoroutine(GoToTargetAndWaitAndReturn());
    }

    private void ProcessBoxesTransferCompletion()
    {
        StartCoroutine(GoToTargetAndWaitAndReturn());
    }

    private IEnumerator GoToTargetAndWaitAndReturn()
    {
        while (true)
        {
            // Turning on the ship effects
            foreach (Transform child in ShipEffects.transform)
                child.gameObject.GetComponent<ParticleSystem>().Play();

            // Move to the target point at sea
            yield return StartCoroutine(MoveToPosition(targetPoint.position));

            // Instantiate new cargo on the ship
            shipCarrier.InstantiateCargo();

            // Wait for the specified time
            yield return new WaitForSeconds(waitTimeAtSeaTarget);

            // Move back to the docking point at pier
            yield return StartCoroutine(MoveToPosition(dockingPoint.position));
            
            // Turning off the ship effects
            foreach (Transform child in ShipEffects.transform)
                child.gameObject.GetComponent<ParticleSystem>().Stop();

            // Transfer cargo to pier platform
            yield return StartCoroutine(shipCarrier.TransferBoxesToPier());
            //yield return StartCoroutine(cargoHandler.HandleCargoTransfer());
        }
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        float timeToTravel = distance / shipSpeed;
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