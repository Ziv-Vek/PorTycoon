using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public enum ForkliftTask
{
    PickupBoxes,
    UnloadBoxes
}

public enum ForkliftState
{
    Driving,
    Transfering,
    Idling
}

public interface IForkliftState
{
    public void Enter();
    public void Update();
    public void Exit();
}

[Serializable]
public class Driving : IForkliftState
{
    private ForkliftMover forkliftMover;
    private ForkliftCarrier carrier;
    private Vector3 destinationPos;
    IForkliftState transfering;
    IForkliftState idling;
    ForkliftTask task;
    Pier pier;
    Conveyor conveyor;
    bool isHaveFuel = true;
    
    public Driving(ForkliftMover forkliftMover, ForkliftCarrier forkliftCarrier) {
        this.forkliftMover = forkliftMover;
        this.carrier = forkliftCarrier;
    }

    public void Enter()
    {
        Debug.Log("enter driving state");
        isHaveFuel = true;
        forkliftMover.MoveToDestination(forkliftMover.destination);
    }

    public void Update()
    {
        forkliftMover.DecreaseFuel();

        if (!isHaveFuel)
        {
            forkliftMover.NoFuelText.transform.parent.rotation = Quaternion.EulerAngles(0, forkliftMover.NoFuelText.transform.rotation.y + forkliftMover.plus, 0);
            forkliftMover.NoFuelText.transform.parent.LookAt(GameObject.Find("Main Camera").transform);
            if (forkliftMover.FuelSlider.value <= 0 && Vector3.Distance(forkliftMover.forkliftArtTrans.position, forkliftMover.player.position) < forkliftMover.wakingDistance)
            {
                forkliftMover.FuelSlider.value = forkliftMover.FuelSlider.maxValue;
                isHaveFuel = true;
                forkliftMover.GetComponent<NavMeshAgent>().speed = ConfigManager.Instance.Config.levels[0].upgrades["forklift_speed"].levels[GameManager.Instance.forklifSpeedLevel - 1];
                forkliftMover.NoFuelText.SetActive(false);
            }
            else
            {
                return;
            }
        }


        if (forkliftMover.FuelSlider.value <= 0)
        {
            isHaveFuel = false;
            forkliftMover.GetComponent<NavMeshAgent>().speed = 0;
            return;
        }

        if (Vector3.Distance(forkliftMover.transform.position, forkliftMover.destination) < forkliftMover.GetComponent<NavMeshAgent>().stoppingDistance)
        {
            Exit();
        }
    }
        
    public void Exit()
    {
        Debug.Log("exit drivin state");
        forkliftMover.TransitionState(true, ForkliftTask.PickupBoxes, forkliftMover.conveyor, ForkliftState.Idling);
    }
}
/*
[Serializable]
public class Transfering : IForkliftState
{
    private ForkliftMover forkliftMover;
    private ForkliftCarrier carrier;
    IForkliftState driving;
    IForkliftState idling;
    ForkliftTask task;
    Pier pier;
    Conveyor conveyor;

    public Transfering(ForkliftMover forkliftMover, ForkliftCarrier forkliftCarrier) {
        this.forkliftMover = forkliftMover;
        this.carrier = forkliftCarrier;
        this.driving = driving;
        this.idling = idling;
    }

    public void Enter()
    {
        this.task = task;
        if (task == ForkliftTask.PickupBoxes)
        {
            pier = (Pier)destinationType;
        }
    }

    public void Update()
    {
        if (task == ForkliftTask.UnloadBoxes)
        {
            if (carrier.CheckCanGiveBoxes())
            {
                //forklift gives to conveyor, not empty
                return;
            }
            else     //forklift gives to conveyor, forklift is now empty
            {
                task = ForkliftTask.PickupBoxes;
                Exit();
                return;
            }
        }

        //forklift receives from pier, forklift full
        if (carrier.CheckCanReceiveBoxes())
        {
            task = ForkliftTask.UnloadBoxes;
            Exit();
            return;
        }

        //forklift receives from current pier, forklift not full
        if (pier.CheckCanGiveBoxes()) return;

        //forklift can receive, current pier cannot give
        if (!pier.CheckCanGiveBoxes())
        {
            Exit();
            return;
        }
    }*/
/*
    public void Exit()
    {
        var isDestinationPier = task == ForkliftTask.PickupBoxes ? true : false;
        if (isDestinationPier)
        {
            forkliftMover.TransitionState(isDestinationPier, task, pier, idling);
        }
        else
        {
            forkliftMover.TransitionState(isDestinationPier, task, conveyor, driving);
        }
    }
}*/

