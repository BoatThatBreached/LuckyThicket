using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Parser : MonoBehaviour
{
	private string Path = "";
	private Encoding Encode = Encoding.UTF8;

	public void ResetPath(String WPath)
	{
		Path = WPath;
	}

	public void ResetEncode(Encoding WSet)
	{
		Encode = WSet;
	}

	public async void ConvertToFile(Queue<Basis> data, String filename, bool allowRewrite)
	{
		StreamWriter f = new StreamWriter(Path + filename + ".txt", !allowRewrite, Encode);
		StringBuilder sb = new StringBuilder();
		foreach (Basis it in data)
			sb.Append(it.ToString() + " ");
		await f.WriteLineAsync(sb.ToString());
		f.Close();
	}

	public List<Queue<Basis>> ConvertFromFile(String filename, int indexOfLine = int.MaxValue)
	{
		StreamReader f = new StreamReader(Path + filename + ".txt", Encode);
		List<Queue<Basis>> ans = new List<Queue<Basis>>();
		int currentIndex = -1;
		while (!f.EndOfStream && indexOfLine != currentIndex)
		{
			string s = f.ReadLine();
			if (indexOfLine != int.MaxValue && currentIndex + 1 != indexOfLine)
			{
				currentIndex++;
				continue;
			}
			Queue <Basis> q = new Queue<Basis>();
			foreach (var it in s.Split(' ').ToArray())
			{
				if (it == "")
					continue;
				q.Enqueue((Basis)Enum.Parse(typeof(Basis), it));
			}
			ans.Add(q);
			currentIndex++;
		}
		f.Close();
		return ans;
	}
}

