using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public ImageProcessor imageProcessor;
    public Slider slider;
    public UiNode uiNode;
    public int nodeIndex;

    private void Awake()
    {
        this.imageProcessor.OnImageProgress += OnImageProgress;
        this.imageProcessor.OnFinishedParsing += OnFinishedParsing;
    }

    private void OnFinishedParsing(object sender, EventArgs e)
    {
        this.imageProcessor.OnImageProgress -= OnImageProgress;
        this.imageProcessor.OnFinishedParsing -= OnFinishedParsing;
        this.uiNode.SelectNode(this.nodeIndex);
    }

    private void OnImageProgress(object sender, float progress)
    {
        this.slider.value = progress;
    }
}
