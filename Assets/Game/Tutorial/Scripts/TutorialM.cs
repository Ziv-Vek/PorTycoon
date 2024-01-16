using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialM : MonoBehaviour
{
    public static TutorialM Instance;
    public int step;
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
    [SerializeField] GameObject[] buyers;

    [SerializeField] GameObject ArrowPrefab;


    private void Start()
    {
        Arrow = transform.Find("Arrow").gameObject;
        Target = BuyingShipPlace;
        step = 0;
        foreach (GameObject buyer in buyers)
        {
            buyer.SetActive(false);
        }
        BuyingHRoffice.transform.gameObject.SetActive(false);
        BuyingLogisticOffice.gameObject.SetActive(false);
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
        if (step == 0)
        {
            Target = ShipmentPlace;
            GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
            Arrow.transform.localScale = Vector3.one;
            step = 1;
        }
    }

    public void SetConveyor_Target()
    {
        if (step == 1)
        {
            Target = ConveyorPlace;
            Arrow.transform.localScale = Vector3.one;
            GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
            step = 2;
        }
    }

    public void SetBoxTable_Target()
    {
        if (step == 2)
        {
            Target = BoxTablePlace;
            Arrow.transform.localScale = Vector3.one;
            GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
            step = 3;
        }
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
        if (step == 3)
        {
            ClickHere_Collection.SetActive(false);
            Target = ScratchMoneyPile;
            Arrow.transform.localScale = Vector3.one;
            GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
            step = 4;
        }
    }

    public void SetBuyingHRoffice_Target()
    {
        if (step == 4)
        {
            BuyingHRoffice.transform.gameObject.SetActive(true);
            BuyingLogisticOffice.gameObject.SetActive(true);
            Target = BuyingHRoffice;
            Arrow.transform.localScale = Vector3.one;
            GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
            step = 5;
        }
    }

    public void SetHRofficeShop_Target()
    {
        if (step == 5)
        {
            Target = HRofficeShop;
            Arrow.transform.localScale = Vector3.one;
            GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
            ClickOn_HRshop();
            step = 6;
        }
    }

    public void ClickOn_HRshop()
    {
        ClickHere_HRshop.SetActive(true);
    }

    public void SetBuyingLogisticOffice_Target()
    {
        if (step == 6)
        {
            ClickHere_HRshop.SetActive(false);
            Target = BuyingLogisticOffice;
            Arrow.transform.localScale = Vector3.one;
            GreenArrow.transform.position = Target.position + new Vector3(0, 30, 0);
            step = 7;
        }
    }

    public void StartEndAnimation()
    {
        if (step == 7)
        {
            foreach (GameObject buyer in buyers)
            {
                buyer.SetActive(true);
            }
            transform.Find("End Tutorial Camera").gameObject.SetActive(true);
            GreenArrow.SetActive(false);
            PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
            playerMover.ToggleMovement(false);
            playerMover.HideJoystick();
            step = 8;    
            Instantiate(ArrowPrefab).GetComponent<ArrowNavigation>().Target = ShipmentPlace;
            FindAnyObjectByType<CameraManager>().gameObject.GetComponent<Camera>().enabled = false;
        }
    }

    public void DestroyItSelf()
    {
        GameManager.Instance.GoneThroughTutorial = true;
        // Save
        UserDataManager.Instance.SaveUserDataAsync();

        FindAnyObjectByType<CameraManager>().gameObject.GetComponent<Camera>().enabled = true;

        Destroy(ClickHere_Collection);
        Destroy(ClickHere_HRshop);
        Destroy(gameObject);
    }
}