using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagController : MonoBehaviour
{

    GameObject flagObject;
    GameObject column;
    Text StatistictBtn;
    bool shouldShowColumn;

    List<Vector3> listFlags = new List<Vector3>();
    List<GameObject> listColumns = new List<GameObject>();

    public void AddFlag(Vector3 position)
    {
        listFlags.Add(new Vector3(position.x, position.z, 0));
    }
    // Use this for initialization
    void Start()
    {
        StatistictBtn = GameObject.Find("Analyst").GetComponentInChildren<Text>();

        flagObject = GameObject.Find("flagobject");
        column = GameObject.Find("column");

        flagObject.SetActive(false);
        column.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < listFlags.Count; i++)
        {
            //Already set flag
            if (listFlags[i].z == 1)
            {
                continue;
            }
            listFlags[i] = new Vector3(listFlags[i].x, listFlags[i].y, 1);
            GameObject newFlag = cloneFlag();

            newFlag.transform.localPosition = new Vector3(listFlags[i].x, 0, listFlags[i].y);

            GameObject newColumn = cloneColumn();
            newColumn.transform.localPosition = new Vector3(listFlags[i].x, 0, listFlags[i].y);
            listColumns.Add(newColumn);
        }
    }

    public void renderColumn()
    {
        for (int i = 0; i < listColumns.Count; i++)
        {
            listColumns[i].GetComponent<ColumnController>().height = Random.Range(0.05f, 0.5f);
            listColumns[i].GetComponent<ColumnController>().trigger = true;
        }
    }

    public void hideColumn()
    {
        for (int i = 0; i < listColumns.Count; i++)
        {
            listColumns[i].GetComponent<ColumnController>().height = 0;
        }
    }

    public void StatisticBtnTrigger()
    {
        shouldShowColumn = !shouldShowColumn;

        if (shouldShowColumn)
        {
            renderColumn();
            StatistictBtn.text = "Hide statistic";
        }
        else
        {
            hideColumn();
            StatistictBtn.text = "Show statistic";
        }

    }

    private GameObject cloneFlag()
    {
        GameObject newFlag = Object.Instantiate(flagObject);
        newFlag.transform.SetParent(transform);
        newFlag.SetActive(true);
        newFlag.transform.localRotation = Quaternion.Euler(90, -1.5f, -1.5f);
        return newFlag;
    }

    private GameObject cloneColumn()
    {
        GameObject newColumn = Object.Instantiate(column);
        newColumn.transform.SetParent(transform);
        newColumn.SetActive(true);
        newColumn.transform.localRotation = Quaternion.Euler(90, 0, 0);
        newColumn.transform.localScale = new Vector3(0.16f, 0, 0.1f);

        return newColumn;
    }
}
