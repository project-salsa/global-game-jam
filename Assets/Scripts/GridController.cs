using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum UnitType
{
    Ally,
    Enemy
}

public class GridController : MonoBehaviour
{
    GameStateManager stateManager;
    Grid grid;
    public GameObject enemy;
    public GameObject ally;

    int allyIndex = 1;
    int enemyIndex = 1;

    public GameObject[] enemies;
    public GameObject[] allies;


    void Debug_AddTestUnits(int numEnemies = 1)
    {
        for (int i = 0; i < numEnemies; i++)
        {
            var newPos = GetRandomVector();
            AddUnit(UnitType.Enemy, newPos);
        }
        AddUnit(UnitType.Ally, new Vector3Int(1, 1, 0));
    }

    // Start is called before the first frame update
    void Start()
    {
        var stateManagerObject = GameObject.Find("GameStateManager");
        stateManager = stateManagerObject.GetComponent<GameStateManager>();
        grid = GetComponentInParent<Grid>();
        Debug_AddTestUnits();
    }

    // Update is called once per frame
    void Update()
    {
    }

    Vector3Int GetRandomVector()
    {
        float maxX = 10;
        float maxY = 10;
        return new Vector3Int((int)UnityEngine.Random.Range(1f, maxX + 1), (int)UnityEngine.Random.Range(1f, maxY + 1), 0);
    }

    void Move(GameObject obj, int x, int y)
    {
        var newPos = new Vector3Int(x, y, 0);
        var newPosCenter = grid.GetCellCenterLocal(newPos);
        //var newPosLocal = grid.CellToWorld(newPosCenter);
        obj.transform.position = newPosCenter;
    }

    GameObject AddUnit(UnitType type, Vector3Int destination)
    {
        GameObject newObject;
        if (type == UnitType.Ally)
        {
            newObject = Instantiate(ally);
            newObject.name = "Ally" + allyIndex.ToString();
            allyIndex++;
        }
        else if (type == UnitType.Enemy)
        {
            newObject = Instantiate(enemy);
            newObject.name = "Enemy" + enemyIndex.ToString();
            enemyIndex++;
        }
        else
        {
            throw new Exception("tried to make an invalid unit");
        }
        Move(newObject, destination.x, destination.y);

        stateManager.PlaceGameObject(newObject, null, destination);
        return newObject;

    }
}
