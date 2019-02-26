using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class HungryArena : Area
{
    public GameObject banana;
    public int numberBananas;
    public bool respawnBananas;
    public float range;
    public RedBlockLogic redBlock;

    void CreateBanana(int numBana, GameObject bananaType)
    {
        for (int i = 0; i < numBana; i++)
        {
            GameObject bana = Instantiate(bananaType, new Vector3(Random.Range(-range, range), 1f,
                                                          Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            bana.GetComponent<RespawnBananaLogic>().respawn = respawnBananas;
            bana.GetComponent<RespawnBananaLogic>().myArea = this;
        }
    }

    public override void ResetArea()
    {
        base.ResetArea();
        
    }

    public void ResetBananaArea(GameObject[] agents)
    {
        foreach (GameObject agent in agents)
        {
            if (agent.transform.parent == gameObject.transform)
            {
                agent.transform.position = new Vector3(Random.Range(-range, range), 2f,
                                               Random.Range(-range, range))
                                           + transform.position;
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            }
        }

        CreateBanana(numberBananas, banana);

//        if (redBlock != null)
//        {
//            redBlock.ResetArena();
//        }
    }
}
