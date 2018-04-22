namespace ShelfsetChangeReporter
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using FluentDateTime;

    public class Program
    {
        private static HttpClient _httpClient;
        private static string _logFilePath;
        private static string _baseUrl;
        private static string _sitePrefix;
        private static string[] _projectCollectionNames;
        private static string _workspaceOwner;
        private static readonly object _compareProcessingTimeLock = new object();
        private static TimeSpan _compareProcessingTime = TimeSpan.Zero;
        private static readonly object _networkTimeLock = new object();
        private static TimeSpan _networkTime = TimeSpan.Zero;
        private static readonly Stopwatch _overallTime = new Stopwatch();
        private static readonly ConcurrentQueue<string> _owlQueue = new ConcurrentQueue<string>();
        private static bool _isRunning;
        private static readonly ConcurrentQueue<string> _tempFilePool = new ConcurrentQueue<string>();
        private static DateTime _shelvesetStartWindow; //= DateTime.Now.Previous(DayOfWeek.Monday).BeginningOfDay();
        private static DateTime _shelvesetEndWindow;// = DateTime.Now.Previous(DayOfWeek.Friday).EndOfDay();

        // ReSharper disable UnusedParameter.Local
        static void Main(string[] args)
        // ReSharper restore UnusedParameter.Local
        {
            Configure();

            _overallTime.Start();
            _isRunning = true;

            var outputsForThisRunFolder = $"SCROutputs\\{DateTime.Now.ToFileTime()}";
            Directory.CreateDirectory(outputsForThisRunFolder);

            var logFileName = "_ShelvesetChangeReporter.log";
            _logFilePath = Path.Combine(outputsForThisRunFolder, logFileName);

            var collectedEventsFilePath = Path.Combine(outputsForThisRunFolder, "_CollectedEvents.json");

            var owlWorker = new Thread(OwlWorker);
            owlWorker.Name = nameof(owlWorker);
            owlWorker.Start();

            Owl("Preallocating temp files...");
            var tmpFileSw = Stopwatch.StartNew();
            for (var i = 0; i < 200; i++)
            {
                _tempFilePool.Enqueue(Path.GetTempFileName());
            }

            tmpFileSw.Stop();
            Owl($"Temp files created om {tmpFileSw.Elapsed}, getting shelvesets of {_workspaceOwner} ");
            var collectedDiffs = new List<CollectedDiff>();

            var handler = new HttpClientHandler { UseDefaultCredentials = true };
            _httpClient = new HttpClient(handler) { BaseAddress = new Uri(_baseUrl) };

            foreach (var projectCollection in _projectCollectionNames)
            {
                var shelfSetList = GetTargetShelfSetList(projectCollection, _workspaceOwner);

                Owl($"Got {shelfSetList.Count} shelf sets from {projectCollection}...");
                var shelvedChanges = new List<ShelvedChangeset>();

                var current = 0;
                foreach (var shelfSet in shelfSetList)
                {
                    current++;
                    Owl($"Fetching details for shelfset {projectCollection}\\{shelfSet.Id}, {current} of {shelfSetList.Count}");
                    var detailUrl = $"{_sitePrefix}{projectCollection}/_apis/tfvc/shelvesets/changes?shelvesetId={shelfSet.Id}";
                    var shelfSetDetailResponse = GetRestResult<ShelfSetChangesResponse>(detailUrl);
                    shelvedChanges.Add(new ShelvedChangeset
                    {
                        ProjectCollectionName = projectCollection,
                        ShelfSet = shelfSet,
                        Changes = shelfSetDetailResponse.Value
                    });
                }

                for (var i = 1; i < shelvedChanges.Count; i++)
                {
                    var diffsForShelfSet = new ConcurrentBag<Difference>();
                    var shelvedChangesB = shelvedChanges[i - 1];
                    var shelvedChangesA = shelvedChanges[i];
                    Owl($"Comparing prior: {shelvedChangesB.ShelfSet.Name} to {shelvedChangesA.ShelfSet.Name}");

                    Parallel.ForEach(shelvedChangesA.Changes, shelfSetChange =>
                    {
                        if (Path.HasExtension(shelfSetChange.Item.Path) == false)
                        {
                            Owl($"No file extension found for path {shelfSetChange.Item.Path}'. Probably a folder, not comparing folders");
                            return;
                        }

                        Owl($"\t Inspecting file at path {shelfSetChange.Item.Path}");
                        var diffs = InspectShelfSetChange(shelfSetChange, shelvedChangesB);
                        diffs.ForEach(diffsForShelfSet.Add);
                    });

                    Parallel.ForEach(shelvedChangesB.Changes, shelfSetChange =>
                    {
                        if (Path.HasExtension(shelfSetChange.Item.Path) == false)
                        {
                            Owl($"No file extension found for path {shelfSetChange.Item.Path}'. Probably a folder, not comparing folders");
                            return;
                        }

                        Owl($"\t Inspecting file at path {shelfSetChange.Item.Path}");
                        var diffs = InspectShelfSetChange(shelfSetChange, shelvedChangesA);
                        diffs.ForEach(diffsForShelfSet.Add);
                    });

                    collectedDiffs.Add(new CollectedDiff
                    {
                        ProjectCollectionName = projectCollection,
                        ShelfSetA = shelvedChangesA.ShelfSet,
                        ShelfSetB = shelvedChangesB.ShelfSet,
                        Diffs = diffsForShelfSet.ToList()
                    });
                }
            } //foreach projectCollection

            var collectedEvents = new List<WorkEvent>();
            foreach (var collectedDiff in collectedDiffs)
            {
                Owl($"Reporting diffs for shelf set {collectedDiff.ProjectCollectionName}\\{collectedDiff.ShelfSetA.Name}, created on {collectedDiff.ShelfSetA.CreatedDate} ...");
                var filePath = Path.Combine(outputsForThisRunFolder, $"{collectedDiff.ProjectCollectionName}_{collectedDiff.ShelfSetA.CreatedDate.ToFileTime()}_{collectedDiff.ShelfSetA.Id}.json");
                File.WriteAllText(filePath, JsonConvert.SerializeObject(collectedDiff, Formatting.Indented));

                if (collectedDiff.Diffs.Count == 0)
                {
                    Owl("No diffs...");
                    continue;
                }

                Owl("\n\tShelfset had differences...");
                var paths = collectedDiff.Diffs.Select(d => d.Path).Distinct().OrderBy(d => d).ToList();
                var eventMessage = new StringBuilder();
                eventMessage.AppendLine("Coding in progress detected for files (Line,Path)...");

                foreach (var path in paths)
                {
                    Owl($"Path '{path}'");
                    var diffsForPath = collectedDiff.Diffs.Where(d => d.Path.Equals(path, StringComparison.OrdinalIgnoreCase)).ToList();
                    var nonFileDiffs = diffsForPath.Where(d => d.DiffType != DiffType.Changed).OrderBy(d => d.DiffType);
                    foreach (var nfd in nonFileDiffs)
                    {
                        Owl($"non file diff {nfd.DiffType}");
                    }

                    var distinctLineNumbers = diffsForPath.Where(d => d.DiffType == DiffType.Changed).Select(d => d.LineNumber).Distinct().OrderBy(l => l).ToList();
                    foreach (var lineNumber in distinctLineNumbers)
                    {
                        var diff = diffsForPath.First(d => d.DiffType == DiffType.Changed && d.LineNumber == lineNumber);
                        Owl($"\t\tLine {diff.LineNumber} \n\t\t\t{diff.OlderLine}\n\t\t\t{diff.NewerLine}\n");
                    }
                    eventMessage.AppendLine($" - ({string.Join(",", distinctLineNumbers)}) - {path}");
                }
                collectedEvents.Add(new WorkEvent
                {
                    EventTime = collectedDiff.ShelfSetA.CreatedDate,
                    Event = eventMessage.ToString()
                });
            }

            File.WriteAllText(collectedEventsFilePath, JsonConvert.SerializeObject(collectedEvents.OrderByDescending(e => e.EventTime), Formatting.Indented));

            _overallTime.Stop();

            // ReSharper disable InconsistentlySynchronizedField
            Owl($"FINAL: Overall time: {_overallTime.Elapsed}, Network time: {_networkTime}, Compare processing time: {_compareProcessingTime}");
            // ReSharper restore InconsistentlySynchronizedField
            Owl("DONE. press enter to close");
            _isRunning = false;
            owlWorker.Join(1 * 1000);
            Console.ReadLine();
        }

        private static void Configure()
        {
            _baseUrl = ConfigurationManager.AppSettings["baseUrl"];
            _sitePrefix = ConfigurationManager.AppSettings["sitePrefix"];
            _projectCollectionNames = ConfigurationManager.AppSettings["projectCollectionNames"].Split(',').Select(s => s.Trim()).ToArray();
            _workspaceOwner = ConfigurationManager.AppSettings["lastCommaFirstOfUserName"];
            var inclusionStartDateTime = ConfigurationManager.AppSettings["inclusionStartDateTime"];
            var inclusionEndDateTime = ConfigurationManager.AppSettings["inclusionEndDateTime"];

            DateTime dummeh;
            if (DateTime.TryParse(inclusionStartDateTime, out dummeh))
            {
                _shelvesetStartWindow = dummeh;
            }
            else
            {
                _shelvesetStartWindow = DateTime.Now.Previous(DayOfWeek.Monday).BeginningOfDay();
            }

            if (DateTime.TryParse(inclusionEndDateTime, out dummeh))
            {
                _shelvesetEndWindow = dummeh;
            }
            else
            {
                _shelvesetEndWindow = DateTime.Now.Previous(DayOfWeek.Friday).EndOfDay();
            }

        }

        private static List<ShelfSet> GetTargetShelfSetList(string projectCollection, string workspaceOwner)
        {
            var shelfSetList = new List<ShelfSet>();
            var startTimeEclipsed = false;
            var currentTop = 100;
            var currentSkip = 0;
            while (startTimeEclipsed == false)
            {
                var ownerShelvesetsUrlParameters = $"{_sitePrefix}/{projectCollection}/_apis/tfvc/shelvesets?requestData.owner={workspaceOwner}&$top={currentTop}&$skip={currentSkip}";
                var shelfSetResponse = GetRestResult<ShelfSetResponse>(ownerShelvesetsUrlParameters);
                var shelfSetBlock = shelfSetResponse.Value;

                foreach (var ss in shelfSetBlock)
                {
                    if (ss.CreatedDate < _shelvesetStartWindow)
                    {
                        startTimeEclipsed = true;
                        break;
                    }

                    if (ss.CreatedDate > _shelvesetEndWindow)
                    {
                        continue;
                    }

                    shelfSetList.Add(ss);
                }

                currentSkip = currentTop;
                currentTop = currentTop + currentTop;
            }

            return shelfSetList;
        }

        private static List<Difference> InspectShelfSetChange(ShelfSetChange changeA, ShelvedChangeset setBChanges)
        {
            var setBMatches = setBChanges.Changes.Where(c => c.Item.Path.Equals(changeA.Item.Path, StringComparison.OrdinalIgnoreCase)).ToList();
            if (setBMatches.Count > 1)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"\tFile {changeA.Item.Path} has more than one match in the B shelfset. Skipping...");
                foreach (var sbm in setBMatches)
                {
                    sb.AppendLine($"\t{sbm.ChangeType} - {sbm.Item.Path} \n\t\t {sbm.Item.HashValue} \t {sbm.Item.Version} \n\t\t {sbm.Item.Url}");
                }

                Owl(sb.ToString());
                return Enumerable.Empty<Difference>().ToList();
            }

            if (setBMatches.Count == 0)
            {
                return new List<Difference> { new Difference { DiffType = DiffType.Add, Path = changeA.Item.Path, Set = "A" } };
            }

            var changeB = setBMatches.First();

            try
            {
                var contentDiffs = GetShelfSetChangeDifferences(changeA, changeB);
                if (contentDiffs.Count > 0)
                {
                    return contentDiffs;
                }
            }
            catch (Exception e)
            {
                Owl($"ERROR: failed diffing files {changeA.Item.Path} and {changeB.Item.Path}. \n\t {e}\n");
            }

            return Enumerable.Empty<Difference>().ToList();
        }

        private static List<Difference> GetShelfSetChangeDifferences(ShelfSetChange setAChange, ShelfSetChange setBChange)
        {
            var result = new List<Difference>();
            var tempFile1 = GetTempFile();
            var tempFile2 = GetTempFile();
            var methodSw = Stopwatch.StartNew();
            var sb = new StringBuilder();
            try
            {
                var file1Get = Stopwatch.StartNew();
                var setAContent = DownloadToTempFile(setAChange, tempFile1);
                file1Get.Stop();
                sb.AppendLine($"\tGot file {setAChange.Item.Path} to {tempFile1} in {file1Get.Elapsed}");

                var file2Get = Stopwatch.StartNew();
                var setBContent = DownloadToTempFile(setBChange, tempFile2);
                file2Get.Stop();
                sb.AppendLine($"\tGot file {setAChange.Item.Path} to {tempFile2} in {file2Get.Elapsed}");

                sb.AppendLine($"\tComparing content for {setAChange.Item.Path}");

                var maxLines = Math.Max(setAContent.Length, setBContent.Length);
                sb.AppendLine($"\tMax Lines is {maxLines}");
                for (var i = 0; i < maxLines; i++)
                {
                    if (i > setAContent.Length)
                    {
                        sb.AppendLine("\tShelved item from set A has less lines");
                        result.Add(new Difference { DiffType = DiffType.MissingContent, LineNumber = i, Path = setAChange.Item.Path, Set = "A" });
                        break;
                    }

                    if (i > setBContent.Length)
                    {
                        sb.AppendLine("\tShelved item from set B has less lines");
                        result.Add(new Difference { DiffType = DiffType.MissingContent, LineNumber = i, Path = setBChange.Item.Path, Set = "B" });
                        break;
                    }

                    var setALine = setAContent[i];
                    var setBLine = setBContent[i];
                    if (string.Equals(setALine, setBLine))
                    {
                        continue;
                    }

                    result.Add(new Difference
                    {
                        LineNumber = i,
                        NewerLine = setALine,
                        OlderLine = setBLine,
                        DiffType = DiffType.Changed,
                        Path = setAChange.Item.Path
                    });
                    sb.AppendLine($"\t First diff found on line {i}, stopping more comparisons for this file");
                    break;
                }

                methodSw.Stop();
                lock (_compareProcessingTimeLock)
                {
                    _compareProcessingTime = _compareProcessingTime.Add(methodSw.Elapsed);
                }

                Owl(sb.ToString());
            }
            catch (Exception e)
            {
                Owl(sb.ToString());
                Owl($"ERROR: Failed to get change differences. {e}");
            }

            ReturnTempFile(tempFile1);
            ReturnTempFile(tempFile2);

            return result;
        }

        private static string[] DownloadToTempFile(ShelfSetChange ssChange, string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var sw = Stopwatch.StartNew();
            using (var thisResponse = _httpClient.GetStreamAsync(ssChange.Item.Url).GetAwaiter().GetResult())
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                thisResponse.CopyToAsync(fs).GetAwaiter().GetResult();
            }

            sw.Stop();
            lock (_networkTimeLock)
            {
                _networkTime = _networkTime.Add(sw.Elapsed);
            }

            return File.ReadAllLines(fileName);
        }


        private static T GetRestResult<T>(string urlParameters)
        {
            var sw = Stopwatch.StartNew();
            var response = _httpClient.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseType = response.Content.ReadAsAsync<T>().GetAwaiter().GetResult();
                sw.Stop();
                lock (_networkTimeLock)
                {
                    _networkTime = _networkTime.Add(sw.Elapsed);
                }

                return responseType;
            }

            sw.Stop();
            lock (_networkTimeLock)
            {
                _networkTime = _networkTime.Add(sw.Elapsed);
            }

            throw new InvalidOperationException($"Failed to make request to url ending in {urlParameters}. {(int)response.StatusCode} - {response.ReasonPhrase}");
        }

        private static void Owl(string message)
        {
            _owlQueue.Enqueue(message);
        }

        private static void OwlWorker()
        {
            var consecutiveNoMessagesToWrite = 0;
            while (_isRunning || consecutiveNoMessagesToWrite < 50)
            {
                string message;
                if (_owlQueue.TryDequeue(out message))
                {
                    File.AppendAllText(_logFilePath, $"{DateTime.Now.ToShortTimeString()} - {message}\n");
                    if (message.StartsWith("ERROR:", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (message.StartsWith("WARN:", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }

                    Console.WriteLine(message);
                    Console.ResetColor();
                    consecutiveNoMessagesToWrite = 0;
                    Thread.Sleep(1);
                }
                else
                {
                    consecutiveNoMessagesToWrite++;
                    Thread.Sleep(10);
                }
            }
        }

        private static string GetTempFile()
        {
            string tempPath;
            if (_tempFilePool.TryDequeue(out tempPath))
            {
                return tempPath;
            }

            Owl("WARN: Had to allocate additional temp file");
            var newPath = Path.GetTempPath();
            return newPath;
        }

        private static void ReturnTempFile(string tempFilePath)
        {
            _tempFilePool.Enqueue(tempFilePath);
        }
    }
}