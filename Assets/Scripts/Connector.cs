using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Connector: MonoBehaviour
{
    private const string CardsURL = "http://a0664388.xsph.ru/test.php";
    private const string AuthURL = "http://a0664388.xsph.ru/auth.php";
    private const string DataURL = "http://a0664388.xsph.ru/infoExtend.php";

    public static string GetCardByID(int id)
    {
        var data = "{\"query\":\"queryCard\", \"Id\":" + id + "}";
        return Post(CardsURL, data);
    }

    public static bool TryLogin(string login, string password, out string errors)
    {
        const string ex0 = "{\"query\":\"auth\", \"login\":\"";
        const string ex1 = "\", \"password\":\"";
        const string ex2 = "\"}";
        var data = ex0 + login + ex1 + password + ex2;
        var ans = Post(AuthURL, data);
        //print(ans);
        if (ans.Contains("errors"))
        {
            errors = ans.Split('\"')[3];
            return false;
        }

        errors = "";
        return true;
    }

    private static string Post(string url, string data)
    {
        var req = (HttpWebRequest)WebRequest.Create(url);
        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0";
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

    public static bool TryRegister(string login, string password, out string errors)
    {
        const string ex0 = "{\"query\":\"register\", \"login\":\"";
        const string ex1 = "\", \"password\":\"";
        const string ex2 = "\"}";
        var data = ex0 + login + ex1 + password + ex2;
        var ans = Post(AuthURL, data);
        //print(ans);
        if (ans.Contains("errors"))
        {
            errors = ans.Split('\"')[3];
            return false;
        }

        errors = "";
        return true;
    }

    public static IEnumerable<CardCharacter> GetCollection(IEnumerable<int> ids) 
        => ids.Select(id => Parser.GetCardFromJson(GetCardByID(id)));
    
    public static IEnumerable<int> GetCollectionIDs(string login)
    {
        const string ex0 = "{\"query\":\"getIdCardCollection\", \"login\":\"";
        const string ex2 = "\"}";
        var data = ex0 + login  + ex2;
        var result = Post(CardsURL, data);
        if(result.Contains("errors"))
            yield break;
        var left = result.IndexOf("[", StringComparison.Ordinal);
        var right = result.IndexOf("]", StringComparison.Ordinal);
        var sub = result
            .Substring(left + 1, right - left - 1)
            .Split(',')
            .Select(int.Parse);
        foreach (var id in sub)
            yield return id;
    }
    public static int GetMaxID()
    {
        var data = "{\"query\":\"maxId\"}";
        var result = Post(CardsURL, data);
        return int.Parse(result.Split('\"')[3]);
    }

    public static void InitNewUser(string login, Tribes tribe)
    {
        SetProperty("balance", "100", login);
        SetProperty("level", "1", login);
        
        switch (tribe)
        {
            case Tribes.Beaver:
                InitCollection(login, Enumerable.Range(0, 10));
                break;
            case Tribes.Magpie:
                InitCollection(login, Enumerable.Range(10, 10));
                break;
            case Tribes.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tribe), tribe, null);
        }
    }

    public static void InitCollection(string login, IEnumerable<int> ids)
    {
        const string ex0 = "{\"query\":\"overwriteIdCardCollection\", \"login\":\"";
        const string ex1 = "\", \"deq\":[";
        const string ex2 = "]}";
        var data = ex0 + login + ex1 + string.Join(",", ids) + ex2;
        var res = Post(CardsURL, data);
        print(res);
    }

    public static void SetProperty(string key, string value, string login)
    {
        const string ex0 = "{\"query\":\"setInfo\", \"data\":\"";
        const string ex1 = "\", \"login\":\"";
        const string ex2 = "\", \"value\":\"";
        const string ex3 = "\"}";
        var data = ex0 + key + ex1 + login + ex2 + value + ex3;
        Post(DataURL, data);
    }

    public static string GetProperty(string key, string login)
    {
        const string ex0 = "{\"query\":\"getInfo\", \"data\":\"";
        const string ex1 = "\", \"login\":\"";
        const string ex3 = "\"}";
        var data = ex0 + key + ex1 + login  + ex3;
        var res= Post(DataURL, data).Split('\"')[3];
        print(res);
        return res;
    }
}
