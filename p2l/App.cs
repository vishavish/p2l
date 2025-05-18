using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;


class App()
{
	const string FILENAME = "Timestamp.txt";
	
	Dictionary<string, string[]> fileInfo = new();
	string path = "";
	List<Task> tasks = new();
	
	public void Init(string dirPath)
	{
		Console.WriteLine("Checking file...");
		if (!File.Exists(Path.Combine(dirPath.ToString(), FILENAME)))
		{
			Console.WriteLine($"{ FILENAME } could not be found.");
			return;
		}

		Console.WriteLine("Reading file...");

		path = dirPath;
		string[] lines = File.ReadAllLines(FILENAME);
		for (int i = 0; i < lines.Length; i++)
		{
			(string name, string[] timestamps) = GetLineInfo(lines[i]);
			string rawFileName = Path.GetFileNameWithoutExtension(name);
			
			if (!File.Exists(name))
			{
				Console.WriteLine($"File [{ name.Trim() }] could not be found.");
				return;
			}
			
			if (!IsValidTimestamp(timestamps)) return;
			if (!Directory.Exists(Path.Combine(path, rawFileName)))
			{
				Directory.CreateDirectory(Path.Combine(path, rawFileName));
			}
			
			fileInfo.Add(name, timestamps);
		}

		foreach (KeyValuePair<string, string[]> kv in fileInfo) {
		 // tasks.Add(Task.Run(() => ProcessTimestamps(kv.Key, kv.Value)));
		  
		  ProcessTimestamps(kv.Key, kv.Value);
		}

		// await Task.WhenAll(tasks);

		Console.WriteLine("------SUMMARY-------");
		Console.WriteLine("Press any key to close . . .");

		Console.ReadKey();
	}
	
	private void ProcessTimestamps(string input, string[] stamps)
	{
		bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
		StringBuilder sb = new StringBuilder();
		string name = Path.GetFileNameWithoutExtension(input);
		string programName = isWindows ? "cmd" : "/bin/bash" ;
		
		if (isWindows)
		{
			sb.Append($"/c"); 
		}
		else
		{
			sb.Append($"-c"); 
		}
		sb.Append($"ffmpeg -i {input} -c:a copy -c:v libx264 -crf 18 ");
		for (int i = 0; i < stamps.Length; i++)
		{
			sb.Append($"-ss { stamps[i].Split('-')[0] } -to { stamps[i].Split('-')[1] } {name}\\{name}_{i}.mp4 ");
		}
		sb.Append("-hide_banner \"");

		ProcessStartInfo psi = new ProcessStartInfo()
		{
			FileName = programName,
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

	private bool IsValidTimestamp(string[] timestamp)
	{
		Console.WriteLine("Validating timestamps...");
		string format = @"mm\:ss";

		for(int i = 0; i < timestamp.Length; i++)
		{
			string[] tstampList = timestamp[i].Split('-');
			for (int j = 0; j < tstampList.Length; j++)
			{
				if (!TimeSpan.TryParseExact(tstampList[j].Trim(), format, CultureInfo.InvariantCulture, out TimeSpan _duration))
				{
					Console.WriteLine($"{ tstampList[j] }: invalid");
					return false;
				}
			}
		}
		
		return true;
	}
}
