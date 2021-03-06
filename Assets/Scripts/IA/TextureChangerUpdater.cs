﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChangerUpdater : MonoBehaviour {

    #region Fields
    private TextureChangerSource textureChangerSource;
    private Renderer mRenderer;
    private Material[] materials;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        textureChangerSource = GetComponentInParent<TextureChangerSource>();
        UnityEngine.Assertions.Assert.IsNotNull(textureChangerSource, "ERROR: TextureChangerUpdater in gameObject '" + gameObject.name + "' doesn't have a TextureChangerSource assigned!");
        mRenderer = GetComponent<Renderer>();
        UnityEngine.Assertions.Assert.IsNotNull(mRenderer, "ERROR: TextureChangerUpdater in gameObject '" + gameObject.name + "' couldn't find a Renderer in its GameObject!");
        materials = mRenderer.sharedMaterials;
    }

    private void Update()
    {
        foreach (Material material in materials)
            textureChangerSource.UpdateMaterial(material);
    }
    #endregion

#if UNITY_EDITOR
    private bool valid
    {
        get
        {
            return textureChangerSource != null && mRenderer != null && materials != null;
        }
    }

    public void EditorUpdate()
    {
        if (!Application.isPlaying)
        {
            if (!valid)
            {
                Awake();
            }

            foreach (Material material in materials)
                textureChangerSource.UpdateMaterial(material);
        }
    }
#endif
}