[Serializable] 
public class Idling: IForkliftState
{
    private ForkliftMover forkliftMover;
    ForkliftCarrier forkliftCarrier;
    Pier destinationPier = null;
    ForkliftTask task;

    public Idling(ForkliftCarrier forkliftCarrier, ForkliftMover forkliftMover) {
        this.forkliftCarrier = forkliftCarrier;
        this.forkliftMover = forkliftMover;
    }

    public void Enter() 
    {
        Debug.Log("enter idle state");
        destinationPier = null;
        forkliftMover.FreezeMovement();
    }

    public void Update() {
        //destinationPier = forkliftMover.SetDestinationPier();

        if (forkliftCarrier.CheckCanReceiveBoxes())
        {
            destinationPier = forkliftMover.SetDestinationPier();
            if (destinationPier != null)
            {
                task = ForkliftTask.PickupBoxes;
                Exit();
            }
            else
            {
                if (forkliftCarrier.CheckCanGiveBoxes())
                {
                    task = ForkliftTask.UnloadBoxes;
                    Exit();
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            destinationPier = null;

            task = ForkliftTask.UnloadBoxes;
            Exit();
        }
     }
        
    public void Exit()
    {
        forkliftMover.GetComponent<NavMeshAgent>().isStopped = false;
        if (task == ForkliftTask.PickupBoxes)
        {
            forkliftMover.TransitionState(true, task, destinationPier, ForkliftState.Driving);
        }
        else
        {
            forkliftMover.TransitionState(false, task, destinationPier, ForkliftState.Driving);
        }
    }
}

public class ForkliftMover : MonoBehaviour
{
    #region Class Properties
    //configs:
    private const float StoppingDistance = 2f;
    [SerializeField] private bool isPickUpBoxesTask;     // true if needed to take boxes from pier, false if needed to put boxes on conveyor
    [SerializeField] public float wakingDistance = 6;
    
    //cached ref:
    public Vector3 destination { get; private set; }
    public Conveyor conveyor;
    private ForkliftCarrier myCarrier;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    [SerializeField] public Slider FuelSlider;
    [SerializeField] public GameObject NoFuelText;
    //[SerializeField] private Transform target;
    //[SerializeField] private Transform LastTarget;
    public Transform player { get; private set; }
    public Transform forkliftArtTrans { get; private set; }
    [SerializeField] float backwardMovementSpeed = 5f;
    [SerializeField] private float backwardMovementDistance = 20f;
    [SerializeField] public float plus;
    [SerializeField] public float fuelDecreaseRate = 10f;
    private List<Pier> piers = new List<Pier>();
    ForkliftTask task;
    Vector3 destinationPos;

    GameConfig gameConfig;

    public IForkliftState CurrentState { get; private set; }
    private Driving driving;
    private Idling idling;
    //private Transfering transfering;

    #endregion

    private void Awake()
    {   
        gameConfig = ConfigManager.Instance.Config;
        myCarrier = GetComponent<ForkliftCarrier>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        forkliftArtTrans = transform.GetChild(1).transform;
    }

    private void Start()
    {
        driving = new Driving(this, myCarrier);
        idling = new Idling(myCarrier, this); 
        //transfering = new Transfering(this, myCarrier);

        var portLoader = GetComponentInParent<PortLoader>();

        if (!portLoader)
        {
            UnityEngine.Debug.Log(
                "PortLoader not found on parent GameObject. \n Please ensure Forklift is a child of the Port GameObject.");
        }
        else
        {
            player = portLoader.GetPlayer().transform;
            FindPiers(portLoader);
            FindConveyors(portLoader);
        }

        //destinationPos = this.transform.position;

        NoFuelText.SetActive(false);

        task = ForkliftTask.PickupBoxes;

        CurrentState = idling;
        CurrentState.Enter();


/*
        SetCarryTask();

        target = SelectTargetPier().GetComponent<Pier>().actionRectZone;
        
        if (myCarrier.CheckCanReceiveBoxes())
        {
            
            isPickUpBoxesTask = true;
        }
        else
        {
            isPickUpBoxesTask = false;
        }

        MoveToTarget();
        */
    }
/*
    public (bool IsDestinationPier, object DestinationType, Task Task) SetCarryTask()
    {
        if (myCarrier.CheckCanReceiveBoxes() && SelectTargetPier() != null)
        {

        }
    }*/

    public float GetStoppingDistance()
    {
        return StoppingDistance;
    }

    public Pier SetDestinationPier()
    {
        foreach (var pier in piers)
        {
            if (pier.gameObject.activeSelf && pier.CheckCanGiveBoxes())
            {
                return pier;
            }
        }
        return null;
    }
    
    public void TransitionState(bool isDestinationPier, ForkliftTask task, object destinationType, ForkliftState nextState)
    {
        destination = isDestinationPier ? ((Pier)destinationType).actionRectZone.position : ((Conveyor)destinationType).actionRectZone.position;

        switch (nextState)
        {
            case ForkliftState.Driving:
                CurrentState = driving;
                break;
            /*case ForkliftState.Transfering:
                CurrentState = transfering;
                break;*/
            case ForkliftState.Idling:
                CurrentState = idling;
                break;
        }
        
        CurrentState.Enter();
    }

    public void DecreaseFuel()
    {
        FuelSlider.value -= fuelDecreaseRate * Time.deltaTime;
    }

    public void FreezeMovement()
    {
        navMeshAgent.isStopped = true;
    }

    private void FindConveyors(PortLoader portLoader)
    {
        conveyor = portLoader.GetConveyorInPort();
        // if (conveyorBelt) conveyorBelt.gameObject.name;
    }

    private void FindPiers(PortLoader portLoader)
    {
        foreach (var ship in portLoader.GetShipsInPort())
        {
            piers.Add(ship.GetPier());
        }
    }

    private void Update()
    {
        CurrentState.Update();



        // check if forklift has reached destination
       /* if (target != null && Vector3.Distance(transform.position, target.position) < StopDistance && navMeshAgent.enabled)
        {
            CancelMovement();
        }*/

        //SetCarryingTask();
        
        
      /*  */
    }

    // checks if finished loading/unloading boxes and sets the new task accordingly
    /*public void SetCarryingTask()
    {
        if (target != null || FuelSlider.value == 0) return;

        if (myCarrier.CheckCanReceiveBoxes())
        {
            // target = SelectTargetPier().GetComponent<Pier>().actionRectZone;
            
            if (target == null)
            {
                isPickUpBoxesTask = false;
                target = conveyor;
                StartCoroutine(Move());
                return;
            }

            isPickUpBoxesTask = true;
            StartCoroutine(Move());
            return;
        }
        else
        {
            isPickUpBoxesTask = false;
            target = conveyor;
            StartCoroutine(Move());
        }
        
        // if (target == null && !myCarrier.CheckCanReceiveBoxes() && FuelSlider.value != 0)
        // {
        //     isPickUpBoxesTask = false;
        //     StartCoroutine(Move());
        //     return;
        // }
        //
        // if (target == null && !myCarrier.CheckCanGiveBoxes() && FuelSlider.value != 0)
        // {
        //     isPickUpBoxesTask = true;
        //     StartCoroutine(Move());
        // }
    }*/

    public void MoveToDestination(Vector3 destinationPos)
    {
        navMeshAgent.SetDestination(destinationPos);
        navMeshAgent.isStopped = false;
    }

    // sets movement destination and start movement
    /*IEnumerator Move()
    {
        //rb.constraints = RigidbodyConstraints.None;

        if (isPickUpBoxesTask)
        {
            target = SelectTargetPier().GetComponent<Pier>().transform;
            if (target == null) 
                target = conveyor;
        }
        else
        {
            target = conveyor; 
        }
        if(LastTarget != target)
        yield return StartCoroutine(MoveBackwards());
    }
    
    IEnumerator MoveBackwards()
    {
        navMeshAgent.enabled = false;
        Vector3 backwardTargetPos = transform.position - (transform.forward * backwardMovementDistance);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.AddForce(-transform.forward * backwardMovementSpeed, ForceMode.VelocityChange);
    
        while (Vector3.Distance(transform.position, backwardTargetPos) > 0.1f)
        {
            //rb.AddForce(-transform.forward * backwardMovementSpeed, ForceMode.Force);
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionY;

        navMeshAgent.enabled = true;
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if (!target)
        {
            Debug.Log("no target destination set for forklift");
            target = SelectTargetPier().GetComponent<Pier>().actionRectZone;
        }
        
        navMeshAgent.SetDestination(target.position);
        if (LastTarget != target)
        {
            FuelSlider.value -= 10;
            if (FuelSlider.value == 0)
            {
                GetComponent<NavMeshAgent>().speed = 0;
                NoFuelText.SetActive(true);
            }
        }
        LastTarget = target;
        navMeshAgent.isStopped = false;
    }*/

    // stops movement on navmesh and freezes position
/*    private void CancelMovement()
    {
        target = null;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
        
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }*/
    
    public void FuelUpgrade(int amount)
    {
        FuelSlider.maxValue = amount;
        FuelSlider.value = FuelSlider.maxValue;
        GetComponent<NavMeshAgent>().speed = ConfigManager.Instance.Config.levels[0].upgrades["forklift_speed"].levels[GameManager.Instance.forklifSpeedLevel - 1];
        NoFuelText.SetActive(false);
    }
}


