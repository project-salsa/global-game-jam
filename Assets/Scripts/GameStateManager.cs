using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;
using UnityEngine;


public struct TileData
{
    public int unitType;
    public string unitName;

    public TileData(int type, string name)
    {
        unitType = type;
        unitName = name;
    }
}
public struct Position
{
    public int xPos;
    public int yPos;


    public Position(int x, int y)
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

    List<Position> frontier;
    TileData[,] gridley = new TileData[20, 20];

    System.Random rand;

    // Start is called before the first frame update
    void Start()
    {
        frontier = new List<Position>();
        GenerateNewSeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlayerTurn)
        {
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
            gridley[source.Value.x, source.Value.y] = new TileData(0, "");
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

        gridley[destination.x, destination.y] = new TileData(tag, obj.name);
    }

    public TileData[,] GetDataGrid()
    {
        return gridley;
    }

    public bool isPassable(int x, int y)
    {
        return (gridley[x, y].unitType > 0 ? false : true);
    }

    public void GenerateNewSeed(int seed = -1)
    {
        if (seed != -1)
        {
            rand = new System.Random((int)DateTime.UtcNow.Ticks);
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
            gridley[0, i] = new TileData(3, "");
            gridley[i, 0] = new TileData(3, "");
            gridley[19, i] = new TileData(3, "");
            gridley[i, 19] = new TileData(3, "");
        }
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 4; j++)
            {
                // The four corners of our partition is always walls
                gridley[i * 5, j * 5] = new TileData(3, "");
                gridley[i * 5, j * 5 + 4] = new TileData(3, "");
                gridley[i * 5 + 4, j * 5] = new TileData(3, "");
                gridley[i * 5 + 4, j * 5 + 4] = new TileData(3, "");


                if (partitions[i][j] == (int)HousePartitions.Room)
                {
                    if (i + 1 < 4 && partitions[i + 1][j] == (int)HousePartitions.Room)
                    {
                        gridley[i * 5 + 4, j * 5 + 1] = new TileData((int)TileUnitType.Wall, "");
                        gridley[i * 5 + 4, j * 5 + 3] = new TileData((int)TileUnitType.Wall, "");
                    }
                    else if (j + 1 < 4 && partitions[i][j + 1] == (int)HousePartitions.Room)
                    {
                        gridley[i * 5 + 3, j * 5 + 4] = new TileData((int)TileUnitType.Wall, "");
                        gridley[i * 5 + 1, j * 5 + 4] = new TileData((int)TileUnitType.Wall, "");
                    }
                }
            }
        }
    }

    int GridDistance(Vector3Int p1, Vector3Int p2)
    {
        return Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
    }
}
