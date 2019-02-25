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
