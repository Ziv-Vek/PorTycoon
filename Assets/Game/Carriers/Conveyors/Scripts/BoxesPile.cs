using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using TMPro;

public class BoxesPile : MonoBehaviour
{
    [FormerlySerializedAs("moneyAmount")] [SerializeField]
    public int boxAmountInStack = 1;

    [FormerlySerializedAs("AmountForNewPile")] [SerializeField]
    int amountForNewPile;

    [FormerlySerializedAs("PlusY")] [SerializeField]
    float plusY;

    public Vector3 place;
    [FormerlySerializedAs("moneyLimit")] public int boxesLimit;
    private List<PortBox> boxes = new List<PortBox>();
    [SerializeField] private Vector3 horizontalStep;
    [SerializeField] private Vector3 verticalStep;
    [SerializeField] private Vector3 boxesPositionOrigin;

    [SerializeField] int LimitBoxesThatShown;
    [SerializeField] private Transform pile;
    [SerializeField] TextMeshPro PlusText;

    void Start()
    {
        place = pile.transform.localPosition;
        plusY = 0;
    }

    public void AddBoxToPile(PortBox box)
    {
        boxes.Add(box);
        box.transform.SetParent(pile);
        box.transform.localPosition = new Vector3(0, plusY, 0);
        box.transform.rotation = box.transform.parent.rotation;
        plusY += 5;

        if (boxes.Count > LimitBoxesThatShown)
        {
            if (!PlusText.IsActive())
            {
                PlusText.gameObject.SetActive(true);
            }

            PlusText.text = "+" + (boxes.Count - LimitBoxesThatShown);
            box.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public PortBox TakeBoxFromPile()
    {
        var boxesCount = boxes.Count;

        if (boxesCount > 0)
        {
            var box = boxes[boxesCount - 1];
            boxes.RemoveAt(boxesCount - 1);
            plusY -= 5;
            if (boxes.Count <= LimitBoxesThatShown)
            {
                if (PlusText.IsActive())
                    PlusText.gameObject.SetActive(false);
            }

            PlusText.text = "+" + (boxes.Count - LimitBoxesThatShown);
            box.transform.GetChild(0).gameObject.SetActive(true);

            return box;
        }

        return null;
    }

    public void CancelTakingBoxes()
    {
        CancelInvoke();
    }

    void TakingOneByOne()
    {
        transform.GetChild(transform.childCount - 1).gameObject.GetComponent<MoneyPrefab>().targetPosition =
            GameObject.Find("Player").transform;
        transform.GetChild(transform.childCount - 1).gameObject.GetComponent<MoneyPrefab>().startMove = true;
        transform.GetChild(transform.childCount - 1).parent = null;
        if (plusY - 1.1 >= 0)
            plusY -= 1.1f;
        boxAmountInStack--;

        if (transform.childCount % amountForNewPile == 0)
        {
            if (boxAmountInStack != 0)
            {
                plusY = 0;
                place.x += 4;
                GetComponent<BoxCollider>().center = new Vector3(GetComponent<BoxCollider>().center.x + 0.22f,
                    GetComponent<BoxCollider>().center.y, GetComponent<BoxCollider>().center.z);
                GetComponent<BoxCollider>().size = new Vector3(GetComponent<BoxCollider>().size.x - 0.43f,
                    GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z);
            }
        }

        if (boxAmountInStack == 0)
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
            if (boxAmountInStack >= boxesLimit)
                break;
            // GameObject Money = Instantiate(moneyPrefab, new Vector3(place.x, place.y + plusY, place.z), Quaternion.identity);
            // Money.transform.parent = gameObject.transform;
            plusY += 1.1f;
            boxAmountInStack++;
            if (transform.childCount % amountForNewPile == 0)
            {
                place.x -= 4;
                plusY = 0;
                GetComponent<BoxCollider>().center = new Vector3(GetComponent<BoxCollider>().center.x - 0.22f,
                    GetComponent<BoxCollider>().center.y, GetComponent<BoxCollider>().center.z);
                GetComponent<BoxCollider>().size = new Vector3(GetComponent<BoxCollider>().size.x + 0.43f,
                    GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z);
            }
        }
        // moneyAmount = transform.childCount;
        //  Debug.Log(transform.childCount + " = " + moneyAmount);
    }
}