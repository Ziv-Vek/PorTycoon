using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public GameObject Splash;
    [SerializeField] GameObject[] FishedObjects;


    [SerializeField] ScratchBoard scratch;
    [SerializeField] public int startDelay;
    [SerializeField] public float Delay;

    [SerializeField] public float StartOpportunityTime;
    [SerializeField] public float OpportunityTime;

    [SerializeField] public int AmountOfMoney;
    [SerializeField] public int AmountOfStars;

    [SerializeField] public float MinusDelay;
    [SerializeField] public float MinusOpportunity;

    public int Step;

    public void StartInvoke()
    {
        // InvokeRepeating("MakeSplash",startDelay, Delay);
        OpportunityTime = StartOpportunityTime;
        Delay = startDelay;
        Invoke("MakeSplash", Delay);
        Step = 0;
    }
    public void MakeSplash()
   {
        float randomFloatX = (float)(new System.Random().NextDouble() * (-1046.54 + 1057.63) - 1057.63);
        float randomFloatZ = (float)(new System.Random().NextDouble() * (263.85 - 294.1) + 294.1);
        Instantiate(Splash, new Vector3(randomFloatX, 0, randomFloatZ), Quaternion.identity, transform).GetComponent<Splash>().opportunity = OpportunityTime;
        Step++;
        OpportunityTime -= MinusOpportunity;
        Delay -= MinusDelay;
    }
    public void ObjectFished(Vector3 position)
    {
        Debug.Log("pressed");
        StartCoroutine(RopeToObject(position,true));
        CancelInvoke();
    }
    private IEnumerator RopeToObject(Vector3 position, bool FishedSomething)
    {
        LineRenderer Rope = transform.Find("Rope").GetComponent<LineRenderer>();
        Vector3 Target = position;
        if (FishedSomething)
            Target += new Vector3(0, 2.3f, 0);

        while (Vector3.Distance(Target, Rope.GetPosition(2)) > 0.15f)
        {
            // Calculate the new position using Lerp
            Vector3 newPosition = Vector3.Lerp(Rope.GetPosition(2), Target, 6f * Time.deltaTime);
            Rope.SetPosition(2, newPosition);
            yield return null;
        }   
     //   GetComponent<AudioSource>().Play();
        if(FishedSomething)
        StartCoroutine(ShowObject(position));
    }

    private IEnumerator ShowObject(Vector3 pos)
    {
        int Random = 0;
        if(Step < 7)
           Random = new System.Random().Next(1,FishedObjects.Length);
        GameObject Clone = Instantiate(FishedObjects[Random] ,new Vector3(pos.x,pos.y - 3, pos.z),Quaternion.EulerAngles(0,0,0));
        Clone.transform.SetParent(transform);
        Clone.name = FishedObjects[Random].name;

        Clone.transform.Find("background").transform.LookAt(transform.Find("Fishing Camera").transform);
        Clone.GetComponent<AudioSource>().Play();

        LineRenderer Rope = transform.Find("Rope").GetComponent<LineRenderer>();
        Vector3 targetPos = new Vector3(pos.x, pos.y + 4, pos.z);

        while (Vector3.Distance(Clone.transform.position, targetPos) > 0.2f && Vector3.Distance(Clone.transform.GetChild(0).rotation.eulerAngles, Quaternion.ToEulerAngles(Quaternion.EulerAngles(0,180,0))) > 0.2f)
        {
            // Calculate the new position using Lerp
            Vector3 newPosition = Vector3.Lerp(Clone.transform.position, targetPos, 2f * Time.deltaTime);
            Clone.transform.GetChild(0).rotation = Quaternion.Lerp(Clone.transform.GetChild(0).rotation, Quaternion.EulerAngles(0, 360, 0) ,1 * Time.deltaTime);

            Clone.transform.position = newPosition;

            Clone.transform.Find("background").transform.Rotate(new Vector3(0, 0, 40) * Time.deltaTime);

            Rope.SetPosition(2,Vector3.Lerp(Clone.transform.position + new Vector3(0, 2.3f, 0), Rope.GetPosition(2), 3f * Time.deltaTime));
            yield return null;
        }
        StartCoroutine(MoveObjectToPlayer(Clone));
    }
    private IEnumerator MoveObjectToPlayer(GameObject Object)
    {    
        StartCoroutine(RopeToObject(transform.Find("Rope").GetComponent<LineRenderer>().GetPosition(1),false));
        Transform CameraPosition = transform.Find("Fishing Camera");

        while (Vector3.Distance(CameraPosition.position - new Vector3(0, 12.5f, 8), Object.transform.position) > 0.5f)
        {
            // Calculate the new position using Lerp
            Vector3 newPosition = Vector3.Lerp(Object.transform.position, CameraPosition.position - new Vector3(0, 12.5f, 8),2.8f * Time.deltaTime);
            Object.transform.position =  newPosition;
            yield return null;
        }
        ActiveObject();
        Destroy(Object);
    }
    private void ActiveObject()
    {
        GameObject Clone = transform.GetChild(transform.childCount - 1).gameObject;
        if (Clone.name == "Box")
        {
           BuyingBox1();
           BackToPort();
        }
        else if(Clone.name == "Money")
        {
            Bank.Instance.DepositMoney(AmountOfMoney);
        }
        else if(Clone.name == "Star")
        {
            Bank.Instance.DepositStars(AmountOfStars);
        }  
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
        LineRenderer Rope = transform.Find("Rope").GetComponent<LineRenderer>();
        Rope.SetPosition(2, Rope.GetPosition(1));
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
