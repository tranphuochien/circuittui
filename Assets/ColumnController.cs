using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnController : MonoBehaviour {

    public bool trigger = false;
    public float height = 0.5f;
	// Update is called once per frame
	void Update () {
       
        if (trigger)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.16f,height,1), 5 * Time.deltaTime);
        }
    }
}
