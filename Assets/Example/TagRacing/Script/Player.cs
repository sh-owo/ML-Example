using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    
    private float speed = 10f;
    private float rotationSpeed = 200f;
    private float jumpForce = 300f;
    public Vector3 startPos;
    public Quaternion startRot;
    
    private bool isOnGround = true;

    public Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        this.startPos = this.transform.position;
        this.startRot = this.transform.rotation;
    }

    public void Move(int moveX, int rotation)
    {
        int moveVar = 0;
        switch (moveX)
        {
            case 0:
                moveVar = 0;
                break;
            case 1:
                moveVar = 1;
                break;
            case 2:
                moveVar = -1;
                break;
        }
        int rotationVar = 0;
        switch (rotation)
        {
            case 0:
                rotationVar = 0;
                break;
            case 1:
                rotationVar = 1;
                break;
            case 2:
                rotationVar = -1;
                break;
        }
        this.transform.position += this.transform.forward * moveVar * speed * Time.deltaTime;
        this.transform.rotation *= Quaternion.Euler(0, rotationVar * rotationSpeed * Time.deltaTime, 0);
    }

    public void Jump()
    {
        if (!isOnGround) return;
        this.rb.AddForce(Vector3.up * jumpForce);
        isOnGround = false;
        
    }

    private void OnCollisionEnter(Collision other)
    {
       if(other.gameObject.CompareTag("Ground"))
       {
           isOnGround = true;
       }
    }
}
