using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScrollViewItemCreator : MonoBehaviour
{
    public GameObject contentHolder;

    //Spawns the scrollview items as content holder children
    public void FillScrollView(List<GameObject> scrollViewItems)
    {
        foreach(var item in scrollViewItems)
        {
            Instantiate(item, this.contentHolder.transform);
        }
    }

    //Abstract function to create scrollview items
    public abstract List<GameObject> CreateItems();
}
