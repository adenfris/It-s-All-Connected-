using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    public Article article;

    public Vector2 pinOffset = Vector2.zero;

    void Start()
    {
        VerifyInspectorSettings();
    }

    private void OnMouseDown() {
        article.PinClicked((Vector2)this.transform.localPosition + pinOffset);
    }

    private void VerifyInspectorSettings()
    {
        if (article == null)
        {
            Debug.LogError("Article not assigned to pin in inspector!");
        }
    }
}
