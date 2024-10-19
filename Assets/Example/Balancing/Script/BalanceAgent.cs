using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using Random = UnityEngine.Random;

public class BalanceAgent : Agent
{
    public GameObject sub1;
    public GameObject sub2;
    
    private Rigidbody rb;
    private Vector3 ballPos;
    
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        ballPos = transform.position;
    }
    
    public override void OnEpisodeBegin()
    {
        // Reset the agent and object positions
        rb.MovePosition(ballPos);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)); // Ensure these values are floating point

        // Randomly position sub1 and sub2
        sub1.transform.position = new Vector3(ballPos.x + Random.Range(-0.1f, 0.1f), 3f, ballPos.z + Random.Range(-0.1f, 0.1f)); 
        sub2.transform.position = new Vector3(sub1.transform.position.x + Random.Range(-0.1f, 0.1f), sub1.transform.position.y + 2f, sub1.transform.position.z + Random.Range(-0.1f, 0.1f)); 
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect the agent's position and relative positions of sub1 and sub2
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.InverseTransformPoint(sub1.transform.position));
        sensor.AddObservation(transform.InverseTransformPoint(sub2.transform.position));
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get the movement input from the actions
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
   
        Vector3 movement = new Vector3(moveX, 0f, moveZ) * Time.deltaTime * 1f;
        rb.MovePosition(rb.position + movement);
        
        // Call the reward function
        agentReward();
    }

    private void agentReward()
    {
        // Reward based on how high sub2 is
        float positionY = sub2.transform.position.y; 
        float normalizedDistance = positionY / 5f;

        // Log debug information
        Debug.Log($"Distance : {positionY}, Normalized Distance : {normalizedDistance}");

        // End episode if agent moves too far from the initial position
        if(Mathf.Abs(transform.position.x) > Mathf.Abs(ballPos.x) + 100 || Mathf.Abs(transform.position.z) > Mathf.Abs(ballPos.z) + 100)
        {
            EndEpisode();
        }

        AddReward(normalizedDistance * 0.1f); // Reward based on the normalized distance
        
        /*// Penalize if sub2 falls too low, but don't end the episode
        if(sub2.transform.position.y < 0.5f)
        {
            AddReward(-0.3f); // Penalize heavily for failure but allow continuation
        }
        else
        {
            AddReward(normalizedDistance * 0.1f); // Reward based on the normalized distance
        }*/
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Map human input to the actions
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");

        Debug.Log("Horizontal (X axis) : " + continuousActionsOut[0]);
        Debug.Log("Vertical (Z axis) : " + continuousActionsOut[1]);
    }
}
