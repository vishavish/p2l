using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;


class App()
{
	const string FILENAME = "Timestamp.txt";
	
	Dictionary<string, string[]> fileInfo = new();
	string path = "";
	
	public void Init(string dirPath)
	{
		Console.WriteLine("Checking file...");
		if (!File.Exists(Path.Combine(dirPath, FILENAME)))
		{
			throw new Exception($"{ FILENAME } could not be found.");
		}

		Console.WriteLine("Reading file...");
		path = dirPath;
		string[] lines = File.ReadAllLines(Path.Combine(dirPath, FILENAME));
		for (int i = 0; i < lines.Length; i++)
		{
			(string name, string[] timestamps) = GetLineInfo(lines[i]);
			string rawFileName = Path.GetFileNameWithoutExtension(name);
			
			if (!File.Exists(Path.Combine(dirPath, name)))	
			{
				throw new Exception($"Input file [{ name }] could not be found.");
			}
			
			Console.WriteLine($"Validating timestaps [{ i + 1 }/{ lines.Length }]");
			ValidateTimestamps(timestamps);
						
			if (!Directory.Exists(Path.Combine(dirPath, rawFileName))) 
			{
				Directory.CreateDirectory(Path.Combine(dirPath, rawFileName));
			}
			
			fileInfo.Add(name, timestamps);
		}

		foreach (KeyValuePair<string, string[]> kv in fileInfo)
		{
			ProcessTimestamps(kv.Key, kv.Value);
		}

		Console.WriteLine("Press any key to close . . .");
		Console.ReadKey();
	}
	
	private int ProcessTimestamps(string input, string[] stamps)
	{
		StringBuilder sb = new StringBuilder();
		bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
		string name = Path.GetFileNameWithoutExtension(input);
		string programName = isWindows ? "cmd" : "/bin/bash" ;
		string separator = isWindows ? "\\" : "//" ;
		
		if (isWindows)
		{
			sb.Append($"/c"); 
		}
		else
		{
			sb.Append($"-c "); 
		}

		sb.Append($"\"ffmpeg -i {path}{input} -c:a copy -c:v libx264 -crf 18 ");
		for (int i = 0; i < stamps.Length; i++)
		{
			sb.Append($"-ss { stamps[i].Split('-')[0] } -to { stamps[i].Split('-')[1] } {path}{name}{separator}{name}_{i}.mp4 ");
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
		
		return proc.ExitCode;
	}

	private (string, string[]) GetLineInfo(string line)
	{
		char delimiter = ',';
		int openBidx = line.IndexOf('[');
		int closeBidx = line.LastIndexOf(']');
		
		if (openBidx < 0 || closeBidx < 0)
		{
			throw new Exception("Missing `[` or `]` in your Timestamp.txt file.");
		}
		
		return (line.Substring(0, openBidx).Trim(), line.Substring(openBidx + 1).Replace(']', ' ').Split(delimiter));
	}

	private bool ValidateTimestamps(string[] timestamp)
	{
		string format = @"mm\:ss";

		for(int i = 0; i < timestamp.Length; i++)
		{
			string[] tempList = timestamp[i].Split('-');
			for (int j = 0; j < tempList.Length; j++)
			{
				if (!TimeSpan.TryParseExact(tempList[j].Trim(), format, CultureInfo.InvariantCulture, out TimeSpan _))
				{
					throw new Exception("One or more timestamps are invalid. Please check and try again.");
				}
			}
		}
		
		return true;
	}
}
