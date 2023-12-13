using System;
using System.Collections.Generic;
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
    private Vector3 destinationPos;
    bool isHaveFuel = true;
    private int CurrentLevel => forkliftMover.transform.parent.GetComponent<PortLoader>().PortLevel;

    public Driving(ForkliftMover forkliftMover, ForkliftCarrier forkliftCarrier) {
        this.forkliftMover = forkliftMover;
    }

    public void Enter()
    {
        isHaveFuel = true;
        forkliftMover.MoveToDestination(forkliftMover.destination);
    }

    public void Update()
    {
        forkliftMover.DecreaseFuel();

        if (!isHaveFuel)
        {
            forkliftMover.NoFuelText.transform.parent.rotation = Quaternion.EulerAngles(0, forkliftMover.NoFuelText.transform.rotation.y + forkliftMover.plus, 0);
            forkliftMover.NoFuelText.transform.parent.LookAt(forkliftMover.mainCamera.transform);
            if (Vector3.Distance(forkliftMover.forkliftArtTrans.position, forkliftMover.player.position) < forkliftMover.wakingDistance)
            {
                HandleRefuel();
            }
            else
            {
                return;
            }
        }

        if (forkliftMover.FuelSlider.value <= 0)
        {
            isHaveFuel = false;
            forkliftMover.HornSorce.Play();
            forkliftMover.NoFuelText.SetActive(true);
            forkliftMover.GetComponent<NavMeshAgent>().speed = 0;
            return;
        }

        if (Vector3.Distance(forkliftMover.transform.position, forkliftMover.destination) < 
            forkliftMover.GetComponent<NavMeshAgent>().stoppingDistance)
        {
            Exit();
        }
    }

    public void HandleRefuel()
    {
        forkliftMover.FuelSlider.value = forkliftMover.FuelSlider.maxValue;
        isHaveFuel = true;
        forkliftMover.GetComponent<NavMeshAgent>().speed = ConfigManager.Instance.Config.levels[GameManager.Instance.level - 1]
        .upgrades["forklift_speed"]
        .levels[GameManager.Instance.LevelsData["Port" + CurrentLevel].forklifSpeedLevel - 1];
        forkliftMover.NoFuelText.SetActive(false);
        forkliftMover.GasRefillSorce.Play();
    }

    public void Exit()
    {
        forkliftMover.TransitionState(destinationPos, ForkliftState.Idling);
    }
}

[Serializable] 
public class Idling: IForkliftState
{
    private ForkliftMover forkliftMover;
    ForkliftCarrier forkliftCarrier;
    Pier destinationPier = null;
    Vector3 destination;

    public Idling(ForkliftCarrier forkliftCarrier, ForkliftMover forkliftMover) {
        this.forkliftCarrier = forkliftCarrier;
        this.forkliftMover = forkliftMover;
    }

    public void Enter() 
    {
        destinationPier = null;
        forkliftMover.FreezeMovement();
    }

    public void Update() {
        if (forkliftCarrier.CheckCanReceiveBoxes())
        {
            destinationPier = forkliftMover.SetDestinationPier();
            if (destinationPier != null)    // drive to a different pier
            {
                destination = destinationPier.actionRectZone.position;
                Exit();
            }
            else
            {
                if (forkliftCarrier.CheckCanGiveBoxes())    // drive to conveyor
                {
                    destination = forkliftMover.conveyor.actionRectZone.position;
                    Exit();
                }
                else
                {
                    return;
                }
            }
        }
        else // drive to conveyor
        {
            destination = forkliftMover.conveyor.actionRectZone.position;

            Exit();
        }
     }
        
    public void Exit()
    {
        forkliftMover.GetComponent<NavMeshAgent>().isStopped = false;
        forkliftMover.TransitionState(destination, ForkliftState.Driving);
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
    [SerializeField] public Slider FuelSlider;
    [SerializeField] public GameObject NoFuelText;
    public Transform player { get; private set; }
    public Transform forkliftArtTrans { get; private set; }
    [SerializeField] float backwardMovementSpeed = 5f;
    [SerializeField] private float backwardMovementDistance = 20f;
    [SerializeField] public float plus;
    [SerializeField] public float fuelDecreaseRate = 10f;
    private List<Pier> piers = new List<Pier>();
    public IForkliftState CurrentState { get; private set; }
    private Driving driving;
    private Idling idling;
    public AudioSource HornSorce;
    public AudioSource GasRefillSorce;
    public Camera mainCamera { get; private set; }
    #endregion

    private int CurrentLevel => transform.parent.GetComponent<PortLoader>().PortLevel;

    private void Awake()
    {   
        myCarrier = GetComponent<ForkliftCarrier>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        forkliftArtTrans = transform.GetChild(1).transform;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        driving = new Driving(this, myCarrier);
        idling = new Idling(myCarrier, this); 

        var portLoader = GetComponentInParent<PortLoader>();

        if (!portLoader)
        {
            Debug.Log(
                "PortLoader not found on parent GameObject. \n Please ensure Forklift is a child of the Port GameObject.");
        }
        else
        {
            player = portLoader.GetPlayer().transform;
            FindPiers(portLoader);
            FindConveyors(portLoader);
        }

        NoFuelText.SetActive(false);

        CurrentState = idling;
        CurrentState.Enter();
    }

    private void Update()
    {
        CurrentState.Update();
    }

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

    public void TransitionState(Vector3 newDestination, ForkliftState nextState)
    {
        destination = newDestination;

        switch (nextState)
        {
            case ForkliftState.Driving:
                CurrentState = driving;
                break;
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
    }

    private void FindPiers(PortLoader portLoader)
    {
        foreach (var ship in portLoader.GetShipsInPort())
        {
            piers.Add(ship.GetPier());
        }
    }

    public void MoveToDestination(Vector3 destinationPos)
    {
        navMeshAgent.SetDestination(destinationPos);
        navMeshAgent.isStopped = false;
    }

    public void FuelUpgrade(int amount)
    {
        if (driving == null)
        {
            GetComponent<NavMeshAgent>().speed = ConfigManager.Instance.Config.levels[GameManager.Instance.level - 1]
        .upgrades["forklift_speed"]
        .levels[GameManager.Instance.LevelsData["Port" + CurrentLevel].forklifSpeedLevel - 1];
            FuelSlider.maxValue = amount;
        }
        else
        {
            FuelSlider.maxValue = amount;
            driving.HandleRefuel();
        }
    }
}


