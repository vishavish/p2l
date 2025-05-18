using System.ComponentModel;
using Spectre.Console.Cli;


var app = new CommandApp<SplitCommand>();
app.Run(args);


internal sealed class SplitCommand : Command<SplitCommand.Settings>
{
	public sealed class Settings : CommandSettings
	{
		[Description("Path to search for the timestamps.")]
		[CommandArgument(0,"[dirPath]")]
		public string? DirectoryPath { get; init; }
	}

	public override int Execute(CommandContext context, Settings settings)
	{
		App app = new();
		app.Init(settings.DirectoryPath!);

		return 0;
	}
}
