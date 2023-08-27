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
    void Start()
    {
        place = gameObject.transform.position;
        PlusY = 0;
        //for (int i = 0; i < moneyAmount; i++)
        //{
        //    GameObject Money = Instantiate(moneyPrefab, new Vector3(transform.position.x, transform.position.y + PlusY, transform.position.z), Quaternion.identity);
        //    Money.transform.parent = gameObject.transform;
        //    PlusY += 1.1f;
        //}
    }

    void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.tag == "Player")
        {
            if (moneyAmount != 0)
               // TakingOneByOne();
            InvokeRepeating("TakingOneByOne", 0, 0.12f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (moneyAmount != 0 && other.gameObject.tag == "Player")
            CancelInvoke();
        gameObject.GetComponent<AudioSource>().pitch = 1.1f;
    }
    void TakingOneByOne()
    {
      //  Destroy(transform.GetChild(moneyAmount - 1).gameObject);
        transform.GetChild(transform.childCount - 1).gameObject.GetComponent<MoneyPrefab>().startMove = true;
        transform.GetChild(transform.childCount - 1).parent = null;
        Bank.Instance.DepositMoney(MoneyPerBill);
        if(PlusY - 1.1 >= 0)
        PlusY -= 1.1f;
        moneyAmount--;

        if (transform.childCount % AmountForNewPile == 0)
        {
            if (moneyAmount != 0)
            {     
                PlusY = 0;
                place.x += 4;
                GetComponent<BoxCollider>().center = new Vector3(GetComponent<BoxCollider>().center.x + 0.22f, GetComponent<BoxCollider>().center.y, GetComponent<BoxCollider>().center.z);
                GetComponent<BoxCollider>().size = new Vector3(GetComponent<BoxCollider>().size.x - 0.43f, GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z);
            }
        }
                Debug.Log(PlusY);

        if (moneyAmount == 0)
        {
            CancelInvoke();
            gameObject.GetComponent<AudioSource>().pitch = 1.1f;
            return;
        }
        gameObject.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<AudioSource>().pitch += 0.005f;

    }
    public void AddMoney(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject Money = Instantiate(moneyPrefab, new Vector3(place.x, place.y + PlusY, place.z), Quaternion.identity);
            Money.transform.parent = gameObject.transform;
            PlusY += 1.1f;
            moneyAmount ++;
            if (transform.childCount % AmountForNewPile == 0 )
            {
                place.x -= 4;
                PlusY = 0;
                GetComponent<BoxCollider>().center = new Vector3(GetComponent<BoxCollider>().center.x - 0.22f, GetComponent<BoxCollider>().center.y, GetComponent<BoxCollider>().center.z);
                GetComponent<BoxCollider>().size = new Vector3(GetComponent<BoxCollider>().size.x + 0.43f, GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z);
            }
        }
        // moneyAmount = transform.childCount;
        //  Debug.Log(transform.childCount + " = " + moneyAmount);
    }
}
