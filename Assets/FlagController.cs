using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour {

    GameObject flagObject;

    List<Vector3> listFlags = new List<Vector3>();

    public void AddFlag(Vector3 position)
    {
        listFlags.Add(new Vector3(position.x, position.z, 0));
    }
	// Use this for initialization
	void Start () {
        flagObject = GameObject.Find("flagobject");
        flagObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
        for (int i = 0; i < listFlags.Count; i++) 
        {
            //Already set flag
            if (listFlags[i].z == 1 )
            {
                continue;
            }
            listFlags[i] = new Vector3(listFlags[i].x, listFlags[i].y, 1);
            GameObject newFlag = cloneFlag();
            newFlag.transform.SetParent(transform);
            newFlag.transform.localPosition = listFlags[i];
        }
	}

    private GameObject cloneFlag()
    {
        return Object.Instantiate(flagObject);
    }
}
