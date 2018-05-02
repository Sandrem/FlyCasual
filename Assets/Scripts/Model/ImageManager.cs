using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ImageManager
{

    static public WWW GetImage(string url)
    {
        string filePath = Application.persistentDataPath + "/ImageCache";
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
            Debug.Log("WWW ERROR " + www.error);
        }
    }

}
