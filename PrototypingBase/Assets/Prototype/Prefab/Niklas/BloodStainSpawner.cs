using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodStainSpawner : MonoBehaviour
{

    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    public GameObject[] bloodStainPrefabs;
    Vector3 pos;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("COllision!");
        if (other.gameObject.GetComponent<MockupEnemyController>() == null)
        {
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);


            int i = 0;

            while (i < numCollisionEvents)
            {

                pos = collisionEvents[i].intersection;

                SpawnBloodStain();


                i++;
            }
        }
    }

    private void SpawnBloodStain()
    {
        int randomNumber = Random.Range(0, bloodStainPrefabs.Length);
        GameObject particlesInstance = Instantiate(bloodStainPrefabs[randomNumber], pos, Quaternion.identity) as GameObject;
        
        ParticleSystem parts = particlesInstance.GetComponentInChildren<ParticleSystem>();
        float totalDuration = parts.main.duration;
        Destroy(particlesInstance, totalDuration);
    }
}
