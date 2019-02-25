using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class HungryAgent : Agent
{
    public GameObject area;
    HungryArena myArea;

    public float moveSpeed = 2;
    public float dashSpeed = 4;
    public float dashingTime = 0.5f;

    private RayPerception rayPer;
    private HungryAcademy myAcademy;
    Rigidbody agentRb;

    private bool dashing = false;
    private float dashStartTime = 0;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        agentRb = GetComponent<Rigidbody>();
        Monitor.verticalOffset = 1f;
        myArea = area.GetComponent<HungryArena>();
        rayPer = GetComponent<RayPerception>();
        myAcademy = FindObjectOfType<HungryAcademy>();
    }

    public override void CollectObservations()
    {
        float rayDistance = 50f;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        string[] detectableObjects = { "banana", "agent", "wall", "ground" };
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        Vector3 localVelocity = transform.InverseTransformDirection(agentRb.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
        AddVectorObs(System.Convert.ToInt32(dashing));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        MoveAgent(vectorAction);
    }


    
    private void MoveAgent(float[] vectorAction)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        int dirToGoForwardAction = (int)vectorAction[0];
        int rotateDirAction = (int)vectorAction[1];
        int dashAction = (int)vectorAction[2];

        //moving
        if (dirToGoForwardAction == 1)
            dirToGo = transform.forward * 1f;
        else if (dirToGoForwardAction == 2)
            dirToGo = transform.forward * -1f;

        if (rotateDirAction == 1)
            rotateDir = transform.up * -1f;
        else if (rotateDirAction == 2)
            rotateDir = transform.up * 1f;

        //dashing
        if (dashAction == 1 && dashing == false)
        {
            Dash();
        }

        if (dashingTime > 0)
        {
            dirToGo = Vector3.zero;
        }

        dashingTime -= Time.fixedDeltaTime;
        if (dashingTime <= 0 && dashing)
        {
            FinishDashing();
        }

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);
        agentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
    }

    private void Dash()
    {
        dashing = true;
        dashingTime = 1f;
        agentRb.AddForce(transform.forward * dashSpeed, ForceMode.VelocityChange);
    }

    private void FinishDashing()
    {
        dashing = false;
    }

    public override void AgentReset()
    {
        dashing = false;
        agentRb.velocity = Vector3.zero;
        transform.position = new Vector3(Random.Range(-myArea.range, myArea.range),
                                 2f, Random.Range(-myArea.range, myArea.range))
                             + area.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
    }

    public override void AgentOnDone()
    {

    }
}
