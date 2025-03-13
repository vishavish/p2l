using System.Diagnostics;
using System.Text;


void CutVideo(string input, string[] stamps)
{
	StringBuilder sb = new StringBuilder();
	sb.Append($"-c \"ffmpeg -i {input} -c:a copy -c:v libx264 -crf 18 ");
	for (int i = 0; i < stamps.Length; i++)
	{
		sb.Append($"-ss { stamps[i].Split('-')[0] } -to { stamps[i].Split('-')[1] } output{ i }.mp4 ");
	}
	sb.Append("-metadata title -hide_banner \"");

	ProcessStartInfo psi = new ProcessStartInfo()
	{
		FileName = "/bin/bash",
		Arguments = sb.ToString()	
	};

	Process.Start(psi);
}

(string, string[]) GetLineInfo(string line)
{
	char delimiter = ',';
	int openBidx = line.IndexOf('[');
	string input = line.Substring(0, openBidx);
	return (input, line.Substring(openBidx + 1).Replace(']', ' ').Split(delimiter));
}


List<Task> tasks = new();
Dictionary<string, string[]> fileInfo = new();
const string FILENAME = "Timestamp.txt";
string[] lines = File.ReadAllLines(FILENAME);

// Console.WriteLine(Environment.CurrentDirectory);
// Console.ReadLine();

for(int i = 0; i < lines.Length; i++)
{
	(string name, string[] timestamps) = GetLineInfo(lines[i]);
	fileInfo.Add(name, timestamps);
}

foreach (KeyValuePair<string, string[]> kv in fileInfo)
{
	tasks.Add(Task.Run(() => CutVideo(kv.Key, kv.Value)));
}

await Task.WhenAll(tasks);

Console.WriteLine("------SUMMARY-------");
Console.WriteLine("Press any key to close . . .");

Console.ReadKey();

