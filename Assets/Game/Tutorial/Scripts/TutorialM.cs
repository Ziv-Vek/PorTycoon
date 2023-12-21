using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialM : MonoBehaviour
{
    public static TutorialM Instance;
    [SerializeField] GameObject Arrow;
    [SerializeField] GameObject GreenArrow;
    [SerializeField] Transform playePosition;
    [SerializeField] Transform Target;
    [SerializeField] Transform BuyingShipPlace;
    [SerializeField] Transform ShipmentPlace;
    [SerializeField] Transform ConveyorPlace;
    [SerializeField] Transform BoxTablePlace;
    [SerializeField] GameObject ClickHere_Collection;
    [SerializeField] Transform ScratchMoneyPile;
    [SerializeField] Transform BuyingHRoffice;
    [SerializeField] Transform HRofficeShop;
    [SerializeField] GameObject ClickHere_HRshop;
    [SerializeField] Transform BuyingLogisticOffice;


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
        GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
        Arrow.transform.localScale = Vector3.one;
    }

    public void SetConveyor_Target()
    {
        Target = ConveyorPlace;
        Arrow.transform.localScale = Vector3.one;
        GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
    }

    public void SetBoxTable_Target()
    {
        Target = BoxTablePlace;
        Arrow.transform.localScale = Vector3.one;
        GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
    }

    public void ClickOn_CollectionPanel()
    {
        GreenArrow.SetActive(false);
        ClickHere_Collection.SetActive(true);
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
    }

    public void SetScratchMoney_Target()
    {
        ClickHere_Collection.SetActive(false);
        Target = ScratchMoneyPile;
        Arrow.transform.localScale = Vector3.one;
        GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
    }

    public void SetBuyingHRoffice_Target()
    {
        Target = BuyingHRoffice;
        Arrow.transform.localScale = Vector3.one;
        GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
    }

    public void SetHRofficeShop_Target()
    {
        Target = HRofficeShop;
        Arrow.transform.localScale = Vector3.one;
        GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
        ClickOn_HRshop();
    }

    public void ClickOn_HRshop()
    {
        ClickHere_HRshop.SetActive(true);
    }

    public void SetBuyingLogisticOffice_Target()
    {
        ClickHere_HRshop.SetActive(false);
        Target = BuyingLogisticOffice;
        Arrow.transform.localScale = Vector3.one;
        GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
    }

    public void StartEndAnimation()
    {
        transform.Find("End Tutorial Camera").gameObject.SetActive(true);
        GreenArrow.SetActive(false);
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
    }

    public void DestroyItSelf()
    {
        GameManager.Instance.GoneThroughTutorial = true;
        GameManager.Instance.LevelsData["Port1"].ShipNumber = 1;
        // Save
        UserDataManager.Instance.SaveUserDataAsync();

        Destroy(ClickHere_Collection);
        Destroy(ClickHere_HRshop);
        Destroy(gameObject);
    }
}