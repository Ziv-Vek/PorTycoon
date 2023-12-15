using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MoneyPile : MonoBehaviour
{
    [SerializeField] public int moneyAmount = 2;
    [SerializeField] GameObject moneyPrefab;
    [SerializeField] int AmountForNewPile;
    [SerializeField] float PlusY;
    public Vector3 place;
    [SerializeField] int MoneyPerBill;
    public int moneyLimit;
    public float TimePerStash = 0.15f;

    void Start()
    {
        place = gameObject.transform.position;
        PlusY = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (moneyAmount != 0)
                InvokeRepeating("TakingOneByOne", 0, TimePerStash);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (moneyAmount != 0 && other.gameObject.CompareTag("Player"))
            CancelInvoke();
        gameObject.GetComponent<AudioSource>().pitch = 1.1f;
        TimePerStash = 0.16f;
    }

    void TakingOneByOne()
    {
        var childCount = transform.childCount;
        transform.GetChild(childCount - 1).gameObject.GetComponent<MoneyPrefab>().targetPosition =
            GameObject.Find("Player").transform;
        transform.GetChild(childCount - 1).gameObject.GetComponent<MoneyPrefab>().startMove = true;
        transform.GetChild(childCount - 1).parent = null;
        Bank.Instance.DepositMoney(MoneyPerBill);
        if (PlusY - 1.1 >= 0)
            PlusY -= 1.1f;
        moneyAmount--;

        if (transform.childCount % AmountForNewPile == 0)
        {
            if (moneyAmount != 0)
            {
                PlusY = 0;
                place.x += 4;
                GetComponent<BoxCollider>().center = new Vector3(GetComponent<BoxCollider>().center.x + 0.22f,
                    GetComponent<BoxCollider>().center.y, GetComponent<BoxCollider>().center.z);
                GetComponent<BoxCollider>().size = new Vector3(GetComponent<BoxCollider>().size.x - 0.43f,
                    GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z);
            }
        }

        if (moneyAmount == 0)
        {
            CancelInvoke();
            gameObject.GetComponent<AudioSource>().pitch = 1.1f;
            return;
        }
        if (!gameObject.GetComponent<AudioSource>().isPlaying)
        {
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<AudioSource>().pitch += 0.004f;
        }
        if (TimePerStash - (0.2f * Time.deltaTime) > 0.05f)
        {
            CancelInvoke("TakingOneByOne");
            TimePerStash -= 0.2f * Time.deltaTime;
        }

        Invoke("TakingOneByOne", TimePerStash);
    }

    public void AddMoney(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (moneyAmount >= moneyLimit)
                break;
            GameObject Money = Instantiate(moneyPrefab, new Vector3(place.x, place.y + PlusY, place.z),
                Quaternion.identity);
            Money.transform.parent = gameObject.transform;
            PlusY += 1.1f;
            moneyAmount++;
            if (transform.childCount % AmountForNewPile == 0)
            {
                place.x -= 4;
                PlusY = 0;
                GetComponent<BoxCollider>().center = new Vector3(GetComponent<BoxCollider>().center.x - 0.22f,
                    GetComponent<BoxCollider>().center.y, GetComponent<BoxCollider>().center.z);
                GetComponent<BoxCollider>().size = new Vector3(GetComponent<BoxCollider>().size.x + 0.43f,
                    GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z);
            }
        }
    }
}