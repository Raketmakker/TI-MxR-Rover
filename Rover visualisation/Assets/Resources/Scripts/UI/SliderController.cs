using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public ImageProcessor imageProcessor;
    public Slider slider;

    private void Awake()
    {
        this.imageProcessor.OnImageProgress += OnImageProgress;
    }

    private void OnImageProgress(object sender, float progress)
    {
        this.slider.value = progress;
    }
}
