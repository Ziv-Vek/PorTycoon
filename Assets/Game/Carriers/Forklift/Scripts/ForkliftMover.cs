using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    [SerializeField] public Slider FuelSlider;
    [SerializeField] GameObject NoFuelText;
    private Transform target;
    private Transform LastTarget;

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
        }
        else
        {
            isPickUpBoxesTask = false;
        }

        Move();
        NoFuelText.SetActive(false);
      
    }

    private void Update()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) < StopDistance)
        {
            CancelMovement();
        }

        SetCarryingTask();
        if (Vector3.Distance(transform.GetChild(0).transform.position, GameObject.Find("Player").transform.position) < 6 && FuelSlider.value <= 0)
        {
            FuelSlider.value = FuelSlider.maxValue;
            NoFuelText.SetActive(false);
        }
    }

    // checks if finished loading/unloading boxes and sets the new task accordingly
    private void SetCarryingTask()
    {
        if (target == null && !myCarrier.CheckCanReceiveBoxes() && FuelSlider.value != 0)
        {
            isPickUpBoxesTask = false;
            Move();
            return;
        }

        if (target == null && !myCarrier.CheckCanGiveBoxes() && FuelSlider.value != 0)
        {
            isPickUpBoxesTask = true;
            Move();
        }
    }

    // sets movement destination and start movement
    private void Move()
    {
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
        if (LastTarget != target)
        {
            FuelSlider.value -= 10;
            if (FuelSlider.value == 0)
                NoFuelText.SetActive(true);
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
        NoFuelText.SetActive(false);
    }
}
