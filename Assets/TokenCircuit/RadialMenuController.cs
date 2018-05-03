using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class RadialMenuController : MonoBehaviour, IPointerClickHandler
{
    const string item_dien_tro = "dien_tro";
    const string item_diode = "diode";
    const string item_tu_dien = "tu_dien";
    const string item_cancle = "cancle";
    private String currentItemChosen = "center_menu";
    private RectTransform rectTransformMainMenu;
    private GameObject rootMenu;
    private String nameCurItem;
    private BoardManager boardManager;

    List<Button> childButtons = new List<Button>();
    Vector3[] buttonGoalPos;
    bool open = false;
    public int buttonDistance = 150;
    public float speed = 2f;

    // Use this for initialization
    void Start()
    {
        boardManager = BoardManager.GetInstance();
        rootMenu = GameObject.Find(BoardManager.MENU_NAME);
        rootMenu.SetActive(false);
        rootMenu.transform.localScale = new Vector3(0, 0.01f, 0);

        childButtons = this.GetComponentsInChildren<Button>(true).Where(x => x.gameObject.transform.parent != transform.parent).ToList();
        rectTransformMainMenu = this.GetComponent<RectTransform>();

        this.GetComponent<Button>().onClick.AddListener(() => { OpenMenu(); });

        rectTransformMainMenu.pivot = new Vector2(0.5f, 0.5f);
        buttonGoalPos = new Vector3[childButtons.Count];

        foreach (Button b in childButtons)
        {
            b.gameObject.SetActive(false);
        }

        //ZoomOutWhenAppear();
    }

    public void ZoomOutWhenAppear(Vector3 position)
    {
        rootMenu.SetActive(true);
        rootMenu.transform.localPosition = position;
        StartCoroutine(ZoomOutMenu());
    }

    public void ZoomInWhenDisappear()
    {
        StartCoroutine(ZoomInMenu());
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

    private void ExportItemByName(String name)
    {
        if (name == null || name == item_cancle)
        {
            return;
        }

        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector2 positionItem = GameObject.Find(BoardManager.MENU_NAME).transform.position;
        item.transform.position = positionItem;
        item.gameObject.name = "Cube_" + DateTime.Now.ToString();
        switch (name)
        {
            case item_dien_tro:
                item.GetComponent<MeshRenderer>().material = Resources.Load(item_dien_tro, typeof(Material)) as Material;
                break;
            case item_tu_dien:
                item.GetComponent<MeshRenderer>().material = Resources.Load(item_tu_dien, typeof(Material)) as Material;
                break;
            case item_diode:
                item.GetComponent<MeshRenderer>().material = Resources.Load(item_diode, typeof(Material)) as Material;
                break;
        }
        boardManager.AddObjectToCircuit(item);
    }

    public void ItemClick(GameObject gameObject)
    {
        OpenMenu();
        nameCurItem = gameObject.transform.gameObject.name;

        if (nameCurItem.Equals(item_cancle))
        {
            ZoomInWhenDisappear();
            return;
        }

        //Create item in center menu
        GameObject clone = Instantiate(gameObject);
        RectTransform rectTransform = clone.GetComponent<RectTransform>();
        clone.transform.SetParent(this.GetComponent<Button>().transform);
        rectTransform.anchoredPosition = new Vector3(0, 0, 0);
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.sizeDelta = new Vector2(35, 35);
        rectTransform.localScale = new Vector3(1, 1, 1);
    }

    private IEnumerator ZoomOutMenu()
    {
        int loops = 0;
        while (loops <= 5)
        {
            yield return new WaitForSeconds(0.001f);


            rootMenu.transform.localScale = new Vector3(loops, 0.01f, loops);
            loops++;
        }
    }

    private IEnumerator ZoomInMenu()
    {
        int loops = 5;
        while (loops >= 0)
        {
            yield return new WaitForSeconds(0.001f);


            rootMenu.transform.localScale = new Vector3(loops, 0.01f, loops);
            loops--;
        }
        rootMenu.SetActive(true);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //onLeft.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            HandleCloseMenuAndExportItem();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            //onMiddle.Invoke();
        }
    }

    private void HandleCloseMenuAndExportItem()
    {
        ZoomInWhenDisappear();
        ExportItemByName(nameCurItem);

    }
}
