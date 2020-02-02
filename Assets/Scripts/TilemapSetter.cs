using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSetter : MonoBehaviour
{
    public GameObject manager;
    GameStateManager managerInterface;
    public Tilemap tilemap;
    public TileBase brick;
    public TileBase grASS;

    bool yes = true;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameStateManager");
        managerInterface = manager.GetComponent<GameStateManager>();
        tilemap = this.GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (yes)
        {
            UpdateTilemap();
            yes = false;
        }
    }

    void UpdateTilemap()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                //TileBase tile();

                Vector3Int vec = new Vector3Int(i, j, 0);

                if (managerInterface.GetDataGrid()[i, j].unitType == 3)
                {
                    tilemap.SetTile(vec, brick);
                }
                else
                {
                    tilemap.SetTile(vec, grASS);
                }
            }
        }
    }
}
