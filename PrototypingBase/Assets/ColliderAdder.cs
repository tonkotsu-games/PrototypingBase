using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderAdder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        WaveManager.OnFirstModStage += AddCollider;
    }

    void AddCollider()
    {
        gameObject.AddComponent<MeshCollider>();
    }
}
