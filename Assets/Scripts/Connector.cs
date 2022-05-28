using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

public class Connector : MonoBehaviour
{
    private const string CardsURL = "http://a0677209.xsph.ru/test.php";
    private const string AuthURL = "http://a0677209.xsph.ru/auth.php";
    private const string DataURL = "http://a0677209.xsph.ru/infoExtend.php";
    private const string GameURL = "http://a0677209.xsph.ru/gameLogic.php";

    public static string SendCard(CardCharacter card)
    {
        return Post(CardsURL, JsonUtility.ToJson(card));
    }

    public static CardCharacter GetCardByID(int id)
    {
        var data = "{\"query\":\"queryCard\", \"Id\":" + id + "}";
        return Parser.GetCardFromJson(Post(CardsURL, data));
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
            errors = "Возникла ошибка!";
            if (ans.Contains("bad pass"))
                errors = "Пароль неверный!";
            else if (ans.Contains("not found"))
                errors = "Пользователь не найден!";
            
            return false;
        }

        errors = ans;
        
        return true;
    }

    private static string Post(string url, string data)
    {

        try
        {
            var req = (HttpWebRequest) WebRequest.Create(url);

            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0";
            req.Method = "POST";
            req.Timeout = 1000000;
            req.ContentType = "application/json; charset=utf-8";
            var sentData = Encoding.Default.GetBytes(data);
            req.ContentLength = sentData.Length;
            var sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();
            var res = req.GetResponse();
            var receiveStream = res.GetResponseStream();
            if (receiveStream == null)
                return string.Empty;
            var result = string.Empty;
            var sr = new System.IO.StreamReader(receiveStream, Encoding.UTF8);
            var read = new char[256];
            var count = sr.Read(read, 0, 256);

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
        catch (WebException e)
        {
            print($"An error occured! Message: {e.Message}. Trying to rePOST");
            return Post(url, data);
        }
    }

    public static string Register(string login, string password)
    {
        const string ex0 = "{\"query\":\"register\", \"login\":\"";
        const string ex1 = "\", \"password\":\"";
        const string ex2 = "\"}";
        var data = ex0 + login + ex1 + password + ex2;
        var ans = Post(AuthURL, data);
        return ans;
    }

    public static IEnumerable<CardCharacter> GetCollection(IEnumerable<int> ids)
        => ids.Select(GetCardByID);

    public static IEnumerable<int> GetCollectionIDs(string login)
    {
        const string ex0 = "{\"query\":\"getIdCardCollection\", \"login\":\"";
        const string ex2 = "\"}";
        var data = ex0 + login + ex2;
        var result = Post(CardsURL, data);
        if (result.Contains("errors"))
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
        SetProperty("balance", "300", login);
        SetProperty("level", "1", login);
        
        SetProperty(Account.DeckNames, "[]", login);

        switch (tribe)
        {
            case Tribes.Beaver:
                InitCollection(login, Enumerable.Range(0, 10));
                break;
            case Tribes.Magpie:
                InitCollection(login, new []{10,11,12,13,14,15,16,17,18,21});
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
        var data = ex0 + key + ex1 + login + ex3;
        var res = Post(DataURL, data).Split('\"')[3];
        //print(res);
        return res;
    }

    public static List<Room> GetRoomsList()
    {
        const string data = "{\"query\":\"getListRooms\"}";
        var res = Post(GameURL, data);
        var jsons = res.GetJsons();
        var rooms = new List<Room>();
        foreach (var room in jsons.Select(sJson => sJson
            .Replace("\"1\"", "\"FPLegacy\"")
            .Replace("\"2\"", "\"SPLegacy\"")
            .Replace("\"name\"", "\"Name\"")
            .Replace("\"data\"", "\"DataString\"")
            .Replace("\"lastTurn\"", "\"LastTurn\"")).Select(Room.CreateFromJson))
        {
            room.Name = room.Name.FromSystemRoom();
            rooms.Add(room);
        }

        return rooms;
    }

    public static string CreateRoom(string token, string name)
    {
        const string ex0 = "{\"query\":\"createRoom\", \"token\":\"";
        const string ex1 = "\", \"name\":\"";
        const string ex3 = "\"}";
        var data = ex0 + token + ex1 + name + ex3;
        var res = Post(GameURL, data);
        return res;
    }

    public static string JoinRoom(string token, string name)
    {
        const string ex0 = "{\"query\":\"joinRoom\", \"token\":\"";
        const string ex1 = "\", \"name\":\"";
        const string ex3 = "\"}";
        var data = ex0 + token + ex1 + name + ex3;
        var res = Post(GameURL, data);
        return res;
    }

    public static string SendRoom(string name, string token, string data)
    {
        const string ex0 = "{\"query\":\"setData\", \"name\":\"";
        const string ex1 = "\", \"token\":\"";
        const string ex2 = "\", \"data\":";
        const string ex3 = "}";
        var customData = ex0 + name + ex1 + token + ex2 + data + ex3;
        //print(customData);
        return Post(GameURL, customData);
    }

    public static Room GetRoom(string name, string token)
    {
        const string ex0 = "{\"query\":\"getData\", \"name\":\"";
        const string ex1 = "\", \"token\":\"";
        const string ex3 = "}";
        var customData = ex0 + name + ex1 + token + ex3;
        var res = Post(GameURL, customData).GetJsons()[0];
        
        var room = new Room
        {
            DataString = res
        };
        room.Pull();
        return room;
    }

    public static string DestroyRoom(string token, string name)
    {
        const string ex0 = "{\"query\":\"exitRoom\", \"token\":\"";
        const string ex1 = "\", \"name\":\"";
        const string ex3 = "\"}";
        
        var data = ex0 + token + ex1 + name + ex3;
        print(data);
        var res = Post(GameURL, data);
        return res;
    }
}