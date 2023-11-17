using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public GameObject Splash;
    [SerializeField] GameObject[] FishedObjects;


    [SerializeField] ScratchBoard scratch;
    [SerializeField] int startDelay;
    [SerializeField] int Delay;

    [SerializeField] int AmountOfMoney;
    [SerializeField] int AmountOfStars;

    public void StartInvoke()
    {
        InvokeRepeating("MakeSplash",startDelay, Delay);
    }
    public void MakeSplash()
   {
        float randomFloatX = (float)(new System.Random().NextDouble() * (-1046.54 + 1057.63) - 1057.63);
        float randomFloatZ = (float)(new System.Random().NextDouble() * (263.85 - 294.1) + 294.1);
        Instantiate(Splash, new Vector3(randomFloatX, 0, randomFloatZ), Quaternion.identity).transform.SetParent(transform);
    }
    public void ObjectFished(Vector3 position)
    {
        Debug.Log("pressed");
        GetComponent<AudioSource>().Play();
        StartCoroutine(MoveCoroutine(position));
        CancelInvoke();
    }

    private IEnumerator MoveCoroutine(Vector3 pos)
    {
        int Random = new System.Random().Next(FishedObjects.Length);
        GameObject Clone = Instantiate(FishedObjects[Random] ,new Vector3(pos.x,pos.y - 3, pos.z),Quaternion.EulerAngles(0,0,0));
        Clone.transform.SetParent(transform);
        Clone.name = FishedObjects[Random].name;

        Clone.transform.Find("background").transform.LookAt(transform.Find("Fishing Camera").transform);

        Vector3 targetPos = new Vector3(pos.x, pos.y + 4, pos.z);

        while (Vector3.Distance(Clone.transform.position, targetPos) > 0.2f & Vector3.Distance(Clone.transform.GetChild(0).rotation.eulerAngles, Quaternion.ToEulerAngles(Quaternion.EulerAngles(0,180,0))) > 0.2f)
        {
            // Calculate the new position using Lerp
            Vector3 newPosition = Vector3.Lerp(Clone.transform.position, targetPos, 1.5f * Time.deltaTime);
            Clone.transform.GetChild(0).rotation = Quaternion.Lerp(Clone.transform.GetChild(0).rotation, Quaternion.EulerAngles(0, 360, 0) ,1 * Time.deltaTime);

            Clone.transform.position = newPosition;

            Clone.transform.Find("background").transform.Rotate(new Vector3(0, 0, 40) * Time.deltaTime);
            yield return null;
        }
        Invoke(nameof(ActiveObject), 1.2f);
    }
    private void ActiveObject()
    {
        GameObject Clone = transform.GetChild(transform.childCount - 1).gameObject;
        if (Clone.name == "Box")
        {
            BuyingBox1();
        }
        else if(Clone.name == "Money")
        {
            Bank.Instance.DepositMoney(AmountOfMoney);
        }
        else if(Clone.name == "Star")
        {
            Bank.Instance.DepositStars(AmountOfStars);
        }
        BackToPort();
    }
    public void BackToPort()
    {
        GameObject Clone = transform.GetChild(transform.childCount - 1).gameObject;
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        if (Clone.name != "Box" || FindInArrayByName(Clone) == null)
        {
            playerMover.ToggleMovement(true);
            playerMover.ShowJoystick();
        }
        if (FindInArrayByName(Clone) != null )
            Destroy(Clone);
        gameObject.SetActive(false);
    }
    public void BuyingBox1()
    {
        var newBox = new PortBox();
        newBox.isPurchasedBox = true;
        newBox.Awake();
        scratch.Open(newBox);
    }
    public GameObject FindInArrayByName(GameObject obj)
    {
        foreach(GameObject gameObject in FishedObjects)
        {
            if(gameObject.name == obj.name)
                return gameObject;
        }
        return null;
    }
}
