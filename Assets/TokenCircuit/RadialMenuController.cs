using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class RadialMenuController : MonoBehaviour
{
    private String currentItemChosen = "center_menu";
    private RectTransform rectTransformMainMenu;
    List<Button> childButtons = new List<Button>();
    Vector3[] buttonGoalPos;
    bool open = false;
    public int buttonDistance = 150;
    public float speed = 2f;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
        GameObject.Find("Menu").transform.localScale = new Vector3(0, 0.01f, 0);

        childButtons = this.GetComponentsInChildren<Button>(true).Where(x => x.gameObject.transform.parent != transform.parent).ToList();
        rectTransformMainMenu = this.GetComponent<RectTransform>();

        this.GetComponent<Button>().onClick.AddListener(() => { OpenMenu(); });

        rectTransformMainMenu.pivot = new Vector2(0.5f, 0.5f);
        buttonGoalPos = new Vector3[childButtons.Count];

        foreach (Button b in childButtons)
        {
            b.gameObject.SetActive(false);
        }

        ZoomOutWhenAppear();

    }

    public void ZoomOutWhenAppear()
    {
        gameObject.SetActive(true);
        StartCoroutine(ZoomOutMenu());
    }

    public void OpenMenu()
    {
        Destroy(GameObject.Find(currentItemChosen));
        open = !open;
        float angle = 180 / (childButtons.Count - 1) * Mathf.Deg2Rad;
        for (int i = 0; i < childButtons.Count; i++)
        {
            if (open)
            {
                float xpos = Mathf.Cos(angle * i) * buttonDistance;
                float ypos = Mathf.Sin(angle * i) * buttonDistance;
                buttonGoalPos[i] = new Vector3(
                    rectTransformMainMenu.localPosition.x + xpos,
                    rectTransformMainMenu.localPosition.y + ypos,
                    0);
            }
            else
            {
                buttonGoalPos[i] = rectTransformMainMenu.localPosition;
            }
        }

        StartCoroutine(MoveButtons());
    }


    public void ItemClick(GameObject gameObject)
    {
        OpenMenu();

        if (gameObject.transform.gameObject.name.Equals("cancle"))
        {
            return;
        }
        GameObject clone = Instantiate(gameObject);
        RectTransform rectTransform = clone.GetComponent<RectTransform>();
        clone.transform.gameObject.name = currentItemChosen;
        clone.transform.parent = this.GetComponent<Button>().transform;
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.sizeDelta = new Vector2(35, 35);
        rectTransform.localScale = new Vector3(1, 1, 1);
    }

    private IEnumerator ZoomOutMenu()
    {
        GameObject cylinder = GameObject.Find("Menu");
        int loops = 0;
        while (loops <= 5)
        {
            yield return new WaitForSeconds(0.001f);


            cylinder.transform.localScale = new Vector3(loops, 0.01f, loops);
            loops++;
        }
    }

    public GameObject GetCurrentItem()
    {
        return GameObject.Find(currentItemChosen);
    }

    private IEnumerator MoveButtons()
    {
        foreach (Button b in childButtons)
        {
            b.gameObject.SetActive(true);
        }
        int loops = 0;
        while (loops <= buttonDistance / speed)
        {
            yield return new WaitForSeconds(0.01f);
            for (int i = 0; i < childButtons.Count; i++)
            {
                Color c = childButtons[i].GetComponent<Image>().color;
                if (open)
                {
                    c.a = Mathf.Lerp(c.a, 1, speed * Time.deltaTime);
                }
                else
                {
                    c.a = Mathf.Lerp(c.a, 0, speed * Time.deltaTime);
                }
                childButtons[i].GetComponent<Image>().color = c;
                childButtons[i].GetComponent<RectTransform>().localPosition = Vector2.Lerp(
                    childButtons[i].GetComponent<RectTransform>().localPosition,
                    buttonGoalPos[i], speed * Time.deltaTime);
            }
            loops++;
        }
        if (!open)
        {
            foreach (Button b in childButtons)
            {
                b.gameObject.SetActive(false);
                b.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
