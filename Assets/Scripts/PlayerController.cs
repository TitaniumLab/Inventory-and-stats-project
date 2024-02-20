using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100;
        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mousePos);
        RaycastHit hit;
        if (Input.GetMouseButton(0) && Physics.Raycast(Camera.main.transform.position, mouseInWorld, out hit, 100))
        {
            agent.destination = hit.point;
        }
    }
}
