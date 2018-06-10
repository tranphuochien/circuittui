using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadHeatMap : MonoBehaviour {
    private readonly static string POPULATION = "Textures/heatmap/population/";
    private readonly static string POLUTION = "Textures/heatmap/polution/";
    // Use this for initialization
    void Start () {
       
        
    }
	
	// Update is called once per frame
	void Update () {
        Texture2D texture = Resources.Load("Textures/clouds_map_out") as Texture2D; //No need to specify extension.

        this.GetComponent<Renderer>().material.mainTexture = texture;
    }
}
