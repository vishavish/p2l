if (args.Length > 0)
{
	switch(args[0])
	{
		case "-help":
		case "--h"  :
			ShowUsage();
			break;
		default:
			new App().Init(args[0]);
			break;
	}
} 
else 
	ShowUsage();
	

void ShowUsage()
{
	Console.WriteLine($"USAGE: p2l.dll  <dirpath> [OPTION]\n");
	Console.WriteLine("<dirPath>       Path to search for the Timestamp.txt\n");
	Console.WriteLine("[OPTION]");
	Console.WriteLine("-help | --h     Print the help info.");
}
