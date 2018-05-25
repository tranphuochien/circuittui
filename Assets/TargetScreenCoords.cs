using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TargetScreenCoords : MonoBehaviour {

    private ImageTargetBehaviour mImageTargetBehaviour = null;

    // Use this for initialization
    void Start()
    {
        // We retrieve the ImageTargetBehaviour component
        // Note: This only works if this script is attached to an ImageTarget
        mImageTargetBehaviour = GetComponent<ImageTargetBehaviour>();

        if (mImageTargetBehaviour == null)
        {
            Debug.Log("ImageTargetBehaviour not found ");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mImageTargetBehaviour == null)
        {
            Debug.Log("ImageTargetBehaviour not found");
            return;
        }

        Vector2 targetSize = mImageTargetBehaviour.GetSize();
        float targetAspect = targetSize.x / targetSize.y;

        // We define a point in the target local reference 
        // we take the bottom-left corner of the target, 
        // just as an example
        // Note: the target reference plane in Unity is X-Z, 
        // while Y is the normal direction to the target plane
        Vector3 pointOnTarget = new Vector3(-0.5f, 0, -0.5f / targetAspect);

        // We convert the local point to world coordinates
        Vector3 targetPointInWorldRef = transform.TransformPoint(pointOnTarget);

        // We project the world coordinates to screen coords (pixels)
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetPointInWorldRef);

        Debug.Log("target point in screen coords: " + screenPoint.x + ", " + screenPoint.y);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPoint);
        GameObject plane = Instantiate(Resources.Load("VuforiaTrackingPlane"), worldPos, Quaternion.identity) as GameObject;
        plane.transform.name = "ABCD";
        plane.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        plane.transform.localScale = new Vector3(0.05f, 1, 0.05f);
        plane.GetComponent<MeshRenderer>().material = Resources.Load("white 2", typeof(Material)) as Material;

    }
}
