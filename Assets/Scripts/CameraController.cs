using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject robotPlayer;
    // Start is called before the first frame update
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - robotPlayer.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = robotPlayer.transform.position + offset;
    }
}
