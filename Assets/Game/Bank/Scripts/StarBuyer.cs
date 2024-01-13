using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StarBuyer : MonoBehaviour
{
    [SerializeField] public int moneyAmount;
    [SerializeField] public int Price;
    public TextMeshPro PriceText;
    public Slider SliderFill;

    public GameObject product;

    //if there is product plugin...
    public GameObject productPlugin;
    public float TimePerStash = 0.15f;
    public int CurrentLevel;

    public AudioClip Sound;

    // Start is called before the first frame update
    void Start()
    {
        CurrentLevel = transform.parent.GetComponent<PortLoader>().PortLevel;
        PriceText.text = Price.ToString();
        SliderFill.maxValue = Price;
        SliderFill.value = GameManager.Instance.LevelsData["Port" + CurrentLevel].StatueMoney;
        PriceText.text = (Price - GameManager.Instance.LevelsData["Port" + CurrentLevel].StatueMoney).ToString();

        if (product.activeSelf)
        {
            try
            {
                Debug.Log($"product {product.name} is active");
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
        if (GameManager.Instance.stars < 1)
        {
            CancelInvoke();
            gameObject.GetComponent<AudioSource>().pitch = 1.1f;
            return;
        }

        Bank.Instance.DepositStars(-1);
        moneyAmount++;
        PriceText.text = Price - moneyAmount + "";
        SliderFill.value++;

        GameManager.Instance.LevelsData["Port" + CurrentLevel].StatueMoney++;

        if (Price == moneyAmount)
        {
            ActiveProduct(true);
        }

        gameObject.GetComponent<AudioSource>().Play();

        if (TimePerStash - 0.1f * Time.deltaTime > 0.015f)
        {
            CancelInvoke("GivingOneByOne");
            TimePerStash -= 0.15f * Time.deltaTime;
        }

        Invoke("GivingOneByOne", TimePerStash);
    }

    public void UpdateMoneyAmount(int sum)
    {
        moneyAmount = sum;
        PriceText.text = Price - moneyAmount + "";
        SliderFill.value = sum;
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

        //check if the function was called from this class
        if (nameof(Buyer) == new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Name)
        {
           //cool Statue Active
        }
        else
        {
            //silent Statue Active
        }
        if (productPlugin != null)
            productPlugin.SetActive(true);

        AudioManager.Instance.Play("Buying Upgrade");

        StartCoroutine(UserDataManager.Instance.SaveUserDataWithDelay());

        Destroy(gameObject);
    }
}
