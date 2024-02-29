using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField]
    private UiManager uiManager;
    private bool isMoving = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !uiManager.IsCursorOnUI())
            isMoving = true;
        if (Input.GetMouseButtonUp(0))
            isMoving = false;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 100;
            Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mousePos);
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, mouseInWorld, out hit);
            if (hit.collider != null)
            {
                agent.destination = hit.point;
            }
        }
    }
}
