using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//ALWAYS COL BEFORE ROW, X BEFORE Y
public class Grid_Holder_Script : MonoBehaviour
{
    //my fields I have to edit
    [SerializeField]
    public int cols = 16;
    [SerializeField]
    public int rows = 16;
    [SerializeField]
    private float sheepNum = 1;
    [SerializeField]
    private float wolfNum = 1;
    [SerializeField]
    private float tileSize = 1;

    //clock
    [SerializeField]
    public float grassTimer = 5f; //start
    [SerializeField]
    public float grassReset = 5f; //reset
    [SerializeField]
    public float sheepTimer = 5f; //start
    [SerializeField]
    public float sheepReset = 5f; //reset

    //public arraylist with all the tiles at cords
    public GameObject[,] tileGrid;
    //public arraylist with all the sheeps at cords
    public GameObject[,] sheepGrid;
    //public arraylist with all the wolves at cords
    public GameObject[,] wolfGrid;


    // Start is called before the first frame update
    void Start() //summon the objects
    {
        tileGrid = new GameObject[cols, rows];

        sheepGrid = new GameObject[cols, rows];

        wolfGrid = new GameObject[cols, rows];

        GenerateGrid();
        //moved the summon sheep to end of grid to make it sequential
    }


    private void GenerateGrid()
    {
        //make a reference
        GameObject referenceTile = (GameObject)Instantiate(Resources.Load("Grass_Fab"));

        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                GameObject tile = (GameObject)Instantiate(referenceTile, transform);

                float posX = col * tileSize;
                float posY = row * -tileSize;
                
                tile.transform.position = new Vector2(posX, posY);

                tile.GetComponent<Grass_Fab_Script>().xPos = col;
                tile.GetComponent<Grass_Fab_Script>().yPos = row;

                int randomness = Random.Range(1, 100);
                if (randomness < 15) //water bitch
                {
                    tile.GetComponent<Grass_Fab_Script>().water = true;
                }

                tileGrid[col, row] = tile;
            }
        }

        //destroy reference
        Destroy(referenceTile);

        //resize grid
        float gridW = cols * tileSize;
        float gridH = rows * tileSize;

        transform.position = new Vector2(-gridW / 2 + tileSize / 2, gridH / 2 - tileSize / 2);

        //why does summon sheep still spawn on water
        SummonSheep(); //do it here to make sure they are sequential
    }


    void SummonSheep()
    {
        float temp = sheepNum; //temp

        GameObject referenceSheep = (GameObject)Instantiate(Resources.Load("PixelSheep_Fab"));

        while(temp > 0)
        {
            int rCol = Random.Range(0, cols); //fuck random
            int rRow = Random.Range(0, rows); //fuck random

            if (!tileGrid[rCol, rRow].GetComponent<Grass_Fab_Script>().water) //dont place on water
            {
                if (sheepGrid[rCol, rRow] == null) //this works even if i dont create null sheep
                {
                    GameObject sheep = (GameObject)Instantiate(referenceSheep, transform);


                    float posX = rCol - (cols / 2) + .5f;
                    float posY = -rRow + (rows / 2) - .5f;

                    sheep.transform.position = new Vector3(posX, posY, -2);

                    sheep.GetComponent<PixelSheep_Script>().xPoS = rCol;
                    sheep.GetComponent<PixelSheep_Script>().yPoS = rRow;

                    sheepGrid[rCol, rRow] = sheep; //add them to the 2darray

                    temp -= 1; //tick down the temp var
                }
            }
        }

        Destroy(referenceSheep);

        //make the wolves
        SummonWolves();
    }


    //make wolves
    void SummonWolves()
    {
        float temp = wolfNum; //temp

        GameObject referenceWolf = (GameObject)Instantiate(Resources.Load("Wolf_Fab"));

        while (temp > 0)
        {
            int rCol = Random.Range(0, cols); //fuck random
            int rRow = Random.Range(0, rows); //fuck random

            if (!tileGrid[rCol, rRow].GetComponent<Grass_Fab_Script>().water) //dont place on water
            {
                if (sheepGrid[rCol, rRow] == null) //this works even if i dont create null sheep
                {
                    if (wolfGrid[rCol, rRow] == null)
                    {
                        GameObject wolf = (GameObject)Instantiate(referenceWolf, transform);


                        float posX = rCol - (cols / 2) + .5f;
                        float posY = -rRow + (rows / 2) - .4f;

                        wolf.transform.position = new Vector3(posX, posY, -2);

                        wolf.GetComponent<Wolf_Script>().xPoS = rCol;
                        wolf.GetComponent<Wolf_Script>().yPoS = rRow;

                        wolfGrid[rCol, rRow] = wolf; //add them to the 2darray

                        temp -= 1; //tick down the temp var
                    }
                }
            }
        }

        Destroy(referenceWolf);
    }


    public void BabySheep(int xs, int ys)
    {
        GameObject referenceSheep = (GameObject)Instantiate(Resources.Load("PixelSheep_Fab"));

        bool done = false;

        //loop through the neighbor tiles of the og sheep
        for (int x = xs - 1; x <= xs + 1; x++)
        {
            for (int y = ys - 1; y <= ys + 1; y++)
            {
                //copied and converted from summonsheep
                if (!tileGrid[x, y].GetComponent<Grass_Fab_Script>().water) //dont place on water
                {
                    if (sheepGrid[x, y] == null) //this works even if i dont create null sheep
                    {
                        GameObject sheep = (GameObject)Instantiate(referenceSheep, transform);


                        float posX = x - (cols / 2) + .5f;
                        float posY = -y + (rows / 2) - .5f;

                        sheep.transform.position = new Vector3(posX, posY, -2);

                        sheep.GetComponent<PixelSheep_Script>().xPoS = x;
                        sheep.GetComponent<PixelSheep_Script>().yPoS = y;

                        sheepGrid[x, y] = sheep; //add them to the 2darray

                        //break after making one child
                        done = true;
                        break;
                    }
                }
            }
            if(done == true)
            {
                //break again to get out of both
                break;
            }
        }
    }





    //Use this Update to control all sheep and grass
    //void Update()
    //{
    //    //print("YEP");
    //    grassTimer -= Time.deltaTime;
    //    sheepTimer -= Time.deltaTime;

    //    if (grassTimer <= 0)
    //    {
    //        foreach (GameObject tile in tileGrid)
    //        {
    //            //tile.GetComponent<Grass_Fab_Script>().loop();
    //        }

    //        grassTimer = grassReset; //reset the timer
    //    }

    //    if (sheepTimer <= 0)
    //    {
    //        sheepTimer = sheepReset; //reset the timer before starting the loops

    //        foreach (GameObject sheep in sheepGrid)
    //        {
    //            if (sheep != null)
    //            {
    //                print("Start");
    //                sheep.GetComponent<PixelSheep_Script>().loop();
    //            }
    //        }
    //    }
    //}
}
