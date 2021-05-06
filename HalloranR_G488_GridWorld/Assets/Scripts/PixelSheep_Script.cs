using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//ALWAYS COL BEFORE ROW, X BEFORE Y
public class PixelSheep_Script : MonoBehaviour
{
    //Variables
    [SerializeField]
    public int health;
    [SerializeField]
    public int senseRange = 3;

    //change from aStar to Dijstra
    private bool aStarBool = false;

    //position of the sheep so Caps S
    public float xPoS;
    public float yPoS;

    //Grid
    public Grid_Holder_Script grid;

    //Tile and Sheep 2D arrays
    public GameObject[,] tileGrid;
    public GameObject[,] sheepGrid;
    public GameObject[,] wolfGrid;

    //Dijstra List
    public List<GameObject> path = new List<GameObject>();


    //A* lists
    List<GameObject> open = new List<GameObject>();
    List<GameObject> closed = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        //get the grid and arrays
        grid = GameObject.FindWithTag("GameController").GetComponent<Grid_Holder_Script>();

        tileGrid = grid.tileGrid;
        sheepGrid = grid.sheepGrid;
        wolfGrid = grid.wolfGrid;

        GameObject theGrid = GameObject.Find("Grid_Holder");
    }

    //loop has been de activated
    public void loop()
    {
        if (path.Count > 0)
        {
            Debug.Log("Why we movin' so?");

            int last = path.Count - 1;
            GameObject spot = path[last];
            path.Remove(spot);
            Move(spot);
        }
        else
        {
            Eat();

            print(aStarBool);

            GameObject dest = Sense();

            if (aStarBool)
            {
                AStar(dest);
            }
            else
            {
                Dijkstra(dest);
            }
        }
    }


    //________________________________ACTIONS__________________________________________________________________


    public void Eat()
    {
        GameObject here = tileGrid[(int)xPoS, (int)yPoS];

        here.GetComponent<Grass_Fab_Script>().Die();

        health += 10;
    }


    //gets the best grass in a radius
    public GameObject Sense()
    {
        //changed to be only dist 1 for testing
        List<GameObject> bros = new List<GameObject>();

        for (int x = (int)xPoS - senseRange; x <= (int)xPoS + senseRange; x++)
        {
            for (int y = (int)yPoS - senseRange; y <= (int)yPoS + senseRange; y++)
            {
                if (x >= 0 && x < grid.cols)
                {
                    if (y >= 0 && y < grid.rows)
                    {
                        if (!(x == xPoS && y == yPoS))
                        {
                            //print("addin");
                            bros.Add(tileGrid[x, y]);
                        }
                    }
                }
            }
        }

        //getting null only
        int yield = 0;
        List<GameObject> same = new List<GameObject>(); //list  to hold all the same vars

        foreach (GameObject space in bros)
        {
            int grass = space.GetComponent<Grass_Fab_Script>().grassLevel;
            if(yield == grass)
            {
                yield = grass;
                same.Add(space);
            }
            if (yield < grass)
            {
                yield = grass;
                same = new List<GameObject>();
                same.Add(space);
            }
        }

        //get a random element of the list so no wierd converegence
        GameObject best = same[Random.Range(0, same.Count - 1)]; 
        //return the tile and mabye user some color
        best.GetComponent<Grass_Fab_Script>().SenseColor();

        //print("Sense");
        print(best.GetComponent<Grass_Fab_Script>().xPos + "," + best.GetComponent<Grass_Fab_Script>().yPos);
        return best;
    }


    //gets the closest sheep's tile
    public GameObject SenseSheep()
    {
        GameObject desire = null;

        float bestdist = 10000f;

        foreach (GameObject sheep in sheepGrid)
        {
            if (sheep != null)
            {
                if (sheep != gameObject) //refer to the game object
                {
                    var sheepScript = sheep.GetComponent<PixelSheep_Script>();

                    //calculate dist
                    float dist = Mathf.Sqrt(
                        Mathf.Pow(sheepScript.xPoS - xPoS, 2f) +
                        Mathf.Pow(sheepScript.yPoS - yPoS, 2f));

                    //if its better set it to best
                    if (dist < bestdist)
                    {
                        bestdist = dist;
                        desire = sheep;
                    }
                }
            }
        }

        var xcord = desire.GetComponent<PixelSheep_Script>().xPoS;
        var ycord = desire.GetComponent<PixelSheep_Script>().yPoS;

        foreach(GameObject tile in tileGrid)
        {
            Grass_Fab_Script ts = tile.GetComponent<Grass_Fab_Script>();
            if (ts.xPos == xcord && ts.yPos == ycord) return tile;
        }

        return null;
    }


    public void Dijkstra(GameObject dest)
    {
        //WORIED THAT ITS BREAKING DUE TO AN INFINITE WHILE LOOP (BUT WHY NO PRINTS)
        List<GameObject> daList = new List<GameObject>(); 

        Grass_Fab_Script goal = dest.GetComponent<Grass_Fab_Script>();

        //step 1
        foreach (GameObject tile in tileGrid)
        {
            Grass_Fab_Script script = tile.GetComponent<Grass_Fab_Script>();

            //step 2
            if (script.xPos == xPoS && script.yPos == yPoS)
            {
                print("Start The D Man");
                script.visited = false;
                script.dist = 0;
                script.previousTile = null;
                daList.Add(tile);

            }
            else
            {
                script.visited = false;
                script.dist = 2147483647;
                script.previousTile = null;
                daList.Add(tile);
            }
        }

        while (daList.Count > 0)
        //for (int i = 0; i < 4; i++)
        {
            //get the shortest distance and asign them to tc and dist 
            GameObject tC = null; //tile current
            int dist = 2147483647;

            foreach (GameObject space in daList)
            {
                int len = space.GetComponent<Grass_Fab_Script>().dist;

                if (tC == null)
                {
                    tC = space;
                    dist = len;
                }
                else if (len < dist)
                {
                    tC = space;
                    dist = len;
                }
            }
            daList.Remove(tC);


            if (dist >= 2147483646) { break; }; //cant go any furhter reduced by one to see if anything hits

            Grass_Fab_Script cur = tC.GetComponent<Grass_Fab_Script>(); //current tile script
            cur.visited = true; //current is visited

            //print(cur.xPos);
            //print(cur.yPos);
            if ((cur.xPos == goal.xPos) && (cur.yPos == goal.yPos)) {
                //print("WEEEEEEEEE");
                break; }; //using the location not the actual game object

            List<GameObject> nL = cur.GetNeighbors();

            foreach (GameObject step in nL)
            {
                //get the next node to the neighbor
                var next = step.GetComponent<Grass_Fab_Script>();

                //if it isnt visited
                if (!next.visited)
                {
                    //make the new distance
                    //print(cur.dist);
                    //print(cur.edgeweight);
                    int newDist = cur.dist + cur.edgeweight;
                    //print(newDist);

                    //if its beter
                    if (newDist < next.dist)
                    {
                        //set dist and previous
                        next.dist = newDist;
                        next.previousTile = tC;
                        //print("Setting new dist");
                    }
                }
            }
            //Debug.Log("Debug message here");
            //if i dont have this break the program shuts down
            //break;
        }

        //print("here is the start");
        GameObject main = dest;
        GameObject prev = goal.previousTile;
        while (main != null)
        {
            //print("Looping through path");
            if (main.GetComponent<Grass_Fab_Script>().xPos == xPoS)
            {
                if (main.GetComponent<Grass_Fab_Script>().yPos == yPoS)
                {
                    //print("end of path");
                    break;
                }
            }
            path.Add(main);

            //print(main.GetComponent<Grass_Fab_Script>().xPos + "," + main.GetComponent<Grass_Fab_Script>().yPos);
            main = prev;
            prev = main.GetComponent<Grass_Fab_Script>().previousTile;
        }
    }


    public void AStar(GameObject dest)
    {
        //initialize
        open.Clear();
        closed.Clear();

        //add script
        Grass_Fab_Script goal = dest.GetComponent<Grass_Fab_Script>();

        //clear each tile and get them ready
        foreach (GameObject tile in tileGrid)
        {
            Grass_Fab_Script script = tile.GetComponent<Grass_Fab_Script>();

            //step 2
            if (script.xPos == xPoS && script.yPos == yPoS)
            {
                print("Start The Star");
                script.previousTile = null;

                //set the start to the open list
                script.aG = 0;
                script.aH = 0;
                open.Add(tile);
                script.OpenColor();

            }
            else
            {
                script.previousTile = null;
            }
        }

        print("before while loop");

        //Da Loop
        while (open.Count > 0)
        {
            GameObject curNode = null;

            //get best path in open list
            foreach (GameObject shortest in open)
            {
                if (curNode == null)
                {
                    curNode = shortest;
                }
                else if (shortest.GetComponent<Grass_Fab_Script>().aF < curNode.GetComponent<Grass_Fab_Script>().aF)
                {
                    curNode = shortest;
                }
            }

            //print(curNode.GetComponent<Grass_Fab_Script>().aF);

            //remove from open (works?) and reset color
            open.Remove(curNode);
            curNode.GetComponent<Grass_Fab_Script>().ResetColor();

            //used to break out of the while loop
            bool done = false;

            foreach (GameObject neighbor in curNode.GetComponent<Grass_Fab_Script>().GetNeighbors())
            {
                //make a script
                Grass_Fab_Script script = neighbor.GetComponent<Grass_Fab_Script>();

                //set parent
                script.previousTile = curNode;

                //check if it is dest
                if (script.xPos == goal.xPos && script.yPos == goal.yPos)
                {
                    print("we Made it");
                    //end search
                    done = true;
                    break;
                }

                //g equals previous cost plus edgeweight
                script.aG = curNode.GetComponent<Grass_Fab_Script>().aG + curNode.GetComponent<Grass_Fab_Script>().edgeweight;

                //set heurisitc (manhattan)
                script.aH = Mathf.Abs(goal.xPos - script.xPos) + Mathf.Abs(goal.yPos - script.yPos);

                //set the af
                script.aF = script.aH + script.aG;

                //print("loopin");
                //print(script.aF);

                bool openBool = true;
                bool closedBool = true;

                //see if the path is worse in any of either lists
                foreach (GameObject openTile in open)
                {
                    if (openTile.GetComponent<Grass_Fab_Script>().aF < script.aF)
                    {
                        openBool = false;
                    }
                }
                foreach (GameObject closedTile in closed)
                {
                    if (closedTile.GetComponent<Grass_Fab_Script>().aF < script.aF)
                    {
                        closedBool = false;
                    }
                }

                //if the neighbor is shortest path, add to open list
                if (openBool && closedBool)
                {
                    //print("we add to lists");
                    open.Add(neighbor);
                    neighbor.GetComponent<Grass_Fab_Script>().OpenColor();


                }
            }//end of neighbors foreach

            //break out of while loop
            if (done)
            {
                break;
            }

            //add curnode to closed list
            closed.Add(curNode);
            curNode.GetComponent<Grass_Fab_Script>().ClosedColor();
        }

        //reset color
        foreach (GameObject tile in closed) {tile.GetComponent<Grass_Fab_Script>().ResetColor();}
        foreach (GameObject tile in open) { tile.GetComponent<Grass_Fab_Script>().ResetColor();}

        //add to path (stolen from dijksta)
        GameObject main = dest;
        GameObject prev = goal.previousTile;

        while (main != null)
        {
            //print("Looping through path");
            if (main.GetComponent<Grass_Fab_Script>().xPos == xPoS && main.GetComponent<Grass_Fab_Script>().yPos == yPoS)
            {
                //print("end of path");
                break;
            }
            path.Add(main);
            main.GetComponent<Grass_Fab_Script>().PathColor();

            //print(main.GetComponent<Grass_Fab_Script>().xPos + "," + main.GetComponent<Grass_Fab_Script>().yPos);
            main = prev;
            if(main != null)
            {
                prev = main.GetComponent<Grass_Fab_Script>().previousTile;
            }
        }
    }


    //take a tile and if its next to the sheep in either 8 directions
    public void Move(GameObject ajTile)
    { 
        float tileX = ajTile.GetComponent<Grass_Fab_Script>().xPos;
        float tileY = ajTile.GetComponent<Grass_Fab_Script>().yPos;

        print(tileX + "," + tileY);

        //make sure the tile is next to us
        if ((Mathf.Abs(xPoS - tileX) <= 1) && (Mathf.Abs(yPoS - tileY) <= 1))
        {
            if (!ajTile.GetComponent<Grass_Fab_Script>().water) //make sure it aint water
            {
                if (wolfGrid[(int)tileX, (int)tileY] == null)
                {
                    if (sheepGrid[(int)tileX, (int)tileY] == null) //make sure there aint a sheep there
                    {
                        //print("here");
                        sheepGrid[(int)tileX, (int)tileY] = sheepGrid[(int)xPoS, (int)yPoS];
                        sheepGrid[(int)xPoS, (int)yPoS] = null; //set the old space to null

                        xPoS = tileX;
                        yPoS = tileY;

                        //change visual position
                        this.transform.position = new Vector3(ajTile.transform.position.x, ajTile.transform.position.y, -2);

                        //change color
                        ajTile.GetComponent<Grass_Fab_Script>().ResetColor();
                    }
                    else
                    {
                        //Do this so that if something is there you wait and move later
                        print("WAit!!!");
                        path.Insert(0, ajTile);
                    }
                }
                else
                {
                    //next path is water so clear path
                    print("WOLF!!");
                    path.Clear();
                }
            }
            else
            {
                //next path is water so clear path
                print("WATER");
                path.Clear();
            }
        }
        else
        {
            //next path is not ajacent to the tile so clear path
            print("BAD PATH");
            path.Clear();
        }
    }


    public void Babies()
    {
        if(health >= 30)
        {
            print("here1");
            for (int x = (int)xPoS - 1; x <= (int)xPoS + 1; x++)
            {
                for (int y = (int)yPoS - 1; y <= (int)yPoS + 1; y++)
                {
                    if (!(x == (int)xPoS && y == (int)yPoS))
                    {
                        print(x.ToString() + " " + y.ToString());

                        if (sheepGrid[x, y] != null)
                        {
                            print("here3");
                            GameObject partner = sheepGrid[x, y];

                            PixelSheep_Script pscript = partner.GetComponent<PixelSheep_Script>();

                            if (pscript.health >= 30)
                            {
                                print("here4");
                                //take health away from both
                                pscript.health -= 15;

                                health -= 15;

                                grid.BabySheep((int)xPoS, (int)yPoS);
                            }
                        }
                    }
                }
            }
        }
    }
}
