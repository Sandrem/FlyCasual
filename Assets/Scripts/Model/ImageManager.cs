using RuleSets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IImageHolder
{
    string ImageUrl { get; set; }
}

public static class ImageManager
{

    static public WWW GetImage(string url)
    {
        string filePath = Application.persistentDataPath + "/" + Edition.Instance.Name + "/ImageCache";
        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
        filePath += "/" + url.GetHashCode() + ".png";
        bool web = false;
        WWW www;
        if (File.Exists(filePath) && new FileInfo(filePath).Length > 100)
        {
            string pathforwww = "file://" + filePath;
            //Debug.Log("TRYING FROM CACHE " + url + "  file " + pathforwww);
            www = new WWW(pathforwww);
        }
        else
        {
            web = true;
            www = new WWW(url);
        }
        Global.Instance.StartCoroutine(DoLoad(www, filePath, web));
        return www;
    }

    static IEnumerator DoLoad(WWW www, string filePath, bool web)
    {
        yield return www;

        if (www.error == null)
        {
            if (web)
            {
                //System.IO.Directory.GetFiles
                //Debug.Log("SAVING DOWNLOAD  " + www.url + " to " + filePath);
                // string fullPath = filePath;
                File.WriteAllBytes(filePath, www.bytes);
                //Debug.Log("SAVING DONE  " + www.url + " to " + filePath);
                //Debug.Log("FILE ATTRIBUTES  " + File.GetAttributes(filePath));
                //if (File.Exists(fullPath))
                // {
                //    Debug.Log("File.Exists " + fullPath);
                // }
            }
            else
            {
                //Debug.Log("SUCCESS CACHE LOAD OF " + www.url);
            }
        }
        else
        {
            if (!web)
            {
                File.Delete(filePath);
            }
            Debug.Log("Trying to download..." + www.url);
            Debug.Log("WWW ERROR " + www.error);
        }
    }

    // DELETE CACHED IMAGES FOR MIGRATIONS

    static public void DeleteCachedImage(string url, Type edition = null)
    {
        if (edition != null)
        {
            DeleteCachedImageByEdition(url, edition);
        }
        else
        {
            DeleteCachedImageByEdition(url, typeof(FirstEdition));
            DeleteCachedImageByEdition(url, typeof(SecondEdition));
        }
    }

    private static void DeleteCachedImageByEdition(string url, Type edition)
    {
        Edition ruleSet = (Edition)Activator.CreateInstance(edition);
        string filePath = Application.persistentDataPath + "/" + ruleSet.Name + "/ImageCache";
        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
        filePath += "/" + url.GetHashCode() + ".png";
        if (File.Exists(filePath) && new FileInfo(filePath).Length > 100)
        {
            File.Delete(filePath);
        }
    }

}
