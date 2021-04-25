using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Linq;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class Article : MonoBehaviour
{
    public Dictionary<string, int> wordCounts;
    
    public Pin pin;
    
    public TMP_Text articleHeadline;

    [SerializeField]
    private GameObject stringObject;
    [SerializeField]
    private TMP_Text articleBody;

    private ArticlesManager manager;

    private LineRenderer stringRenderer;

    private List<string> wordBlacklist;

    private bool draggingArticle = false;
    private bool draggingString = false;
    private Vector2 orignalMousePosition = Vector3.zero;

    private void Update() {
        DragArticle();
        DragString();
    }

    private void OnMouseDown() {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            GameObject currentObject = EventSystem.current.currentSelectedGameObject;
            if (currentObject != null && currentObject.transform.gameObject.GetComponent<Draggable>() == null)
            {
                return;
            }
        }

        draggingArticle = true;
        orignalMousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Start is called before the first frame update
    public void InitializeArticle(ArticlesManager manager, string articleHeadline, string articleBody, List<string> wordBlacklist)
    {
        this.manager = manager;
        this.articleHeadline.text = articleHeadline;
        this.articleBody.text = articleBody;
        this.wordBlacklist = wordBlacklist;
        
        VerifyInspectorSettings();

        stringRenderer = stringObject.GetComponent<LineRenderer>();
        stringRenderer.enabled = false;

        wordCounts = DoWordCount();
    }

    public void PinClicked(Vector2 pinStringPosition)
    {
        draggingString = true;

        stringRenderer.enabled = true;

        stringRenderer.SetPosition(0, pinStringPosition);
    }

    private void VerifyInspectorSettings()
    {
        if (manager == null)
        {
            Debug.LogError("Articles Manager not assigned to article!");
        }

        if (pin == null)
        {
            Debug.LogError("Pin collider not assigned to article in inspector!");
        }

        if (stringObject == null)
        {
            Debug.LogError("String prefab not assigned to article in inspector!");
        }
        else if (stringObject.GetComponent<LineRenderer>() == null)
        {
            Debug.LogError("String prefab assigned to article has no LineRenderer!");
        }

        if (articleHeadline == null)
        {
            Debug.LogError("Article headline not assigned to article in inspector!");
        }

        if (articleBody == null)
        {
            Debug.LogError("Article body not assigned to article in inspector!");
        }
    }
    
    private Dictionary<string, int> DoWordCount()
    {
        Regex removeNonLettersRegex = new Regex("[^a-zA-Z \\-]+");
        string parsedBody = removeNonLettersRegex.Replace(articleBody.text, "");

        parsedBody = parsedBody.ToLower();

        Regex fixTooLongSpacesRegex = new Regex("[\\s]+");
        parsedBody = fixTooLongSpacesRegex.Replace(parsedBody, " ");

        string[] words = parsedBody.Split(' ');

        var groupedWords = words.GroupBy(word => word)
            .OrderByDescending(wordGroup => wordGroup.Count())
            .Where(longWord => WordAllowed(longWord.Key, wordBlacklist));

        Dictionary<string, int> wordCountDictionary = new Dictionary<string, int>();
        foreach (var group in groupedWords)
        {
            wordCountDictionary.Add(group.Key.ToString(), group.Count());
        }

        return wordCountDictionary;
    }

    private static bool WordAllowed(String word, List<string> wordBlacklist)
    {
        bool notTooShort = word.Length >= Settings.Instance.shortestWordAllowed;
        bool blacklisted = wordBlacklist.Any(blacklistWord => blacklistWord.Contains(word));

        return (notTooShort && !blacklisted);
    }

    private void DragArticle()
    {
        if (draggingArticle && Input.GetButton(InputNames.dragArticle))
        {
            Vector2 currentMousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 moveVector = currentMousePosition - orignalMousePosition;
            transform.Translate(moveVector, Space.Self);
            orignalMousePosition = currentMousePosition;
        }
        else
        {
            draggingArticle = false;
        }
    }

    private void DragString()
    {
        if (draggingString && Input.GetButton(InputNames.dragString))
        {
            Vector3 globalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 localMousePosition = this.transform.InverseTransformPoint(globalMousePosition);
            stringRenderer.SetPosition(1, localMousePosition);
        }
        else if (draggingString && !Input.GetButton(InputNames.dragString)) // We just stopped dragging
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] mouseHits = Physics2D.RaycastAll(mousePos, Vector2.up, 0.0000001f);
            
            Pin droppedPin = null;
            foreach (RaycastHit2D hit in mouseHits)
            {
                droppedPin = hit.collider.gameObject.GetComponent<Pin>();
                if (droppedPin != null)
                {
                    manager.ConnectArticles(this, droppedPin.article);
                    break;
                }
            }

            stringRenderer.enabled = false;
            draggingString = false;
        }
    }
}
