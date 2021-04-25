using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

public class Settings : Singleton<Settings>
{
    public float zoomSpeed = 500.0f;
    public int shortestWordAllowed = 4;

    private static string wordBlacklistFilename = "Word Blacklist.txt";

    public static List<string> GetWordBlacklist()
    {
        var streamReader = new StreamReader(Application.dataPath + "/" + wordBlacklistFilename);
        var blacklistFile = streamReader.ReadToEnd();
        streamReader.Close();

        string[] words = blacklistFile.Split("\n"[0]);
        return words.Cast<string>().ToList();
    }
}
