using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public static SkyboxChanger instance;
    [SerializeField]
    private List<Texture> textures;
    MeshRenderer myRend;

    private void Awake()
    {
       if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        myRend = gameObject.GetComponent<MeshRenderer>();
    }

    public void ChangeTexture(int index)
    {
        Debug.Log("Changing Texture");
        myRend.material.SetTexture("_BaseColorMap", textures[index]);
    }

    
}
