using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class Connection : MonoBehaviour
{
    public Article articleOne;
    public Article articleTwo;

    public string connectionWord;

    public int possibleScore = 0;
    public int actualScore;

    [SerializeField]
    private AudioManager audioManager;
    [SerializeField]
    private TMP_InputField wordInput;
    [SerializeField]
    private TMP_Text wordTextDisp;

    [SerializeField]
    private float stringZLayer = 5;

    private LineRenderer lineRenderer;

    private UserInterface userInterface;

    private void Start() {
        lineRenderer = this.GetComponent<LineRenderer>();
    }

    private void Update() {
        UpdateLineRenderer();
    }

    public void SetConnection(Article articleOne, Article articleTwo, UserInterface userInterface)
    {
        this.userInterface = userInterface;

        if (lineRenderer == null)
        {
            lineRenderer = this.GetComponent<LineRenderer>();
        }

        lineRenderer.enabled = true;

        this.articleOne = articleOne;
        this.articleTwo = articleTwo;

        wordInput.enabled = true;
        wordInput.onEndEdit.AddListener(delegate{LockInput(wordInput);});

        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        audioManager.PlayConnectionEffect();
    }

    private void VerifyInspectorSettings()
    {
        if (wordTextDisp == null)
        {
            Debug.LogError("Word Text UI not assigned in connection inspector!");
        }
        if (wordInput == null)
        {
            Debug.LogError("Word Input UI not assigned in connection inspector!");
        }
    }
    void LockInput(TMP_InputField input)
	{
        wordTextDisp.enabled = true;
        connectionWord = wordInput.text;
        wordTextDisp.text = connectionWord;
		wordInput.gameObject.SetActive(false);
        CalculateScore();
	}

    private void UpdateLineRenderer()
    {
        if (articleOne == null || articleTwo == null)
        {
            return;
        }

        Vector3 posOne = (Vector2)articleOne.pin.transform.position + articleOne.pin.pinOffset;
        posOne.z = stringZLayer;

        Vector3 posTwo = (Vector2)articleTwo.pin.transform.position + articleTwo.pin.pinOffset;
        posTwo.z = stringZLayer;

        lineRenderer.SetPosition(0, posOne);
        lineRenderer.SetPosition(1, posTwo);

        Vector3 centerOfLine = (posOne + posTwo) / 2;
        wordInput.transform.position = centerOfLine;
        wordTextDisp.transform.position = centerOfLine;
    }

    private void CalculateScore()
    {
        if (!articleOne.wordCounts.ContainsKey(connectionWord) || !articleTwo.wordCounts.ContainsKey(connectionWord))
        {
            actualScore = 0;
        }
        else
        {
            int articleOneScore = (int)articleOne.wordCounts[connectionWord];
            int articleTwoScore = (int)articleTwo.wordCounts[connectionWord];

            actualScore = Mathf.Max(articleOneScore, articleTwoScore);
        }

        foreach (string word in articleOne.wordCounts.Keys)
        {
            if (articleTwo.wordCounts.ContainsKey(word))
            {
                int thisWordScore = Mathf.Max(articleOne.wordCounts[word], articleTwo.wordCounts[word]);
                possibleScore = Mathf.Max(possibleScore, thisWordScore);
            }
        }

        userInterface.AddToScore(actualScore, possibleScore);

        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        audioManager.PlayConnectionEffect();
    }
}

public class ConnectionCompare : Comparer<Connection>
{
    // Compares by Length, Height, and Width.
    public override int Compare(Connection x, Connection y)
    {
        if (x.articleOne.articleHeadline.text.CompareTo(y.articleOne.articleHeadline.text) == 0)
        {
            return x.articleTwo.articleHeadline.text.CompareTo(y.articleTwo.articleHeadline.text);
        }
        else if (x.articleOne.articleHeadline.text.CompareTo(y.articleTwo.articleHeadline.text) == 0)
        {
            return x.articleTwo.articleHeadline.text.CompareTo(y.articleOne.articleHeadline.text);
        }
        else
        {
            return -1;
        }
    }
}
