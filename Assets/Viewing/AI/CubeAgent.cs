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

    [SerializeField] private GameObject target;
    [Range(1,30)] public float speed = 1f;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        targetPos = target.transform.position;
    }

    public override void OnEpisodeBegin()
    {
        // 에이전트의 위치 초기화
        transform.position = startPos;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f,360f), 0f);

        target.transform.position = new Vector3(UnityEngine.Random.Range(targetPos.x-4, targetPos.x+4), 1f, UnityEngine.Random.Range(targetPos.z + 10, targetPos.z -10));
        // target.transform.position = new Vector3(UnityEngine.Random.Range(targetPos.x-13, targetPos.x+13), 1f, UnityEngine.Random.Range(targetPos.z + 13, targetPos.z + 13));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 에이전트의 위치와 속도, target 위치 등을 센서로 관찰
        sensor.AddObservation(transform.InverseTransformDirection(rb.velocity));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        
        // 에이전트의 이동 및 회전
        float moveX = actions.ContinuousActions[0];
        float rotation = actions.ContinuousActions[1];

        transform.position += transform.forward * moveX * speed * Time.deltaTime;
        transform.Rotate(rotation * Vector3.up * 10f); 

        AddReward(-1/MaxStep);
        
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.1f);
        }
        if (other.gameObject.CompareTag("Target"))
        {
            Debug.Log("Reached target");
            AddReward(5f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 사람 제어용 입력
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }
}