using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Buyer : MonoBehaviour
{
    [SerializeField] public int moneyAmount = 0;
    [SerializeField] GameObject moneyPrefab;
    [SerializeField] public int Price;
    public TextMeshPro PriceText;
    public Slider SliderFill;
    public GameObject product;
    //if there is product plugin...
    public GameObject productPlugin;
    public GameObject productClone;
    public GameObject NextBuyer;

    public float TimePerStash = 0.15f;



    // Start is called before the first frame update
    void Start()
    {
        PriceText.text = Price + "$";
        SliderFill.maxValue = Price;
        if (product.active)
        {   
            try { NextBuyer.SetActive(true); } catch { }
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (moneyAmount != Price)
                InvokeRepeating("GivingOneByOne", 0, 0.1f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (moneyAmount != Price && other.gameObject.tag == "Player")
            CancelInvoke();
        gameObject.GetComponent<AudioSource>().pitch = 1.1f;
        TimePerStash = 0.135f;
    }
    void GivingOneByOne()
    { 
        if (GameManager.Instance.money < 1)
        {
            CancelInvoke();
            gameObject.GetComponent<AudioSource>().pitch = 1.1f;
            return;
        }
        Bank.Instance.DepositMoney(-1);
        GameObject Money = Instantiate(moneyPrefab, GameObject.Find("Player").transform.position + new Vector3(0, 4, 0), Quaternion.identity);
        Money.GetComponent<MoneyPrefab>().targetPosition = transform.GetChild(0).transform;
        Money.GetComponent<MoneyPrefab>().startMove = true;
        Money.transform.parent = transform;
        moneyAmount++;
        PriceText.text = Price - moneyAmount + "$" ;
        SliderFill.value++;
        if (Price == moneyAmount)
        {
            ActiveProduct();
        }   
        if (!gameObject.GetComponent<AudioSource>().isPlaying)
        {
            gameObject.GetComponent<AudioSource>().pitch += 0.004f;
            gameObject.GetComponent<AudioSource>().Play();
        }

        if (TimePerStash - (0.2f * Time.deltaTime) > 0.04f)
        {
            CancelInvoke("GivingOneByOne");
            TimePerStash -= 0.2f * Time.deltaTime;
        }
        Invoke("GivingOneByOne", TimePerStash);
    }
    public void ActiveProduct()
    {
        product.SetActive(true);
        product.transform.position = productClone.transform.position;
        try { NextBuyer.SetActive(true); } catch { }
        //check if the function was called from this calss
        if (nameof(Buyer) == new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Name)
        {
            if (gameObject.name == "Forklift Buyer")
            {
                GameManager.Instance.ForkliftIsEnabled = true;
            }
            else if (gameObject.name == "Ship Buyer" && GameManager.Instance.ShipNumber < 3)
            {
                GameManager.Instance.ShipNumber++;
            }
            else if (gameObject.name == "HandyMan Buyer" && GameManager.Instance.HandyManNumber < 2)
            {
                GameManager.Instance.HandyManNumber++;
            }
        }    
        if(productPlugin != null)
            productPlugin.SetActive(true);

        StartCoroutine(UserDataManager.Instance.SaveUserDataWithDelay());
        
        Destroy(gameObject);
    }
}
