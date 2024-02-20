using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Vector3 offset = new Vector3(0, 8, -3);

    private void Start()
    {
        transform.position = player.transform.position + offset;
        transform.LookAt(player.transform.position);
    }

    private void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
