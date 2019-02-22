using Editions;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public interface IImageHolder
{
    string ImageUrl { get; set; }
}

public static class ImageManager
{
    static public IEnumerator GetTexture(System.Action<Texture2D> callback, string url)
    {
        string filePath = Application.persistentDataPath + "/" + Edition.Current.Name + "/ImageCache";
        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
        filePath += "/" + url.GetHashCode() + ".png";
        bool web = false;
        UnityWebRequest www;
        if (File.Exists(filePath) && new FileInfo(filePath).Length > 100)
        {
            string pathforwww = "file://" + filePath;
            www = UnityWebRequestTexture.GetTexture(pathforwww);
        }
        else
        {
            web = true;
            www = UnityWebRequestTexture.GetTexture(url);
        }
        yield return www.SendWebRequest();
        if (www.error == null)
        {
            if (web)
            {
                File.WriteAllBytes(filePath, www.downloadHandler.data);
            }
        }
        else
        {
            if (!web)
            {
                File.Delete(filePath);
            }
        }

        Texture2D texture = null;
        try
        {
            texture = DownloadHandlerTexture.GetContent(www);
        }
        catch
        {
            Debug.Log("Cannot download image");
        }

        callback(texture);
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
