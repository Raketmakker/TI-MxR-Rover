using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItemCreator : ScrollViewItemCreator
{
    public GameObject itemToSpawn;

    private void Awake()
    {
        base.FillScrollView(CreateItems());
    }

    public override List<GameObject> CreateItems()
    {
        List<GameObject> items = new List<GameObject>();

        for (int i = 0; i < 10; i++)
        {
            items.Add(this.itemToSpawn);
        }
        return items;
    }
}
