using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;
using UnityEngine.UI;

public class HungryAcademy : Academy
{
    [HideInInspector]
    public GameObject[] agents;
    [HideInInspector]
    public HungryArena[] listArea;

    public int totalScore;
    public Text scoreText;

    [Header("Specific to WallJump")]
    public float agentRunSpeed;
    public float agentJumpHeight;
    //when a goal is scored the ground will use this material for a few seconds.
    public Material goalScoredMaterial;
    //when fail, the ground will use this material for a few seconds. 
    public Material failMaterial;

    [HideInInspector]
    //use ~3 to make things less floaty
    public float gravityMultiplier = 2.5f;
    [HideInInspector]
    public float agentJumpVelocity = 777;
    [HideInInspector]
    public float agentJumpVelocityMaxChange = 10;

    // Use this for initialization
    public override void InitializeAcademy()
    {
        Physics.gravity *= gravityMultiplier;
    }

    public override void AcademyReset()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("banana"));

        agents = GameObject.FindGameObjectsWithTag("agent");
        listArea = FindObjectsOfType<HungryArena>();
        foreach (HungryArena ba in listArea)
        {
            ba.ResetBananaArea(agents);
        }

        totalScore = 0;
    }

    void ClearObjects(GameObject[] objects)
    {
        foreach (GameObject bana in objects)
        {
            Destroy(bana);
        }
    }

    public override void AcademyStep()
    {
//        scoreText.text = string.Format(@"Score: {0}", totalScore);
    }
}
