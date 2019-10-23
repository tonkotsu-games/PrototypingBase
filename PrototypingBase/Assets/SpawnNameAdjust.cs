using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class SpawnNameAdjust : MonoBehaviour
{
    private void Update()
    {
        gameObject.name = gameObject.GetComponent<EnemySpawner>().Wave.ToString() + "-EnemySpawner";
        gameObject.GetComponentInChildren<TextMeshPro>().text = gameObject.GetComponent<EnemySpawner>().Wave.ToString() + " - " + gameObject.GetComponentInChildren<EnemySpawner>().amountOfEnemiesToSpawn;
    }
}
