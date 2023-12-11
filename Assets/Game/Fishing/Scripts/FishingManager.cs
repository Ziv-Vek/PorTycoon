using System.Collections;
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
        Invoke(nameof(MakeSplash), Delay);
        Step = 0;
    }

    public void MakeSplash()
    {
        float randomFloatX = (float)(new System.Random().NextDouble() * (-1046.54 + 1057.63) - 1057.63);
        float randomFloatZ = (float)(new System.Random().NextDouble() * (263.85 - 294.1) + 294.1);
        Instantiate(Splash, new Vector3(randomFloatX, 0, randomFloatZ), Quaternion.identity, transform)
            .GetComponent<Splash>().opportunity = OpportunityTime;
        Step++;
        OpportunityTime -= MinusOpportunity;
        Delay -= MinusDelay;
        VibrationManager.Instance.HeavyVibrate();
    }

    public void ObjectFished(Vector3 position)
    {
        Debug.Log("pressed");
        StartCoroutine(RopeToObject(position, true));
        CancelInvoke();
    }

    private IEnumerator RopeToObject(Vector3 position, bool fishedSomething)
    {
        LineRenderer rope = transform.Find("Rope").GetComponent<LineRenderer>();
        Vector3 target = position;
        if (fishedSomething)
            target += new Vector3(0, 2.3f, 0);

        while (Vector3.Distance(target, rope.GetPosition(2)) > 0.1f)
        {
            // Calculate the new position using Lerp
            Vector3 newPosition = Vector3.Lerp(rope.GetPosition(2), target, 7f * Time.deltaTime);
            rope.SetPosition(2, newPosition);
            yield return null;
        }

        //   GetComponent<AudioSource>().Play();
        if (fishedSomething)
            StartCoroutine(ShowObject(position));
    }

    private IEnumerator ShowObject(Vector3 pos)
    {
        int random = 0;
        if (Step < 7)
            random = new System.Random().Next(1, FishedObjects.Length);
        GameObject clone = Instantiate(FishedObjects[random], new Vector3(pos.x, pos.y - 3, pos.z),
            Quaternion.EulerAngles(0, 0, 0));
        clone.transform.SetParent(transform);
        clone.name = FishedObjects[random].name;

        clone.transform.Find("background").transform.LookAt(transform.Find("Fishing Camera").transform);
        clone.GetComponent<AudioSource>().Play();

        LineRenderer rope = transform.Find("Rope").GetComponent<LineRenderer>();
        Vector3 targetPos = new Vector3(pos.x, pos.y + 4, pos.z);

        while (Vector3.Distance(clone.transform.position, targetPos) > 0.2f && Vector3.Distance(
                   clone.transform.GetChild(0).rotation.eulerAngles,
                   Quaternion.ToEulerAngles(Quaternion.EulerAngles(0, 180, 0))) > 0.2f)
        {
            // Calculate the new position using Lerp
            Vector3 newPosition = Vector3.Lerp(clone.transform.position, targetPos, 2f * Time.deltaTime);
            clone.transform.GetChild(0).rotation = Quaternion.Lerp(clone.transform.GetChild(0).rotation,
                Quaternion.EulerAngles(0, 360, 0), 1 * Time.deltaTime);

            clone.transform.position = newPosition;

            clone.transform.Find("background").transform.Rotate(new Vector3(0, 0, 40) * Time.deltaTime);

            rope.SetPosition(2,
                Vector3.Lerp(clone.transform.position + new Vector3(0, 2.3f, 0), rope.GetPosition(2),
                    3f * Time.deltaTime));
            yield return null;
        }

        StartCoroutine(MoveObjectToPlayer(clone));
    }

    private IEnumerator MoveObjectToPlayer(GameObject Object)
    {
        StartCoroutine(RopeToObject(transform.Find("Rope").GetComponent<LineRenderer>().GetPosition(1), false));
        Transform cameraPosition = transform.Find("Fishing Camera");

        while (Vector3.Distance(cameraPosition.position - new Vector3(0, 12.5f, 8), Object.transform.position) > 0.5f)
        {
            // Calculate the new position using Lerp
            Vector3 newPosition = Vector3.Lerp(Object.transform.position,
                cameraPosition.position - new Vector3(0, 12.5f, 8), 2.8f * Time.deltaTime);
            Object.transform.position = newPosition;
            yield return null;
        }

        ActiveObject();
        Destroy(Object);
    }

    private void ActiveObject()
    {
        GameObject clone = transform.GetChild(transform.childCount - 1).gameObject;
        if (clone.name == "Box")
        {
            BuyingBox1();
            BackToPort();
        }
        else if (clone.name == "Money")
        {
            Bank.Instance.DepositMoney(AmountOfMoney);
        }
        else if (clone.name == "Star")
        {
            Bank.Instance.DepositStars(AmountOfStars);
        }
    }

    public void BackToPort()
    {
        GameObject clone = transform.GetChild(transform.childCount - 1).gameObject;
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        if (clone.name != "Box" || FindInArrayByName(clone) == null)
        {
            playerMover.ToggleMovement(true);
            playerMover.ShowJoystick();
            playerMover.joystick.DeactivateJoystick();
        }

        if (FindInArrayByName(clone) != null)
            Destroy(clone);
        LineRenderer rope = transform.Find("Rope").GetComponent<LineRenderer>();
        rope.SetPosition(2, rope.GetPosition(1));

        CancelInvoke(nameof(MakeSplash));
        AudioManager.Instance.ChangeSounds("Fishing Music", "General Music");
        gameObject.SetActive(false);
    }

    public void BuyingBox1()
    {
        var newBox = Splash.AddComponent<PortBox>();
        newBox.isPurchasedBox = true;
        newBox.Awake();
        scratch.Open(newBox);
    }

    public GameObject FindInArrayByName(GameObject obj)
    {
        foreach (GameObject g in FishedObjects)
        {
            if (g.name == obj.name)
                return g;
        }

        return null;
    }
}