using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep_Behavior_Script : MonoBehaviour
{
    PixelSheep_Script actionScript;

    //Grid
    public Grid_Holder_Script grid;

    //Tile and Sheep 2D arrays
    public GameObject[,] tileGrid;
    public GameObject[,] sheepGrid;
    public GameObject[,] wolfGrid;

    public string choice = " " ;
    public string active = " ";

    //public List<float> behavior = new List<float>();

    //public float

    void Start()
    {
        actionScript = GetComponent<PixelSheep_Script>();

        grid = GameObject.FindWithTag("GameController").GetComponent<Grid_Holder_Script>();

        tileGrid = grid.tileGrid;
        sheepGrid = grid.sheepGrid;
        wolfGrid = grid.wolfGrid;

        InvokeRepeating("Decide", 0.5f, 0.5f);
    }


    //Here is the main loop
    void Decide()
    {
        float danger = Danger_sense();
        float love = Love_mood();
        float hunger = 1f / (1f + 0.1f * ((float)actionScript.health -1));

        //print(danger.ToString() + " " + love.ToString() + " " + hunger.ToString());
         
        if (love > hunger && love > danger) { choice = "love"; }
        if (hunger > love && hunger > danger) { choice = "hungry"; }
        if (danger > love && hunger < danger) { choice = "danger"; }
    }

    float Danger_sense()
    {
        float bestdist = 10000f;

        foreach (GameObject wolf in wolfGrid)
        {
            if (wolf != null)
            {
                if (wolf != gameObject) //refer to the game object
                {
                    var wolfScript = wolf.GetComponent<Wolf_Script>();

                    //calculate dist
                    float dist = Mathf.Sqrt(
                        Mathf.Pow(wolfScript.xPoS - actionScript.xPoS, 2f) +
                        Mathf.Pow(wolfScript.yPoS - actionScript.yPoS, 2f));

                    //if its better set it to best
                    if (dist < bestdist)
                    {
                        bestdist = dist;
                    }
                }
            }
        }

        return 1f / (1f + 0.4f * (bestdist - 1.5f));
    }


    float Love_mood()
    {
        float bestdist = 10000f;

        if (actionScript.health < 30) return 0;

        foreach (GameObject sheep in sheepGrid)
        {
            if (sheep != null)
            {
                if (sheep != gameObject) //refer to the game object
                {
                    var sheepScript = sheep.GetComponent<PixelSheep_Script>();

                    //calculate dist
                    float dist = Mathf.Sqrt(
                        Mathf.Pow(sheepScript.xPoS - actionScript.xPoS, 2f) +
                        Mathf.Pow(sheepScript.yPoS - actionScript.yPoS, 2f));

                    //if its better set it to best
                    if (dist < bestdist)
                    {
                        bestdist = dist;
                    }
                }
            }
        }

        return 1f / bestdist;
    }


    void Health_Action()
    {

    }

    void Love_Action()
    {
        GameObject tile = actionScript.SenseSheep();

        actionScript.Dijkstra(tile);

        InvokeRepeating("Love_Loop", 0.5f, 5f);
    }

    void Love_Loop()
    {
        //move
        int last = actionScript.path.Count - 1;
        GameObject spot = actionScript.path[last];
        actionScript.path.Remove(spot);
        actionScript.Move(spot);

        //try to make a baby
        actionScript.Babies();
    }

    private void Update()
    {
        if (choice != active)
        {
            actionScript.path.Clear();
            active = choice;

            print(active);

            if (active == "love")
            {
                Love_Action();
            }
            if (active == "danger")
            {

            }
            if (active == "hungry")
            {

            }
        }
    }
}
