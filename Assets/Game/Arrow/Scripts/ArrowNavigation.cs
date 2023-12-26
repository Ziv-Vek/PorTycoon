using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowNavigation : MonoBehaviour
{
    public Transform PlayerPosition;
    [SerializeField] GameObject GreenArrow;
    public Transform Target;
    private void Start()
    {
        PlayerPosition = GameObject.Find("Player").transform;
    }
    void Update()
    {
        if (Target && Vector3.Distance(gameObject.transform.position, Target.position) < 20)
        {
            gameObject.transform.transform.localScale = new Vector3(Vector3.Distance(gameObject.transform.transform.position, Target.position) / 20,
                Vector3.Distance(gameObject.transform.transform.position, Target.position) / 20,
                Vector3.Distance(gameObject.transform.transform.position, Target.position) / 20);
        }
        gameObject.transform.position = PlayerPosition.position;
        gameObject.transform.transform.LookAt(Target);
        GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
        if (Target && Vector3.Distance(gameObject.transform.position, Target.position) < 11)
            Destroy(gameObject);
    }
}
