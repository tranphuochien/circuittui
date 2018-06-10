using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadHeatMap : MonoBehaviour
{

    Text HeatMapBtn;
    private bool shouldShowHeatmap = false;
    private readonly static string POPULATION = "Textures/heatmap/population/";
    private readonly static string POLUTION = "Textures/heatmap/polution/";
    // Use this for initialization
    void Start()
    {
        HeatMapBtn = GameObject.Find("HeatmapBtn").GetComponentInChildren<Text>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldShowHeatmap)
        {
            Texture2D texture = Resources.Load("Textures/clouds_map_out") as Texture2D; //No need to specify extension.

            this.GetComponent<Renderer>().material.mainTexture = texture;
        }
    }

    public void HeatMapBtnTrigger()
    {
        shouldShowHeatmap = !shouldShowHeatmap;

        if (shouldShowHeatmap)
        {
            gameObject.SetActive(true);
            HeatMapBtn.text = "Hide Heatmap";
        }
        else
        {
            gameObject.SetActive(false);
            HeatMapBtn.text = "Show Heatmap";
        }

    }
}
