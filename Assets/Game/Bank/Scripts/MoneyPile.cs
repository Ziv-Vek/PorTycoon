using System;
using UnityEngine;

public class MoneyPile : MonoBehaviour
{
    [SerializeField] private int moneyAmount = 2;
    [SerializeField] GameObject moneyPrefab;
    [SerializeField] float PlusY;
    void Start()
    {
        PlusY = 0;
        for (int i = 0; i < moneyAmount; i++)
        {
            GameObject Money = Instantiate(moneyPrefab, new Vector3(transform.position.x, transform.position.y + PlusY, transform.position.z), Quaternion.identity);
            Money.transform.parent = gameObject.transform;
            PlusY += 1.1f;
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            if(moneyAmount != 0)
            InvokeRepeating("TakingOneByOne", 0, 0.1f);
        }
    }
    public void MoneyTaking()
    {

    }
    void TakingOneByOne()
    {
        Destroy(transform.GetChild(moneyAmount-1).gameObject);
        Bank.Instance.DepositMoney(10);
        PlusY -= 1.1f;
        moneyAmount--;
        if (moneyAmount == 0)
        {
            CancelInvoke();
            return;
        }
    }
    public void AddMoney(int amount)
    {
        moneyAmount += amount;
        for (int i = 0; i < amount; i++)
        {
            GameObject Money = Instantiate(moneyPrefab, new Vector3(transform.position.x, transform.position.y + PlusY, transform.position.z), Quaternion.identity);
            Money.transform.parent = gameObject.transform;
            PlusY += 1.1f;
        }
    }
}
