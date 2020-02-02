using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;
using UnityEngine;

public struct Position
{
    public int xPos;
    public int yPos;


    Position(int x, int y)
    {
        xPos = x;
        yPos = y;
    }
}

public struct House
{
    public Position botLeft;
    public int size;

    House(Position pos, int s)
    {
        botLeft = pos;
        size = s;
    }
}

enum TileUnitType
{
    Nothing = 0,
    Ally = 1,
    Enemy = 2,
    Wall = 3,
}

enum HousePartitions
{
    Hall = 0,
    Room = 1
}

public class GameStateManager : MonoBehaviour
{
    public bool isPlayerTurn { get; set; } = true;

    public GameObject gridObject;
    Grid grid;

    List<Position> frontier;
    int[,] gridley = new int[20, 20];

    System.Random rand;

    // Start is called before the first frame update
    void Start()
    {
        var gridObject = GameObject.Find("Grid");
        grid = gridObject.GetComponent<Grid>();
        frontier = new List<Position>();
        Array.Clear(gridley, 0, gridley.Length);

        GenerateNewSeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlayerTurn)
        {
            Debug.Log("Is it really you? My ASS");

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                UnitController controlMeDaddy = obj.GetComponent<UnitController>();
            }

            isPlayerTurn = true;
        }
    }
    public void PlaceGameObject(GameObject obj, Vector3Int? source, Vector3Int destination)
    {
        if (source.HasValue)
        {
            gridley[source.Value.x, source.Value.y] = 0;
        }

        int tag = 0;
        if (obj.gameObject.tag == "Ally")
        {
            tag = (int)TileUnitType.Ally;
        }
        else if (obj.gameObject.tag == "Enemy")
        {
            tag = (int)TileUnitType.Enemy;
        }
        else
        {
            tag = (int)TileUnitType.Wall;
        }


        gridley[destination.x, destination.y] = tag;
    }

    public int[,] GetDataGrid()
    {
        return gridley;
    }

    public bool isPassable(int x, int y)
    {
        return (gridley[x, y] > 0 ? false : true);
    }

    public void GenerateNewSeed(int seed = -1)
    {
        if (seed != -1)
        {
            rand = new System.Random();
        }
        else
        {
            rand = new System.Random(seed);
        }

        // We now have all of our houses, let's set the array

        List<List<int>> partitions = new List<List<int>>();

        for (int j = 0; j < 4; j++)
        {
            partitions.Add(new List<int>());
            for (int i = 0; i < 4; i++)
            {
                partitions[j].Add(rand.Next(0, 2));
            }
        }

        // Outer edge of our house
        for (int i = 0; i < 20; ++i)
        {
            gridley[0, i] = 3;
            gridley[i, 0] = 3;
            gridley[19, i] = 3;
            gridley[i, 19] = 3;
        }

        // I just ate a fucking jar of salsa in one sitting and my bowels have never felt more alive
        // If this is what death is then I will embrace it's spicy delicious goodness
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 4; j++)
            {
                // The four corners of our partition is always walls
                gridley[i * 5, j * 5] = 3;
                gridley[i * 5, j * 5 + 4] = 3;
                gridley[i * 5 + 4, j * 5] = 3;
                gridley[i * 5 + 4, j * 5 + 4] = 3;


                if (partitions[i][j] == (int)HousePartitions.Room)
                {
                    if (i+1 < 4 && partitions[i+1][j] == (int) HousePartitions.Room)
                    {
                        gridley[i * 5 + 4, j*5 + 1] = (int)TileUnitType.Wall;
                        gridley[i * 5 + 4, j*5 + 3] = (int)TileUnitType.Wall;
                    }
                    else if (j+1 < 4 && partitions[i][j+1] == (int)HousePartitions.Room)
                    {
                        gridley[i * 5 + 3, j*5 + 4] = (int)TileUnitType.Wall;
                        gridley[i * 5 + 1, j*5 + 4] = (int)TileUnitType.Wall;
                    }
                }
            }
        }


        // I'm feeling house'd, daddy
    }

    int GridDistance(Vector3Int p1, Vector3Int p2)
    {
        return Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
    }
}
