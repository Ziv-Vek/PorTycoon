using UnityEngine;
using UnityEngine.AI;

public class AssistantMover : MonoBehaviour
{
    [SerializeField] private Transform pier;
    [SerializeField] private Transform conveyorBelt;
    
    private AssistantCarrier myCarrier;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;

    [SerializeField] private Transform target;  // serialized for debugging

    private bool isPickUpBoxesTask;

    private void Awake()
    {
        myCarrier = GetComponent<AssistantCarrier>();
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
        if (target != null && Vector3.Distance(transform.position, target.position) < 1f)
        {
            CancelMovement();
        }

        SetCarryingTask();
    }

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

    private void CancelMovement()
    {
        Debug.Log("movement canceled");
        target = null;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
        
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }
}
