using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isPlayerTurn { get; set; } = true;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       if (!isPlayerTurn)
       {
            //DELEGATION
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                UnityEngine.Debug.Log("grASS");
                enemy.GetComponent("EnemyUnit");
            }
        }
    }
}
