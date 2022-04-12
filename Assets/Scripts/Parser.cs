using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Parser
{
	private Encoding Encode = Encoding.UTF8;

	public void ResetEncode(Encoding WSet)
	{
		Encode = WSet;
	}

	public async void ConvertToFile(List<CardСharacter> data)
	{
		foreach (var it in data)
		{
			FileStream fStream = new FileStream(Application.dataPath + "\\" + it.Id + ".json", FileMode.Create);
			StringBuilder sb = new StringBuilder();
			foreach (var it1 in it.Ability)
				sb.Append(it1.ToString() + " ");
			it.AbilityString = sb.ToString();
			byte[] buffer = Encode.GetBytes(JsonUtility.ToJson(it));
			await fStream.WriteAsync(buffer, 0, buffer.Length);
			fStream.Close();
		}
	}

	public async Task<List<CardСharacter>> ConvertFromFile(List<String> filename, int indexOfLine = int.MaxValue)
	{
		var ans = new List<CardСharacter>();
		foreach (var fl in filename)
		{
			FileStream fStream = new FileStream(Application.dataPath + "\\" +  fl + ".json", FileMode.Open);
			byte[] buffer = new byte[fStream.Length];
			await fStream.ReadAsync(buffer, 0, buffer.Length);
			string jsonString = Encode.GetString(buffer);
			var cardСharacter = JsonUtility.FromJson<CardСharacter>(jsonString);
			var abilityString = cardСharacter.AbilityString;
			Queue<Basis> q = new Queue<Basis>();
			foreach (var it in abilityString.Split(' ').ToArray())
			{
				if (it == "")
					continue;
				q.Enqueue((Basis)Enum.Parse(typeof(Basis), it));
			}
			cardСharacter.Ability = q;
			ans.Add(cardСharacter);
			fStream.Close();
		}
		return ans;
	}
}



