using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AssistantMover : MonoBehaviour
{
    [SerializeField] private Transform pier;
    [SerializeField] private Transform conveyorBelt;
    
    private AssistantCarrier myCarrier;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        myCarrier = GetComponent<AssistantCarrier>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (myCarrier.CheckCanReceiveBoxes())
        {
            
        }
    }
}
