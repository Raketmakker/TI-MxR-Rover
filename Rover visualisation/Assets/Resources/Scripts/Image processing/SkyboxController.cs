using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    private List<Material> images;
    public ImageProcessor imageProcessor;
    public Material baseMaterial;

    private void Awake()
    {
        this.imageProcessor.OnFinishedParsing += OnFinishedParsing;
    }

    /// <summary>
    /// Removes the event's subscription and destroys the imageProcessor.
    /// </summary>
    /// <param name="sender"> The event's sender. </param>
    /// <param name="e"> Empty event args. </param>
    private void OnFinishedParsing(object sender, EventArgs e)
    {
        this.imageProcessor.OnFinishedParsing -= OnFinishedParsing;
        CreateSkyboxMaterials();
    }

    /// <summary>
    /// Creates the materials for the skybox from an image.
    /// Also sets the first image in the skybox.
    /// </summary>
    private void CreateSkyboxMaterials()
    {
        this.images = new List<Material>();
        foreach (Texture2D tex in this.imageProcessor.textureColors.Keys)
        {
            var mat = new Material(baseMaterial);
            mat.mainTexture = tex;
            this.images.Add(mat);
        }
        RenderSettings.skybox = this.images[0];
    }
}
