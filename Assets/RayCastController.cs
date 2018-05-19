using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastController : MonoBehaviour {

    public int x = 12;
    public int y = 7;
    public float z = -1f;
    public Vector3 directionZ = new Vector3(0, 0, -1);
  
    public static bool beginRayCasting = false;

    // Use this for initialization
    void Start () {
	}
  
    // Update is called once per frame
    void Update () {
        if (!beginRayCasting)
        {
            return;
        }
        ResetColor();
        RaycastHit hit;
        
        Vector3 test = new Vector3(GenerateChildObject.curPos.x + 13.57f, GenerateChildObject.curPos.z + 9.94f, z);
        if (Physics.Raycast(test, directionZ, out hit, 100))
        {
            if (hit.collider.gameObject.tag == "cube")
            {
                hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
            }
        }


    }

    void ResetColor ()
    {
        int children = GenerateChildObject.listCubes.transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            GameObject child = GenerateChildObject.listCubes.transform.GetChild(i).gameObject;
            child.GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
