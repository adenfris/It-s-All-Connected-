using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

public class Settings : Singleton<Settings>
{
    public string rssURL = "https://en.wikinews.org/w/index.php?title=Special:NewsFeed&feed=atom&categories=Published&notcategories=No%20publish%7CArchived%7CAutoArchived%7Cdisputed&namespace=0&count=30&hourcount=124&ordermethod=categoryadd&stablepages=only";
    public float zoomSpeed = 500.0f;
    public int shortestWordAllowed = 4;

    private static string wordBlacklistFilename = "Word Blacklist.txt";

    public static List<string> GetWordBlacklist()
    {
        var streamReader = new StreamReader(Application.dataPath + "/" + wordBlacklistFilename);
        var blacklistFile = streamReader.ReadToEnd();
        streamReader.Close();

        string[] words = blacklistFile.Split('\n');
        return words.Cast<string>().ToList();
    }
}
