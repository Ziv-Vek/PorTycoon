using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public Transform targetPosition; // Assign the target position in the Inspector
    public float speed = 50f; // The speed at which the object will move
    public float radius = 2f; // The radius of the orbit.
    private float angle = 0f;  // Current angle (in radians).
    public bool startMove = false;
    [SerializeField] private GameObject StarPartical;
    
    void Start()
    {
        targetPosition = GameObject.Find("Player").transform;
        Instantiate(StarPartical, transform);
        startMove = true;
    }

    void Update()
    {
        if (startMove)
        {
            angle += speed * Time.deltaTime;  
            float x = targetPosition.position.x + Mathf.Cos(angle) * radius;
            float z = targetPosition.position.z + Mathf.Sin(angle) * radius;
            transform.position = new Vector3(x, transform.position.y, z);
            radius -= 4f * Time.deltaTime;
            if(transform.localScale.x > 0)
            transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f) * Time.deltaTime;

            if (radius <= 1f)
            {
                transform.position = targetPosition.position;
                startMove = false;
                Bank.Instance.DepositStars(1);
                UIManager.Instance.UpdateStarsText(GameManager.Instance.stars);     
                Destroy(gameObject);
            }
        }
        transform.Rotate(new Vector3(0,80,0) * Time.deltaTime);
        if(Vector3.Distance(transform.position,targetPosition.position) < 18)
            startMove = true;
    }
}
