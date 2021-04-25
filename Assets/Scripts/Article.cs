using System;
using System.Collections;
using System.Collections.Generic;

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
    private int articleIndex;

    [SerializeField]
    private TMP_Text articleHeadline;
    [SerializeField]
    private TMP_Text articleBody;

    private List<string> wordBlacklist;

    // Start is called before the first frame update
    void Start()
    {
        wordBlacklist = Settings.GetWordBlacklist();

        CheckInspectorSettings();

        string rssURL = "https://en.wikinews.org/w/index.php?title=Special:NewsFeed&feed=atom&categories=Published&notcategories=No%20publish%7CArchived%7CAutoArchived%7Cdisputed&namespace=0&count=30&hourcount=124&ordermethod=categoryadd&stablepages=only";

        ExtractRSSToArticle(rssURL);
    }

    // Update is called once per frame
    void Update()
    {

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

    private SyndicationFeed DownloadRSSFeed(string rssURL)
    {
        XmlReader xmlReader = XmlReader.Create(rssURL);
        SyndicationFeed rssFeed = SyndicationFeed.Load(xmlReader);
        xmlReader.Close();

        return rssFeed;
    }

    private void ExtractRSSToArticle(string rssURL)
    {
        SyndicationFeed rssFeed = DownloadRSSFeed(rssURL);

        SyndicationItem article = rssFeed.Items.ElementAt<SyndicationItem>(articleIndex);

        articleHeadline.text = article.Title.Text;

        string rawBody = HtmlToRawText(article);

        string processedBody = DoWordCount(rawBody);

        articleBody.text = processedBody;
    }

    private string HtmlToRawText(SyndicationItem article)
    {
        string articleText = "";

        HtmlDocument articleBodyHtml = new HtmlDocument();
        articleBodyHtml.LoadHtml(article.Summary.Text);
        HtmlNodeCollection htmlBody = articleBodyHtml.DocumentNode.SelectNodes("//*[@class=\"mw-parser-output\"]/p");
        htmlBody.RemoveAt(0); // Get rid of date code

        foreach (HtmlNode paragraph in htmlBody)
        {
            articleText += paragraph.InnerText;
        }

        return articleText;
    }

    private string DoWordCount(string rawArticleText)
    {
        Regex removeNonLettersRegex = new Regex("[^a-zA-Z \\-]+");
        string parsedBody = removeNonLettersRegex.Replace(rawArticleText, "");

        parsedBody = parsedBody.ToLower();

        Regex fixTooLongSpacesRegex = new Regex("[\\s]+");
        parsedBody = fixTooLongSpacesRegex.Replace(parsedBody, " ");

        string[] words = parsedBody.Split(' ');

        var groupedWords = words.GroupBy(word => word)
            .OrderByDescending(wordGroup => wordGroup.Count())
            .Where(longWord => WordAllowed(longWord.Key, wordBlacklist));

        string wordCountResult = "";
        foreach (var group in groupedWords)
        {
            wordCountResult += string.Format("\"{0}\", {1} occurances\n", group.Key, group.Count());
        }

        return wordCountResult;
    }

    private static bool WordAllowed(String word, List<string> wordBlacklist)
    {
        bool notTooShort = word.Length >= Settings.Instance.shortestWordAllowed;
        bool blacklisted = wordBlacklist.Any(blacklistWord => blacklistWord.Contains(word));

        return (notTooShort && !blacklisted);
    }
}
