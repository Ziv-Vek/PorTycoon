using System;
using System.Collections;
using Unity.Mathematics;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ForkliftMover : MonoBehaviour
{
    //configs:
    private const float StopDistance = 2f;
    private bool isPickUpBoxesTask;     // true if needed to take boxes from pier, false if needed to put boxes on conveyor
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
    }

    // checks if finished loading/unloading boxes and sets the new task accordingly
    private void SetCarryingTask()
    {
        /* states:
         needs to pickup, and can still pickup
         needs to pickup, and cannot pickup
         needs to unload and can still unload
         needs to unload and cannnot unload*/
        if (isPickUpBoxesTask && !myCarrier.CheckCanReceiveBoxes() && FuelSlider.value != 0)
        
        if (target == null && !myCarrier.CheckCanReceiveBoxes() && FuelSlider.value != 0)
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
        GetComponent<NavMeshAgent>().speed = gameConfig.levels[0].upgrades["forklift_speed"].levels[GameManager.Instance.forklifSpeedLevel - 1];
        NoFuelText.SetActive(false);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.GetChild(1).position, wakingDistance);
    }
}
