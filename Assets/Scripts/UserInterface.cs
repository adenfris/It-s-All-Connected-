using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    [SerializeField]
    private ArticlesManager articlesManager;

    // Start is called before the first frame update
    void Start()
    {
        VerifyInspectorSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void VerifyInspectorSettings()
    {
        if (articlesManager == null)
        {
            Debug.LogError("Articles Manager not set in UserInterface inspector settings!");
        }
    }

    public void PressSettingsButton()
    {
        Debug.Log("Settings pressed!");
    }

    public void PressAddArticleButton()
    {
        articlesManager.AddArticle();
    }

    public void PressHandToolButton()
    {
        Debug.Log("Hand tool pressed!");
    }

    public void PressStringToolButton()
    {
        Debug.Log("String tool pressed!");
    }
}
