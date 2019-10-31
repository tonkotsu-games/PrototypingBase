using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArena : MonoBehaviour
{
    [SerializeField]
    private List<Wave.Waves> myWaves;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float maxY;

    private float minY = -0.15f;

    private void Update()
    {
        if (WaveManager.instance != null)
        {
            if (myWaves.Contains(WaveManager.instance.currentWaveState))
            {
               
               if(transform.localPosition.y < maxY)
                {
                    transform.localPosition += (Vector3.up * speed*Time.deltaTime);
                }
                else
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, maxY, transform.localPosition.z);
                }
            }
            else
            {
                if(transform.localPosition.y > minY)
                {                   
                    transform.localPosition += (Vector3.down * speed*Time.deltaTime);
                }
                else
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, minY, transform.localPosition.z);
                }
            }
        }
    }
}
