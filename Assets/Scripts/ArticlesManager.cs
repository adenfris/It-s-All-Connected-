using System.Collections.Generic;

using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

using HtmlAgilityPack;

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ArticlesManager : MonoBehaviour
{

    [SerializeField]
    private GameObject articlePrefab;
    [SerializeField]
    private GameObject connectionPrefab;
    [SerializeField]
    UserInterface userInterface;

    private List<string> wordBlacklist;

    private List<Connection> connectionsList = new List<Connection>();

    private SyndicationFeed rssFeed;

    private int numberOfArticles;

    // Start is called before the first frame update
    void Start()
    {
        VerifyInspectorSettings();

        wordBlacklist = Settings.GetWordBlacklist();

        rssFeed = DownloadRSSFeed(Settings.Instance.rssURL);
    }

    public void AddArticle()
    {
        AddArticle(numberOfArticles);
    }

    public void ConnectArticles(Article articleOne, Article articleTwo)
    {
        GameObject newConnectionObject = GameObject.Instantiate(connectionPrefab);
        newConnectionObject.transform.SetParent(this.transform);

        Connection newConnection = newConnectionObject.GetComponent<Connection>();
        newConnection.SetConnection(articleOne, articleTwo, userInterface);

        ConnectionCompare connectionCompare = new ConnectionCompare();

        bool foundDuplicate = false;
        foreach (Connection otherConnection in connectionsList)
        {
            if (connectionCompare.Compare(newConnection, otherConnection) == 0)
            {
                foundDuplicate = true;
                break;
            }
        }

        if (foundDuplicate)
        {
            Destroy(newConnectionObject);
        }
        else
        {
            connectionsList.Add(newConnection);
            userInterface.AddToScore(newConnection.actualScore, newConnection.possibleScore);
        }
    }

    private void AddArticle(int articleIndex)
    {
        SyndicationItem article = rssFeed.Items.ElementAt<SyndicationItem>(articleIndex);

        string articleHeadline = article.Title.Text;

        string articleBody = HtmlToRawText(article);

        GameObject newArticleObject = Instantiate(articlePrefab);
        newArticleObject.transform.SetParent(this.transform);

        Article newArticle = newArticleObject.GetComponent<Article>();

        newArticle.InitializeArticle(this, articleHeadline, articleBody, wordBlacklist);

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

        if (connectionPrefab == null)
        {
            Debug.LogError("Connection prefab not set in ArticleManager inspector settings!");
        }
        else if (connectionPrefab.GetComponent<Connection>() == null)
        {
            Debug.LogError("Connection prefab set in ArticleManager has no Connection component!");
        }

        if (userInterface == null)
        {
            Debug.LogError("User Interface not set in ArticleManager inspector settings!");
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
