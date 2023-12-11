using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialM : MonoBehaviour
{
    public static TutorialM Instance;
    [SerializeField] GameObject Arrow;
    [SerializeField] Transform playePosition;
    [SerializeField] Transform Target;
    [SerializeField] Transform BuyingShipPlace;
    [SerializeField] Transform ShipmentPlace;
    [SerializeField] Transform ConveyorPlace;
    [SerializeField] Transform BoxTablePlace;
    [SerializeField] GameObject ClickHere;

    private void Start()
    {
        Arrow = transform.Find("Arrow").gameObject;
        Target = BuyingShipPlace;
    }

    private void Update()
    {
        Arrow.transform.position = playePosition.position;
        Arrow.transform.LookAt(Target);
        if (Target && Vector3.Distance(playePosition.position, Target.position) < 20)
        {
            Arrow.transform.localScale = new Vector3(Vector3.Distance(Arrow.transform.position, Target.position) / 20,
                Vector3.Distance(Arrow.transform.position, Target.position) / 20,
                Vector3.Distance(Arrow.transform.position, Target.position) / 20);
        }
    }

    public void SetToShipment_Target()
    {
        Target = ShipmentPlace;
        Arrow.transform.localScale = Vector3.one;
    }

    public void SetConveyor_Target()
    {
        Target = ConveyorPlace;
        Arrow.transform.localScale = Vector3.one;
    }

    public void SetBoxTable_Target()
    {
        Target = BoxTablePlace;
        Arrow.transform.localScale = Vector3.one;
    }

    public void ClickOn_CollectionPanel()
    {
        ClickHere.SetActive(true);
    }

    public void DestroyItSelf()
    {
        GameManager.Instance.GoneThroughTutorial = true;
        Destroy(ClickHere);
        Destroy(gameObject);
    }
}