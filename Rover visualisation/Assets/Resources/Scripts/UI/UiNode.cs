using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiNode : MonoBehaviour
{
    public GameObject nodeToDisable;
    public List<GameObject> nodes = new List<GameObject>();

    public void SelectNode(int nodeIndex)
    {
        if (nodeIndex >= this.nodes.Count)
            return;

        this.nodes[nodeIndex].SetActive(true);
        this.nodeToDisable.SetActive(false);
    }
}
