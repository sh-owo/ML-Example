using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public GameObject camPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = camPos.transform.position;
        Vector3 rot = camPos.transform.rotation.eulerAngles;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
    }
}
