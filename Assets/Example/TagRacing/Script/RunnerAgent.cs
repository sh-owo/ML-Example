using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class RunnerAgent : Agent
{
    Player player;
    private Rigidbody rb;

    private int steps = 0;
    
    public override void Initialize()
    {
        player = GetComponent<Player>();
        rb = player.rb;

        player.startPos = transform.position;
        player.startRot = transform.rotation;
    }
    
    public override void OnEpisodeBegin()
    {
        transform.position = player.startPos;
        transform.rotation = player.startRot;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.InverseTransformDirection(rb.velocity));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveX = actions.DiscreteActions[0];
        int rotation = actions.DiscreteActions[1];
        int jump = actions.DiscreteActions[2];
        
        Movement(moveX, rotation, jump);
        AddReward(-1/MaxStep);
        steps++;
        if (steps > MaxStep)
        {
            Debug.Log("Runner win");
            AddReward(5f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            AddReward(-5f);
            EndEpisode();
        }
    }
    
    private void Movement(int moveX, int rotation, int jump)
    {
        player.Move(moveX, rotation);
        if(jump == 1) player.Jump();
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        
        discreteActions[0] = (int)Input.GetAxis("Vertical");
        discreteActions[1] = (int)Input.GetAxis("Horizontal");
        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
