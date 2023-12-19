using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public int CurrentLevel;

    // Start is called before the first frame update
    void Start()
    {
        CurrentLevel = transform.parent.GetComponent<PortLoader>().PortLevel;
        PriceText.text = Price + "$";
        SliderFill.maxValue = Price;
        if (product.activeSelf)
        {
            try
            {
                Debug.Log($"product {product.name} is active");
                NextBuyer.SetActive(true);
            }
            catch
            {
            }

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (moneyAmount != Price)
                InvokeRepeating("GivingOneByOne", 0, 0.1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (moneyAmount != Price && other.gameObject.CompareTag("Player"))
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
        GameObject money = Instantiate(moneyPrefab, GameObject.Find("Player").transform.position + new Vector3(0, 4, 0),
            Quaternion.identity);
        money.GetComponent<MoneyPrefab>().targetPosition = transform.GetChild(0).transform;
        money.GetComponent<MoneyPrefab>().startMove = true;
        money.transform.parent = transform;
        moneyAmount++;
        PriceText.text = Price - moneyAmount + "$";
        SliderFill.value++;
        if (Price == moneyAmount)
        {
            ActiveProduct(true);
        }

        if (!gameObject.GetComponent<AudioSource>().isPlaying)
        {
            gameObject.GetComponent<AudioSource>().pitch += 0.004f;
            gameObject.GetComponent<AudioSource>().Play();
        }

        if (TimePerStash - 0.1f * Time.deltaTime > 0.015f)
        {
            CancelInvoke("GivingOneByOne");
            TimePerStash -= 0.15f * Time.deltaTime;
        }

        Invoke("GivingOneByOne", TimePerStash);
    }

    public virtual void ActiveProduct(bool isOnPurchaseActivation)
    {
        Debug.Log(isOnPurchaseActivation + " " + product.name);
        product.SetActive(true);

        var productsCompArr = product.GetComponents<IProduct>();
        if (productsCompArr.Length > 0)
        {
            foreach (var activeProduct in product.GetComponents<IProduct>())
            {
                activeProduct.OnProductActivation(isOnPurchaseActivation);
            }    
        }
        else // spawn at default position
        {
            product.transform.position = productClone.transform.position;
        }
        
        try
        {
            NextBuyer.SetActive(true);
        }
        catch
        {
        }

        //check if the function was called from this class
        if (nameof(Buyer) == new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Name)
        {
            if (gameObject.name == "Forklift Buyer")
            {
                GameManager.Instance.LevelsData["Port" + CurrentLevel].ForkliftIsEnabled = true;
            }
            else if (gameObject.name == "Ship Buyer" && GameManager.Instance.LevelsData["Port" + CurrentLevel].ShipNumber < 3)
            {
                GameManager.Instance.LevelsData["Port" + CurrentLevel].ShipNumber++;
            }
            else if (gameObject.name == "HandyMan Buyer" && GameManager.Instance.LevelsData["Port" + CurrentLevel].HandyManNumber < 2)
            {
                GameManager.Instance.LevelsData["Port" + CurrentLevel].HandyManNumber++;
            }
        }

        if (productPlugin != null)
            productPlugin.SetActive(true);

        AudioManager.Instance.Play("Buying Upgrade");

        if (!GameManager.Instance.GoneThroughTutorial)
            FindAnyObjectByType<TutorialM>().SetToShipment_Target();

        StartCoroutine(UserDataManager.Instance.SaveUserDataWithDelay());

        Destroy(gameObject);
    }
}