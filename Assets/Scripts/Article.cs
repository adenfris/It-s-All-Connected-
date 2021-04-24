using System.Collections;
using System.Collections.Generic;

using System.ServiceModel.Syndication;
using System.Xml;

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

    // Start is called before the first frame update
    void Start()
    {        
        CheckInspectorSettings();

        string rssURL = "https://en.wikinews.org/w/index.php?title=Special:NewsFeed&feed=atom&categories=Published&notcategories=No%20publish%7CArchived%7CAutoArchived%7Cdisputed&namespace=0&count=30&hourcount=124&ordermethod=categoryadd&stablepages=only";

        SyndicationFeed rssFeed = DownloadRSSFeed(rssURL);

        foreach (SyndicationItem article in rssFeed.Items)
        {
            articleHeadline.text = article.Title.Text;

            HtmlDocument articleBodyHtml = new HtmlDocument();
            articleBodyHtml.LoadHtml(article.Summary.Text);
            HtmlNodeCollection htmlBody = articleBodyHtml.DocumentNode.SelectNodes("//*[@class=\"mw-parser-output\"]/p");
            htmlBody.RemoveAt(0); // Get rid of date code

            articleBody.text = "";
            foreach (HtmlNode paragraph in htmlBody)
            {
                articleBody.text += paragraph.InnerText;
            }

            break;
        }
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
}
