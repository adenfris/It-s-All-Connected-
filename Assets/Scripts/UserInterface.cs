using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class UserInterface : MonoBehaviour
{
    [SerializeField]
    private ArticlesManager articlesManager;

    [SerializeField]
    private TMP_Text scoreboard;

    private int totalScoreEarned = 0;
    private int totalScorePossible = 0;

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
        if (scoreboard == null)
        {
            Debug.LogError("Scoreboard not set in UserInterface inspector settings!");
        }
    }

    public void AddToScore(int scoreAdded, int scorePossible)
    {
        totalScoreEarned += scoreAdded;
        totalScorePossible += scorePossible;

        float scorePercent = totalScoreEarned / totalScorePossible;

        string newScoreText = string.Format("Score: {0} ({1}%)\n({2} Possible)", totalScoreEarned, scorePercent, totalScorePossible);

        scoreboard.text = newScoreText;
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
