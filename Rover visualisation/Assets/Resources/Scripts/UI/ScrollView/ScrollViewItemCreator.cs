using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScrollViewItemCreator : MonoBehaviour
{
    public GameObject contentHolder;

    private void Awake()
    {
        FillScrollView(CreateItems());
    }

    //Spawns the scrollview items as content holder children
    public virtual void FillScrollView(List<GameObject> scrollViewItems)
    {
        foreach(var item in scrollViewItems)
        {
            Instantiate(item, this.contentHolder.transform);
        }
    }

    //Abstract function to create scrollview items
    public abstract List<GameObject> CreateItems();
}
