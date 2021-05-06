using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//ALWAYS COL BEFORE ROW, X BEFORE Y
public class Grass_Fab_Script : MonoBehaviour
{
    //clock
    [SerializeField]
    public float grassTimer = 10f; //start
    public float timerReset = 5f; //rteset

    //Coordinates of the tile
    public float xPos = 0f;
    public float yPos = 0f;

    //My Grass Sprites
    public Sprite grass1;
    public Sprite grass2;
    public Sprite grass3;
    public Sprite grass4;

    //for water
    public Sprite watersprite;
    public bool water = false;

    //GrassLevel
    public int grassLevel = 0;
    private int oldGrass = 0; //here to test if the grass level has changed

    //Grid
    public Grid_Holder_Script grid;

    //Tile and Sheep 2D arrays
    public GameObject[,] tileGrid;
    public GameObject[,] sheepGrid;


    //vars for dij
    public List<GameObject> neighbors = new List<GameObject>();
    public bool visited = false;
    public int edgeweight = 0;
    public int dist = 0;
    public GameObject previousTile = null;

    //for coloration
    private Renderer rend;
    public bool colored = false;

    [SerializeField]
    private Color senseColor = Color.white;
    [SerializeField]
    private Color resetColor = Color.white;
    [SerializeField]
    private Color pathColor = Color.white;
    [SerializeField]
    private Color openColor = Color.white;
    [SerializeField]
    private Color closedColor = Color.white;

    //for A*
    public float aG = 0;
    public float aH = 0;
    public float aF = 0;

    // Start is to make the grass/water
    void Start()
    {
        //get the grid and arrays
        grid = GameObject.FindWithTag("GameController").GetComponent<Grid_Holder_Script>();

        tileGrid = grid.tileGrid;
        sheepGrid = grid.sheepGrid;

        if (water)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = watersprite;
            edgeweight = 25;
        }
        else //not water
        {
            int random = Random.Range(1, 5); 
            grassLevel = random;  //all my homies hate random
            edgeweight = grassLevel;
        }
    }


    //ned update to manage sprites
    void Update()
    {
        if (!water) //again dont fuck with the water
        {
            if (oldGrass != grassLevel)
            {
                if (grassLevel == 1) { this.gameObject.GetComponent<SpriteRenderer>().sprite = grass1; }
                else if (grassLevel == 2) { this.gameObject.GetComponent<SpriteRenderer>().sprite = grass2; }
                else if (grassLevel == 3) { this.gameObject.GetComponent<SpriteRenderer>().sprite = grass3; }
                else if (grassLevel == 4) { this.gameObject.GetComponent<SpriteRenderer>().sprite = grass4; }

                edgeweight = grassLevel;
                oldGrass = grassLevel; //reset oldgrass
            }
        }

        //reset timer so that it turns off when the sheep gets there
        if (colored)
        {
            grassTimer -= Time.deltaTime;

            if(grassTimer <= 0)
            {
                //ResetColor();
                grassTimer = timerReset;
            }
        }
    }

    public void loop() //called from update in grid holder (pretty much update)
    {
        Grow(); //do this in the loop and i can update it  later
    }

    void Grow() //to
    {
        if (!water) //make shure to not fuck with water
        {
            if (grassLevel >= 4)
            {
                Die();
            }
            else
            {
                grassLevel += 1;
            }
        }
    }

    public void Die() //for eat and withering
    {
        if (!water) //make shure to not fuck with water
        {
            grassLevel -= 2;
            if (grassLevel <= 0)
            {
                grassLevel = 1;
            }
        }
    }

    public List<GameObject> GetNeighbors()
    {
        List<GameObject> bros = new List<GameObject>();

        for (int x = (int)xPos - 1; x <= (int)xPos + 1; x++)
        {
            for (int y = (int)yPos - 1; y <= (int)yPos + 1; y++)
            {
                if (x >= 0 && x < grid.cols)
                {
                    if (y >= 0 && y < grid.rows)
                    {
                        if (!(x == xPos && y == yPos))
                        {
                            bros.Add(tileGrid[x, y]);
                        }
                    }
                }
            }
        }

        return bros;
    }


    //___________________________________________CHANGE_COLOR_________________________________________________



    //it WORKS???
    public void SenseColor()
    {
        rend = GetComponent<Renderer>();

        //when does it end?
        rend.material.color = senseColor;

        colored = true;
    }

    public void PathColor()
    {
        rend = GetComponent<Renderer>();

        //when does it end?
        rend.material.color = pathColor;

        colored = true;
    }

    public void OpenColor()
    {
        rend = GetComponent<Renderer>();

        //when does it end?
        rend.material.color = openColor;

        colored = true;
    }

    public void ClosedColor()
    {
        rend = GetComponent<Renderer>();

        //when does it end?
        rend.material.color = closedColor;

        colored = true;
    }

    public void ResetColor()
    {
        //print("do we make it");
        rend = GetComponent<Renderer>();

        //when does it end?
        rend.material.color = resetColor;    
    }
}
