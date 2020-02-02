using UnityEngine;
using System;

public class UnitController : MonoBehaviour
{
    public bool selected = false;
    public int movementRange { get; set; } = 2;
    public int maxHP { get; set; } = 10;
    public int curHP { get; set; } = 10;
    // either purify value for allies, damage for enemies
    public int baseSkillValue { get; set; } = 2;
    GameStateManager stateManager;
    Grid grid;
    // Use this for initialization
    void Start()
    {
        var gridObject = GameObject.Find("Grid");
        var gameStateObject = GameObject.Find("GameStateManager");
        stateManager = gameStateObject.GetComponent<GameStateManager>();
        grid = gridObject.GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsBeingClickedOn())
            {
                if (selected)
                {
                    DeselectUnit();
                } else
                {
                    SelectUnit();
                }
            }
            else if (selected && gameObject.tag == "Ally")
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                TryClick(grid.LocalToCell(mouseRay.origin));
            }
        }
    }

    bool IsBeingClickedOn ()
    {
        Vector3 currentUnitPos = transform.position;
        var currentUnitGridPos = grid.LocalToCell(currentUnitPos);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var origin = new Vector2(ray.origin.x, ray.origin.y);

        if (Physics.Raycast(ray) || Physics2D.Raycast(origin, new Vector2(0, 0)))
        {
            var rayOriginPos = grid.LocalToCell(ray.origin);
            if (currentUnitGridPos == rayOriginPos)
            {
                return true;
            }
        }
        return false;
    }

    void SelectUnit ()
    {
        selected = true;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.yellow;
    }

    void DeselectUnit()
    {
        var mainColor = Color.green;
        if (gameObject.tag == "Enemy")
        {
            mainColor = Color.red;
        }
        selected = false;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = mainColor;
    }

    

    int GridDistance(Vector3Int p1, Vector3Int p2)
    {
        return Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
    }

    void TryClick(Vector3Int destination)
    {
        var currentUnitTilePos = grid.LocalToCell(transform.position);
        // TODO: add a getObjectAtDestination func
        int[,] dataGrid = stateManager.GetDataGrid();
        var objectAtDestinaton = dataGrid[destination.x, destination.y];
        int distance = GridDistance(destination, currentUnitTilePos);
        switch (objectAtDestinaton)
        {
            case 0:
                
                if (distance <= movementRange)
                {
                    stateManager.PlaceGameObject(gameObject, currentUnitTilePos, destination);
                    Move(destination.x, destination.y);
                    DeselectUnit();
                }
                break;
            case 1:
                // handle ally case
                break;
            case 2:
                // attack enemy
                if (distance == 1)
                {
                    // get enemy at tile

                }
                break;
            case 3:
                // wall - invalid move
                return;
        }
        

    }

    void Move(int x, int y)
    {
        var newPos = new Vector3Int(x, y, 0);
        var newPosCenter = grid.GetCellCenterLocal(newPos);
        transform.position = newPosCenter;
    }
}
