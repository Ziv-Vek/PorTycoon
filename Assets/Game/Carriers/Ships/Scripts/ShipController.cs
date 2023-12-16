using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ShipCarrier))]
public class ShipController : MonoBehaviour, IProduct
{
    public Transform targetPoint;
    public Transform dockingPoint;
    public float waitTimeAtSeaTarget = 3f;
    [SerializeField] public float shipSpeed = 11.5f;
    [CanBeNull] public MeshRenderer cargoMeshRenderer;
    private ShipCarrier shipCarrier;
    public GameObject ShipEffects;
    bool isFirstSpawnAfterPurchase = false;
    
    private const float MIN_DELTA_DISTANCE_FACTOR = 0.2f;
    private const float MAX_DELTA_DISTANCE_FACTOR = 0.8f;

    public void setSpeed(float s)
    {
        shipSpeed = s;
    }

    private void Awake()
    {
        shipCarrier = GetComponent<ShipCarrier>();
    }
    
    // instantiate new ship to the target point
    public void OnProductActivation(bool isOnPurchaseActivation)
    {
        if (isOnPurchaseActivation)
        {
            isFirstSpawnAfterPurchase = true;
            SpawnShip(MIN_DELTA_DISTANCE_FACTOR);
        }
        else
        {
            SpawnShip(Helpers.GenerateRandomNumber(MIN_DELTA_DISTANCE_FACTOR, MAX_DELTA_DISTANCE_FACTOR));
        }

        StartCoroutine(GoToTargetAndWaitAndReturn());
        shipCarrier.InstantiateCargo();
    }

    private void SpawnShip(float deltaDistanceFactor)
    {
        var startingPoint = Vector3.MoveTowards(targetPoint.position, dockingPoint.position,
            Vector3.Distance(targetPoint.position, dockingPoint.position) * deltaDistanceFactor);

        transform.SetPositionAndRotation(startingPoint, Quaternion.identity);
        transform.LookAt(dockingPoint.position, Vector3.up);
    }

    private IEnumerator GoToTargetAndWaitAndReturn()
    {
        while (true)
        {
            // Turning on the ship effects
            foreach (Transform child in ShipEffects.transform)
                child.gameObject.GetComponent<ParticleSystem>().Play();

            // Move back to the docking point at pier
            yield return StartCoroutine(MoveToPosition(dockingPoint.position));

            // Turning off the ship effects
            foreach (Transform child in ShipEffects.transform)
                child.gameObject.GetComponent<ParticleSystem>().Stop();

            // Transfer cargo to pier platform
            yield return StartCoroutine(shipCarrier.TransferBoxesToPier());
            if (cargoMeshRenderer) cargoMeshRenderer.enabled = false;
            //yield return StartCoroutine(cargoHandler.HandleCargoTransfer());

            // Turning on the ship effects
            foreach (Transform child in ShipEffects.transform)
                child.gameObject.GetComponent<ParticleSystem>().Play();

            // Move to the target point at sea
            yield return StartCoroutine(MoveToPosition(targetPoint.position));

            // Instantiate new cargo on the ship
            shipCarrier.InstantiateCargo();

            // Wait for the specified time
            yield return new WaitForSeconds(waitTimeAtSeaTarget);
        }
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        float timeToTravel = distance / shipSpeed;
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;

        transform.LookAt(targetPosition);

        while (elapsedTime < timeToTravel)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / timeToTravel;
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }
    }
}
