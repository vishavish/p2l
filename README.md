# p2l

**p2l** (`putol`; in english `cut`) is a CLI-tool made with .NET to cut video clips using [FFmpeg](https://ffmpeg.org/).

## Purpose

I play games a lot and have lots of clips. I needed a way to cut them without using a video editor.

## Building

**NOTE**: This project requires the [.NET SDK](https://dotnet.microsoft.com/en-us/download).

1. Clone the repository:
```
git clone https://github.com/vishavish/p2l.git
```

2. Use **dotnet run** to run the project:
```
cd p2l
dotnet run --project ./p2l/p2l.csproj <path to Timestamp.txt>
```

## Usage

**p2l** uses [FFmpeg](https://ffmpeg.org/download.html) in the backend.

To use the tool, it needs a path to **Timestamps.txt** file. Below is a sample file:

```
input.mp4 [00:30-00:40,03:30-03:42,06:00-06:25]
````
