using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Article : MonoBehaviour
{
    [SerializeField]
    private TMP_Text articleBody;

    // Start is called before the first frame update
    void Start()
    {
        if (articleBody != null)
        {
            articleBody.text = "Is this working?";
        }
        else
        {
            Debug.LogError("Article body not assigned in inspector!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
