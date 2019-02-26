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
    Rigidbody agentRB;

    private bool dashing = false;
    private float dashStartTime = 0;

    public float jumpingTime;
    public float jumpTime;
    // This is a downward force applied when falling to make jumps look
    // less floaty
    public float fallingForce;
    // Use to check the coliding objects
    public Collider[] hitGroundColliders = new Collider[3];
    Vector3 jumpTargetPos;
    Vector3 jumpStartingPos;

    string[] detectableObjects;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        agentRB = GetComponent<Rigidbody>();
        Monitor.verticalOffset = 1f;
        myArea = area.GetComponent<HungryArena>();
        rayPer = GetComponent<RayPerception>();
        myAcademy = FindObjectOfType<HungryAcademy>();
    }

    public override void CollectObservations()
    {
        float rayDistance = 50f;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        detectableObjects = new string[] { "banana", "agent", "wall", "ground", "redBlock" };
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        Vector3 localVelocity = transform.InverseTransformDirection(agentRB.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
        AddVectorObs(DoGroundCheck(true) ? 1 : 0);
        AddVectorObs(System.Convert.ToInt32(JumpOverRazer()));

        Vector3 agentPos = transform.position - myArea.ground.transform.position;
        agentPos = agentPos / myArea.range;
        AddVectorObs(agentPos);
//        AddVectorObs(System.Convert.ToInt32(dashing));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        MoveAgent(vectorAction);

        if ((!Physics.Raycast(agentRB.position, Vector3.down, 20))) //falling from ground
        {
            Done();
            SetReward(-1f);
        }
    }



    public void MoveAgent(float[] act)
    {

        AddReward(-0.003f);

        bool smallGrounded = DoGroundCheck(true);
        //        bool largeGrounded = DoGroundCheck(false);

        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;
        int dirToGoForwardAction = (int)act[0];
        int rotateDirAction = (int)act[1];
        //        int dirToGoSideAction = (int)act[2];
        int jumpAction = (int)act[2];

        if (dirToGoForwardAction == 1)
            dirToGo = transform.forward * 1f * (smallGrounded ? 1f : 0.5f);
        else if (dirToGoForwardAction == 2)
            dirToGo = transform.forward * -1f * (smallGrounded ? 1f : 0.5f);

        if (rotateDirAction == 1)
            rotateDir = transform.up * -1f;
        else if (rotateDirAction == 2)
            rotateDir = transform.up * 1f;

        //        if (dirToGoSideAction == 1)
        //            dirToGo = transform.right * -0.6f * (smallGrounded ? 1f : 0.5f);
        //        else if (dirToGoSideAction == 2)
        //            dirToGo = transform.right * 0.6f * (smallGrounded ? 1f : 0.5f);


        if (jumpAction == 1)
            if ((jumpingTime <= 0f) && smallGrounded)
            {
                Jump();
            }

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);
        agentRB.AddForce(dirToGo * myAcademy.agentRunSpeed,
                         ForceMode.VelocityChange);
        agentRB.maxDepenetrationVelocity = 2;

        if (jumpingTime > 0f)//start jump
        {
            jumpTargetPos =
            new Vector3(agentRB.position.x,
                        jumpStartingPos.y + myAcademy.agentJumpHeight,
                        agentRB.position.z) + dirToGo * myAcademy.agentRunSpeed;
            MoveTowards(jumpTargetPos, agentRB, myAcademy.agentJumpVelocity,
                        myAcademy.agentJumpVelocityMaxChange);

        }
        else//start falling
        {
            if (smallGrounded == false)
            {
                agentRB.AddForce(
                    Vector3.down * fallingForce, ForceMode.Acceleration);
            }
        }


        jumpingTime -= Time.fixedDeltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("banana"))
        {
            collision.gameObject.GetComponent<RespawnBananaLogic>().OnEaten();
            AddReward(1f);
        }

//        if (collision.gameObject.CompareTag("redBlock"))
//        {
//            SetReward(-1f);
//            Done();
//        }
    }

    private void OnTriggerEnter(Collider other)
    {
//        if (other.gameObject.CompareTag("banana"))
//        {
//            other.gameObject.GetComponent<RespawnBananaLogic>().OnEaten();
//            AddReward(0.5f);
//        }

        if (other.gameObject.CompareTag("redBlock"))
        {
            SetReward(-1f);
            Done();
        }
    }

    // Begin the jump sequence
    public void Jump()
    {

        jumpingTime = 0.2f;
        jumpStartingPos = agentRB.position;
    }

    /// <summary>
    /// Does the ground check.
    /// </summary>
    /// <returns><c>true</c>, if the agent is on the ground, 
    /// <c>false</c> otherwise.</returns>
    /// <param name="boxWidth">The width of the box used to perform 
    /// the ground check. </param>
    public bool DoGroundCheck(bool smallCheck)
    {
        if (!smallCheck)
        {
            hitGroundColliders = new Collider[3];
            Physics.OverlapBoxNonAlloc(
                gameObject.transform.position + new Vector3(0, -0.05f, 0),
                new Vector3(0.95f / 2f, 0.5f, 0.95f / 2f),
                hitGroundColliders,
                gameObject.transform.rotation);
            bool grounded = false;
            foreach (Collider col in hitGroundColliders)
            {

                if (col != null && col.transform != this.transform &&
                    (col.CompareTag("ground") ||
                     col.CompareTag("block") ||
                     col.CompareTag("wall")))
                {
                    grounded = true; //then we're grounded
                    break;
                }
            }
            return grounded;
        }
        else
        {

            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, -0.05f, 0), -Vector3.up, out hit,
                1f);

            if (hit.collider != null &&
                (hit.collider.CompareTag("ground") ||
                 hit.collider.CompareTag("block") ||
                 hit.collider.CompareTag("wall"))
                && hit.normal.y > 0.95f)
            {
                return true;
            }

            return false;
        }
    }

    public bool JumpOverRazer()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + new Vector3(0, -1f, 0), -Vector3.up, out hit,
            10f);
        if (hit.collider != null && hit.collider.CompareTag("redBlock"))
        {
            if (transform.position.y > hit.transform.position.y)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Moves  a rigidbody towards a position smoothly.
    /// </summary>
    /// <param name="targetPos">Target position.</param>
    /// <param name="rb">The rigidbody to be moved.</param>
    /// <param name="targetVel">The velocity to target during the
    ///  motion.</param>
    /// <param name="maxVel">The maximum velocity posible.</param>
    void MoveTowards(
        Vector3 targetPos, Rigidbody rb, float targetVel, float maxVel)
    {
        Vector3 moveToPos = targetPos - rb.worldCenterOfMass;
        Vector3 velocityTarget = moveToPos * targetVel * Time.fixedDeltaTime;
        if (float.IsNaN(velocityTarget.x) == false)
        {
            rb.velocity = Vector3.MoveTowards(
                rb.velocity, velocityTarget, maxVel);
        }
    }

    public override void AgentReset()
    {
        dashing = false;
        agentRB.velocity = Vector3.zero;
        transform.position = new Vector3(Random.Range(-myArea.range, myArea.range),
                                 2f, Random.Range(-myArea.range, myArea.range))
                             + area.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
    }

    public override void AgentOnDone()
    {

    }
}
