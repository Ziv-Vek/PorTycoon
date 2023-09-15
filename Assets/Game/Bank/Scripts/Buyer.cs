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
    public GameObject productClone;


    // Start is called before the first frame update
    void Start()
    {
        PriceText.text = Price + "$";
        SliderFill.maxValue = Price;
        if (product.active)
            Destroy(gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (moneyAmount != Price)
                InvokeRepeating("GivingOneByOne", 0, 0.15f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (moneyAmount != Price && other.gameObject.tag == "Player")
            CancelInvoke();
        gameObject.GetComponent<AudioSource>().pitch = 1.1f;
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
        gameObject.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<AudioSource>().pitch += 0.005f;
    }
    public void ActiveProduct()
    {
            product.SetActive(true);
            product.transform.position = productClone.transform.position;
            Destroy(gameObject);
    }
}
