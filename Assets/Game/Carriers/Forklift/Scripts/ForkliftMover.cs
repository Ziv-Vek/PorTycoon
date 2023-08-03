using UnityEngine;
using UnityEngine.AI;

public class ForkliftMover : MonoBehaviour
{
    //configs:
    private const float StopDistance = 1.2f;
    private bool isPickUpBoxesTask;     // true if needed to take boxes from pier, false if needed to put boxes on conveyor 
    
    //cached ref:
    [SerializeField] private Transform pier;
    [SerializeField] private Transform conveyorBelt;
    private ForkliftCarrier myCarrier;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    
    private Transform target;

    private void Awake()
    {
        myCarrier = GetComponent<ForkliftCarrier>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (myCarrier.CheckCanReceiveBoxes())
        {
            
            isPickUpBoxesTask = true;
            //target = pier;
        }
        else
        {
            isPickUpBoxesTask = false;
            //target = conveyorBelt;
        }

        Move();
    }

    private void Update()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) < StopDistance)
        {
            CancelMovement();
        }

        SetCarryingTask();
    }

    // checks if finished loading/unloading boxes and sets the new task accordingly
    private void SetCarryingTask()
    {
        if (target == null && !myCarrier.CheckCanReceiveBoxes())
        {
            isPickUpBoxesTask = false;
            Move();
            return;
        }

        if (target == null && !myCarrier.CheckCanGiveBoxes())
        {
            isPickUpBoxesTask = true;
            Move();
        }
    }

    // sets movement destination and start movement
    private void Move()
    {
        // if target != null return;

        Debug.Log("move", target);

        rb.constraints = RigidbodyConstraints.None;

        if (isPickUpBoxesTask)
        {
            target = pier;
        }
        else
        {
            target = conveyorBelt;
        }
        
        navMeshAgent.SetDestination(target.position);
        navMeshAgent.isStopped = false;
    }

    // stops movement on navmesh and freezes position
    private void CancelMovement()
    {
        Debug.Log("movement canceled");
        target = null;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
        
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
}
