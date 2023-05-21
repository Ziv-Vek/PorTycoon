using System.Collections;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public Transform targetPoint;
    public Transform dockingPoint;
    public float waitTimeDocking = 3f;
    public float waitTimeTarget = 3f;
    public float shipSpeed = 5f;

    private void Start()
    {
        StartCoroutine(GoToTargetAndWaitAndReturn());
    }

    private IEnumerator GoToTargetAndWaitAndReturn()
    {
        while (true)
        {
            // Move to the target point
            yield return StartCoroutine(MoveToPosition(targetPoint.position, shipSpeed));

            // Wait for the specified time
            yield return new WaitForSeconds(waitTimeTarget);

            // Move back to the docking point
            yield return StartCoroutine(MoveToPosition(dockingPoint.position, shipSpeed));

            // Wait for the specified time
            yield return new WaitForSeconds(waitTimeDocking);
        }
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