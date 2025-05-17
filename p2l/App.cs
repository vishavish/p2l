using System.Diagnostics;
using System.Text;


class App()
{
	List<Task> tasks = new();
	Dictionary<string, string[]> fileInfo = new();
	const string FILENAME = "Timestamp.txt";
	string[] lines = File.ReadAllLines(FILENAME);

	public void Init(ReadOnlySpan<char> dirPath)
	{
		if(!File.Exists(Path.Combine(dirPath.ToString(), FILENAME)))
		{
			Console.WriteLine($"{ FILENAME } could not be found.");
			return;
		}

		for(int i = 0; i < lines.Length; i++)
		{
			(string name, string[] timestamps) = GetLineInfo(lines[i]);
			fileInfo.Add(name, timestamps);
		}

		// foreach (KeyValuePair<string, string[]> kv in fileInfo)
		// {
		// 	tasks.Add(Task.Run(() => CutVideo(kv.Key, kv.Value)));
		// }

		// await Task.WhenAll(tasks);

		Console.WriteLine("------SUMMARY-------");
		Console.WriteLine("Press any key to close . . .");

		Console.ReadKey();
	}

	private void CutVideo(string input, string[] stamps)
	{
		string name = Path.GetFileNameWithoutExtension(input);
		Console.Write(name);
		StringBuilder sb = new StringBuilder();
		sb.Append($"-c \"ffmpeg -i {input} -c:a copy -c:v libx264 -crf 18 ");
		for (int i = 0; i < stamps.Length; i++)
		{
			sb.Append($"-ss { stamps[i].Split('-')[0] } -to { stamps[i].Split('-')[1] } {name}_{i}.mp4 ");
		}
		sb.Append("-hide_banner \"");

		ProcessStartInfo psi = new ProcessStartInfo()
		{
			FileName = "/bin/bash",
			Arguments = sb.ToString(),
			UseShellExecute = false,
			RedirectStandardOutput = false
		};

		using var proc = Process.Start(psi);
		proc!.WaitForExit();
	}

	private (string, string[]) GetLineInfo(string line)
	{
		char delimiter = ',';
		int openBidx = line.IndexOf('[');
		return (line.Substring(0, openBidx), line.Substring(openBidx + 1).Replace(']', ' ').Split(delimiter));
	}
}
