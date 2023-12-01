using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandyCoded.HapticFeedback;

public class MoneyPrefab : MonoBehaviour
{
    public Transform targetPosition; // Assign the target position in the Inspector
    public float speed = 50f; // The speed at which the object will move
    public bool startMove = false;
    private void Update()
    {
        float step = speed * Time.deltaTime;
        if (startMove)
        {
            transform.Translate((targetPosition.position - transform.position + new Vector3(0, 4, 0)).normalized * step);
            if (Vector3.Distance(transform.position, targetPosition.position + new Vector3(0, 4, 0)) < step)
            {
                transform.position = targetPosition.position;
                startMove = false;
                VibrationManager.Instance.LightVibrate();
                Destroy(gameObject);
            }
        }
       
    }
}
