using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Connector: MonoBehaviour
{
    private const string CardsURL = "http://a0664388.xsph.ru/test.php";
    private const string AuthURL = "http://a0664388.xsph.ru/auth.php";

    public void GetCardByID(int id)
    {
        var data = "{\"query\":\"queryCard\", \"Id\":" + id + "}";
        print(Post(CardsURL, data));
    }

    public static bool TryLogin(string login, string password)
    {
        const string ex0 = "{\"query\":\"auth\", \"login\":\"";
        const string ex1 = "\", \"password\":\"";
        const string ex2 = "\"}";
        var data = ex0 + login + ex1 + password + ex2;
        var ans = Post(AuthURL, data);
        if (ans.Contains("errors"))
        {
            print(ans.Split('\"')[3]);
            return false;
        }
        return true;
    }

    private static string Post(string url, string data)
    {
        var req = System.Net.WebRequest.Create(url);
        req.Method = "POST";
        req.Timeout = 1000000;
        req.ContentType = "application/x-www-form-urlencoded";
        var sentData = Encoding.GetEncoding(1251).GetBytes(data);
        req.ContentLength = sentData.Length;
        var sendStream = req.GetRequestStream();
        sendStream.Write(sentData, 0, sentData.Length);
        sendStream.Close();
        var res = req.GetResponse();
        var receiveStream = res.GetResponseStream();
        if (receiveStream == null) 
            return string.Empty;
        var sr = new System.IO.StreamReader(receiveStream, Encoding.UTF8);
        var read = new char[256];
        var count = sr.Read(read, 0, 256);
        var result = string.Empty;
        while (count > 0)
        {
            var str = new string(read, 0, count);
            result += str;
            count = sr.Read(read, 0, 256);
        }
        sr.Close();
        receiveStream.Close();
        return result;
    }

    public static bool TryRegister(string login, string password)
    {
        const string ex0 = "{\"query\":\"register\", \"login\":\"";
        const string ex1 = "\", \"password\":\"";
        const string ex2 = "\"}";
        var data = ex0 + login + ex1 + password + ex2;
        var ans = Post(AuthURL, data);
        if (ans.Contains("errors"))
        {
            print(ans.Split('\"')[3]);
            return false;
        }
        return true;
    }
}
