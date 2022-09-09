using System.Configuration;
using AngryFileSorter;

PrintHelper.Print("Запуск...", ConsoleColor.Yellow);

var fromDirectory = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings.Get("AutoScanFolderPath"))
    ? throw new ArgumentException("Ошибка папки сбора изображений")
    : ConfigurationManager.AppSettings.Get("AutoScanFolderPath");

var files = Directory.GetFiles(fromDirectory!, "*.*", SearchOption.AllDirectories);

var processingImageFormats = ConfigurationManager.AppSettings.Get("ProcessingFileFormats")
    ?.Split(",")
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Select(x => x.Replace(" ", ""))
    .ToArray();

if (processingImageFormats != null && processingImageFormats.Length != 0)
{
    files = files
        .Where(x => processingImageFormats.Contains(Path.GetExtension(x).ToLower()))
        .ToArray();
}
else
{
    PrintHelper.Print("Не указаны форматы изображений для обработки. Будут обработаны файлы всех форматов.",
        ConsoleColor.Yellow);
}

PrintHelper.Print($"Надено файлов для обработки = {files.Length}", ConsoleColor.Yellow);

var path = ConfigurationManager.AppSettings.Get("ResultFolderPath");

if (string.IsNullOrWhiteSpace(path))
{
    path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
}

var newFolderName = Guid.NewGuid().ToString().Replace("-", "");
var newFolderFullPath = Path.Combine(path, newFolderName);

Directory.CreateDirectory(newFolderFullPath);

var needToDeleteSourceFile = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("NeedToDeleteSourceFile"));
MoveFiles(files, newFolderFullPath, needToDeleteSourceFile);

void MoveFiles(string[] selectedFiles, string resultFolderFullPath, bool needToDeleteSource)
{
    var rangeToProcess = Convert.ToInt32(ConfigurationManager.AppSettings.Get("FolderItemsMaxLength"));
    var iterator = 1;

    while (selectedFiles.Length > 0)
    {
        var processingFiles = selectedFiles.Take(rangeToProcess).ToArray();

        foreach (var processingFile in processingFiles)
        {
            var newPath = Path.Combine(resultFolderFullPath, iterator.ToString());
            Directory.CreateDirectory(newPath);
            var fileName = Path.GetFileName(processingFile);
            var resultFile = Path.Combine(newPath, fileName);

            if (needToDeleteSource)
            {
                File.Move(processingFile, resultFile);
            }
            else
            {
                File.Copy(processingFile, resultFile);
            }

            PrintHelper.Print($"Перемещен файл {processingFile} >>> {resultFile}", ConsoleColor.DarkCyan);
        }

        selectedFiles = selectedFiles.Except(processingFiles).ToArray();
        iterator++;
    }
}