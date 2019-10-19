using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringGun : MonoBehaviour
{
    [Header("Gun Point Gameobjects")]
    [SerializeField]
    private GameObject gunPoint = null;
    [SerializeField]
    private GameObject bullet = null;
    private GameObject instanceBullet = null;

    [SerializeField]
    private float bulletSpeed = 0;

    public void Shoot()
    {
        instanceBullet = Instantiate(bullet, gunPoint.transform.position, Quaternion.LookRotation(transform.forward));
        instanceBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
    }

}
