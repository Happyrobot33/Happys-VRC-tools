using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

//it is assumed that by default, the dropdown starts open
//upon start, it closes
public class JSONCollapse : UdonSharpBehaviour
{
    bool isCollapsed = false;
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //get parent
        GameObject parent = transform.parent.gameObject;
        //check to see if there is any children next to us in the hierarchy
        if (parent.transform.childCount == 1)
        {
            //if there isnt, then we are the last child, so we can delete ourselves
            Destroy(gameObject);
        }

        OnClick();
    }

    void OnEnable()
    {
        updateCollapse();
    }

    public void OnClick()
    {
        isCollapsed = !isCollapsed;

        updateCollapse();
    }

    void updateCollapse()
    {
        if (isCollapsed)
        {
            //get parent
            GameObject parent = transform.parent.gameObject;
            //get all children
            Transform[] children = parent.GetComponentsInChildren<Transform>(true);
            //loop through all children
            foreach (Transform child in children)
            {
                //if the child is not the parent, then we can hide it
                //also make sure that its only the first depth of children that we hide
                if (
                    child.gameObject != parent
                    && child.gameObject != gameObject
                    && child.gameObject.transform.parent == parent.transform
                )
                {
                    child.gameObject.SetActive(false);
                }
            }
            setPosClosed();
        }
        else
        {
            //get parent
            GameObject parent = transform.parent.gameObject;
            //get all children
            Transform[] children = parent.GetComponentsInChildren<Transform>(true);
            //loop through all children
            foreach (Transform child in children)
            {
                //if the child is not the parent, then we can show it
                //also make sure that its only the first depth of children that we show
                if (
                    child.gameObject != parent
                    && child.gameObject.transform.parent == parent.transform
                )
                {
                    child.gameObject.SetActive(true);
                }
            }
            setPosOpen();
        }
    }

    void setPosOpen()
    {
        //check if rectTransform is null
        if (rectTransform != null)
        {
            rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
            rectTransform.anchoredPosition = new Vector3(-50, -50);
        }
    }

    void setPosClosed()
    {
        //check if rectTransform is null
        if (rectTransform != null)
        {
            rectTransform.localRotation = Quaternion.Euler(0, 0, -90);
            rectTransform.anchoredPosition = new Vector3(0, -50);
        }
    }
}
