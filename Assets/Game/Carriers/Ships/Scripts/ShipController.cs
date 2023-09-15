using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ShipCarrier))]
public class ShipController : MonoBehaviour
{
    public Transform targetPoint;
    public Transform dockingPoint;
    public float waitTimeAtSeaTarget = 3f;
    [SerializeField] public static float shipSpeed = 11.5f;
    [CanBeNull] public MeshRenderer cargoMeshRenderer; 
    private ShipCarrier shipCarrier;
    public GameObject ShipEffects;

    public void setSpeed(float s)
    { shipSpeed = s; }
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
        Quaternion rot = new Quaternion();
        var dockingPos = dockingPoint.position;
        var targetPos = targetPoint.position;
        
        while (true)
        {
            // Turning on the ship effects
            foreach (Transform child in ShipEffects.transform)
                child.gameObject.GetComponent<ParticleSystem>().Play();

            // Move to the target point at sea
            yield return StartCoroutine(MoveToPosition(targetPos));

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
            if (cargoMeshRenderer) cargoMeshRenderer.enabled = false;
            //yield return StartCoroutine(cargoHandler.HandleCargoTransfer());
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