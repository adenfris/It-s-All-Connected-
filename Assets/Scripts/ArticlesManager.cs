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

public class ArticlesManager : MonoBehaviour
{

    [SerializeField]
    private GameObject articlePrefab;

    private List<string> wordBlacklist;

    private SyndicationFeed rssFeed;

    private int numberOfArticles;

    // Start is called before the first frame update
    void Start()
    {
        wordBlacklist = Settings.GetWordBlacklist();

        rssFeed = DownloadRSSFeed(Settings.Instance.rssURL);
    }

    public void AddArticle()
    {
        AddArticle(numberOfArticles);
    }

    private void AddArticle(int articleIndex)
    {
        SyndicationItem article = rssFeed.Items.ElementAt<SyndicationItem>(articleIndex);

        string articleHeadline = article.Title.Text;

        string articleBody = HtmlToRawText(article);

        GameObject newArticleObject = Instantiate(articlePrefab);
        Article newArticle = newArticleObject.GetComponent<Article>();

        newArticle.InitializeArticle(articleHeadline, articleBody, wordBlacklist);

        numberOfArticles++;
    }

    private void VerifyInspectorSettings()
    {
        if (articlePrefab == null)
        {
            Debug.LogError("Articles prefab not set in ArticleManager inspector settings!");
        }
        else if (articlePrefab.GetComponent<Article>() == null)
        {
            Debug.LogError("Articles prefab set in ArticleManager has no Article script attached!");
        }
    }

    private SyndicationFeed DownloadRSSFeed(string rssURL)
    {
        XmlReader xmlReader = XmlReader.Create(Settings.Instance.rssURL);
        SyndicationFeed rssFeed = SyndicationFeed.Load(xmlReader);
        xmlReader.Close();

        return rssFeed;
    }

    private string HtmlToRawText(SyndicationItem html)
    {
        string rawText = "";

        HtmlDocument articleBodyHtml = new HtmlDocument();
        articleBodyHtml.LoadHtml(html.Summary.Text);
        HtmlNodeCollection htmlBody = articleBodyHtml.DocumentNode.SelectNodes("//*[@class=\"mw-parser-output\"]/p");
        htmlBody.RemoveAt(0); // Get rid of date code

        foreach (HtmlNode paragraph in htmlBody)
        {
            rawText += paragraph.InnerText;
        }

        return rawText;
    }
}
