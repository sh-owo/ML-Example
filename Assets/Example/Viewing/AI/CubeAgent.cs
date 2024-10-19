using System;
using System.Collections;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class CubeAgent : Agent
{
    private Rigidbody rb;
    private Vector3 startPos;
    private Vector3 targetPos;
    public GameObject[] posList = new GameObject[9];
    [SerializeField] private GameObject target;
    [Range(1,30)] public float speed = 20f;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        targetPos = target.transform.position;
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        int random = UnityEngine.Random.Range(0, posList.Length);
        startPos = posList[UnityEngine.Random.Range(0, posList.Length)].transform.position;
        transform.position =  new Vector3(UnityEngine.Random.Range(startPos.x - 16, startPos.x + 16), 1f, UnityEngine.Random.Range(startPos.z - 16, startPos.z + 16));
        transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f,360f), 0f);

        targetPos = posList[UnityEngine.Random.Range(0, posList.Length)].transform.position;

        target.transform.position = new Vector3(UnityEngine.Random.Range(targetPos.x-16, targetPos.x+16), 1f, UnityEngine.Random.Range(targetPos.z + 16, targetPos.z + 16));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.InverseTransformDirection(rb.velocity));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float rotation = actions.ContinuousActions[1];
        
        Vector3 move = transform.forward * moveX * speed * Time.deltaTime;
        rb.AddForce(move * 2f, ForceMode.VelocityChange);
        transform.Rotate(rotation * Vector3.up * 220f * Time.deltaTime); 

        AddReward(-1/MaxStep);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            // EndEpisode();
        }
        if (other.gameObject.CompareTag("Target"))
        {
            Debug.Log("Reached target");
            AddReward(4f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }
}