using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ForkliftMover : MonoBehaviour
{
    //configs:
    private const float StopDistance = 2f;
   [SerializeField] private bool isPickUpBoxesTask;     // true if needed to take boxes from pier, false if needed to put boxes on conveyor
    [SerializeField] private float wakingDistance = 6;
    
    //cached ref:
    [SerializeField] private Transform pier;
    [SerializeField] private Transform conveyorBelt;
    private ForkliftCarrier myCarrier;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    [SerializeField] public Slider FuelSlider;
    [SerializeField] GameObject NoFuelText;
    [SerializeField] private Transform target;
    [SerializeField] private Transform LastTarget;
    private Transform player;
    private Transform forkliftArtTrans;
    [SerializeField] float backwardMovementSpeed = 5f;
    [SerializeField] private float backwardMovementDistance = 20f;
    [SerializeField] float plus;
    private List<Pier> piers = new List<Pier>();
    
    GameConfig gameConfig;

    private void Awake()
    {      
        gameConfig = ConfigManager.Instance.Config;
        myCarrier = GetComponent<ForkliftCarrier>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        forkliftArtTrans = transform.GetChild(1).transform;
    }

    private void Start()
    {
        var portLoader = GetComponentInParent<PortLoader>();
        if (!portLoader)
        {
            Debug.LogError(
                "PortLoader not found on parent GameObject. \n Please ensure Forklift is a child of the Port GameObject.");
        }
        else
        {
            FindPiers(portLoader);
            FindConveyors(portLoader);
        }

        target = pier;
        
        if (myCarrier.CheckCanReceiveBoxes())
        {
            
            isPickUpBoxesTask = true;
        }
        else
        {
            isPickUpBoxesTask = false;
        }

        MoveToTarget();
        NoFuelText.SetActive(false);
    }

    private void FindConveyors(PortLoader portLoader)
    {
        conveyorBelt = portLoader.GetConveyorInPort().transform;
    }

    private void FindPiers(PortLoader portLoader)
    {
        foreach (var ship in portLoader.GetShipsInPort())
        {
            piers.Add(ship.GetPier());
        }
    }
    
    /* SetTransferTask() >>
     * check if I can receive boxes:
     *   yes >>
     *      check in each pier if it can give boxes.
     *      for first pier which can - use this pier, exit early
     *      Move(the pier)
     *   no >>
     *      Move(the conveyor belt)
     *
     * 
     *  
     */

    private void Update()
    {
        // check if forklift has reached destination
        if (target != null && Vector3.Distance(transform.position, target.position) < StopDistance && navMeshAgent.enabled)
        {
            CancelMovement();
        }

        SetCarryingTask();
        if (Vector3.Distance(forkliftArtTrans.position, player.position) < wakingDistance && FuelSlider.value <= 0)
        {
            FuelSlider.value = FuelSlider.maxValue;
            GetComponent<NavMeshAgent>().speed = gameConfig.levels[0].upgrades["forklift_speed"].levels[GameManager.Instance.forklifSpeedLevel - 1];
            NoFuelText.SetActive(false);
        }
        NoFuelText.transform.parent.LookAt(GameObject.Find("Main Camera").transform);
        NoFuelText.transform.parent.rotation = Quaternion.EulerAngles(0, NoFuelText.transform.rotation.y + plus, 0);
    }

    // checks if finished loading/unloading boxes and sets the new task accordingly
    public void SetCarryingTask()
    {
        if ( target == null && !myCarrier.CheckCanReceiveBoxes() && FuelSlider.value != 0)
        {
            isPickUpBoxesTask = false;
            StartCoroutine(Move());
            return;
        }

        if (target == null && !myCarrier.CheckCanGiveBoxes() && FuelSlider.value != 0)
        {
            isPickUpBoxesTask = true;
            StartCoroutine(Move());
        }
    }

    // sets movement destination and start movement
    IEnumerator Move()
    {
        //rb.constraints = RigidbodyConstraints.None;

        if (isPickUpBoxesTask)
        {
            target = pier;
        }
        else
        {
            target = conveyorBelt; 
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
            target = pier;
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
    }

    // stops movement on navmesh and freezes position
    private void CancelMovement()
    {
        target = null;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
        
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
    
    public void FuelUpgrade(int amount)
    {
        FuelSlider.maxValue = amount;
        FuelSlider.value = FuelSlider.maxValue;
        GetComponent<NavMeshAgent>().speed = ConfigManager.Instance.Config.levels[0].upgrades["forklift_speed"].levels[GameManager.Instance.forklifSpeedLevel - 1];
        NoFuelText.SetActive(false);
    }
}
