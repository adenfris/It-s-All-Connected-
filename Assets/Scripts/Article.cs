using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Article : MonoBehaviour
{
    [SerializeField]
    private TMP_Text articleHeadline;
    [SerializeField]
    private TMP_Text articleBody;

    private List<string> wordBlacklist;

    private OrderedDictionary wordCounts;

    // Start is called before the first frame update
    public void InitializeArticle(string articleHeadline, string articleBody, List<string> wordBlacklist)
    {
        this.articleHeadline.text = articleHeadline;
        this.articleBody.text = articleBody;
        this.wordBlacklist = wordBlacklist;
        
        CheckInspectorSettings();

        wordCounts = DoWordCount();

        string wordCountString = "";
        foreach (KeyValuePair<string, int> word in wordCounts)
        {
            wordCountString += string.Format("\"{0}\", {1} occurances\n", word.Key, word.Value);
        }
        this.articleBody.text = wordCountString;
    }

    private void CheckInspectorSettings()
    {
        if (articleHeadline == null)
        {
            Debug.LogError("Article headline not assigned in inspector!");
        }

        if (articleBody == null)
        {
            Debug.LogError("Article body not assigned in inspector!");
        }
    }
    
    private OrderedDictionary DoWordCount()
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

        OrderedDictionary wordCountDictionary = new OrderedDictionary();
        foreach (var group in groupedWords)
        {
            wordCountDictionary.Add(group.Key, group.Count());
        }

        return wordCountDictionary;
    }

    private static bool WordAllowed(String word, List<string> wordBlacklist)
    {
        bool notTooShort = word.Length >= Settings.Instance.shortestWordAllowed;
        bool blacklisted = wordBlacklist.Any(blacklistWord => blacklistWord.Contains(word));

        return (notTooShort && !blacklisted);
    }
}
