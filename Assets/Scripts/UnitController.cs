using UnityEngine;
using System;

public class UnitController : MonoBehaviour
{
    public bool selected = false;
    public int movementRange = 2;
    public int maxHP { get; set; } = 10;
    public int curHP { get; set; } = 10;
    // either purify value for allies, damage for enemies
    public int baseSkillValue { get; set; } = 2;

    // sprites (probs a bit hacky but)
    public Sprite zomb1;
    public Sprite zomb2;
    public Sprite zomb3;

    GameStateManager stateManager;
    Grid grid;
    GridController gridController;
    // Use this for initialization
    void Start()
    {
        var gridObject = GameObject.Find("Grid");
        var gameStateObject = GameObject.Find("GameStateManager");
        stateManager = gameStateObject.GetComponent<GameStateManager>();
        grid = gridObject.GetComponent<Grid>();
        gridController = gridObject.GetComponent<GridController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAppearance();

        if (curHP <= 0)
        {
            gridController.RemoveUnit(gameObject);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (IsBeingClickedOn())
            {
                if (selected)
                {
                    DeselectUnit();
                }
                else
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

    bool IsBeingClickedOn()
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

    void SelectUnit()
    {
        selected = true;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.yellow;
    }

    void DeselectUnit()
    {
        var mainColor = Color.white;
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
        TileData[,] dataGrid = stateManager.GetDataGrid();
        var objectAtDestinaton = dataGrid[destination.x, destination.y];
        int distance = GridDistance(destination, currentUnitTilePos);
        switch (objectAtDestinaton.unitType)
        {
            case 0:
                if (distance <= movementRange)
                {
                    stateManager.PlaceGameObject(gameObject, currentUnitTilePos, destination);
                    Move(destination.x, destination.y);
                    DeselectUnit();
                    stateManager.isPlayerTurn = false;
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
                    Attack(GameObject.Find(objectAtDestinaton.unitName));
                    DeselectUnit();
                    stateManager.isPlayerTurn = false;
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

    void Attack(GameObject target)
    {
        var unitController = target.GetComponent<UnitController>();
        unitController.curHP -= baseSkillValue;
        if (unitController.curHP < 0)
        {
            unitController.curHP = 0;
        }
    }

    public void UpdateAppearance()
    {
        if (tag == "Enemy")
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            if ((float)curHP/(float)maxHP <= .4)
            {
                renderer.sprite = zomb3;
            }
            else if ((float)curHP / (float)maxHP <= .8)
            {
                renderer.sprite = zomb2;
            }
            else
            {
                renderer.sprite = zomb1;
            }
        }
    }

    public void MoveEnemy()
    {
        var myPos = grid.LocalToCell(this.gameObject.transform.position);

        var allies = GameObject.FindGameObjectsWithTag("Ally");
        Vector3Int closest_ally = new Vector3Int(99, 99, 0);

        // Find the closest ally
        foreach (var ally in allies)
        {
            var theirPos = grid.LocalToCell(ally.transform.position);

            int tempX = theirPos.x - myPos.x;
            int tempY = theirPos.y - myPos.y;

            if (Math.Abs(tempX) +Math.Abs(tempY) < Math.Abs(closest_ally.x)+Math.Abs(closest_ally.y))
            {
                closest_ally = theirPos;
            }
        }


        int x = closest_ally.x - myPos.x;
        int y = closest_ally.y - myPos.y;

        Vector3Int destination;

        if (Math.Abs(x) > Math.Abs(y))
        {
            // We moving left/right
            if (x < 0)
            {
                destination = new Vector3Int(myPos.x-1, myPos.y, 0);
            }
            else
            {
                destination = new Vector3Int(myPos.x + 1, myPos.y, 0);
            }
        }
        else
        {
            // We moving up/down
            if (y < 0)
            {
                destination = new Vector3Int(myPos.x, myPos.y-1, 0);
            }
            else
            {
                destination = new Vector3Int(myPos.x, myPos.y+1, 0);
            }
        }

        TileData[,] dataGrid = stateManager.GetDataGrid();
        var objectAtDestinaton = dataGrid[destination.x, destination.y];
        if (objectAtDestinaton.unitType == 0)
        {
            stateManager.PlaceGameObject(this.gameObject, myPos, destination);
            Move(destination.x, destination.y);
        }
        else if (objectAtDestinaton.unitType == 1)
        {
            // TODO: ZOMBONI ATTACKONI
        }
    }
}
