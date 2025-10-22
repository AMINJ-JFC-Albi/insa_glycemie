using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tools;
using UnityEngine;

public class DataCollectorManager {
    private List<DataEntry> dataEntries = new();
    private List<DataGhostEntry> dataGhostEntries = new();
    private DateTime startTime;
    private readonly char csvSeparator;
    private readonly String csvFirstLine;
    private readonly String fileDateTime;

    public DataCollectorManager(String csvFirstLine = "Name,Value,Timestamp", char csvSeparator = ',') {
        this.csvSeparator = csvSeparator;
        this.csvFirstLine = csvFirstLine;
        fileDateTime = $"{DateTime.Now:yyyy-MM-dd_HH-mm}";
    }

    [Serializable]
    private class DataEntry {
        public string ID;
        public string Type;
        public object Value;
        public DateTime Timestamp;

        public DataEntry(string id, string type, object value) {
            ID = id;
            Type = type;
            Value = value;
            Timestamp = DateTime.Now;
        }
    }

    private class DataGhostEntry {
        public string ID;
        public DateTime Timestamp;

        public DataGhostEntry(string id) {
            ID = id;
            Timestamp = DateTime.Now;
        }
    }

    // Initialize the start time
    public void InitializeStartTime() {
        startTime = DateTime.Now;
#if UNITY_EDITOR
        LoggerTool.Log("Start time initialized: " + startTime);
#endif
    }

    // Add a new data ghost entry
    public void AddGhostData(string id) {
        var entry = dataGhostEntries.Find(e => e.ID == id);
        if (entry == null) {
            entry = new DataGhostEntry(id);
            dataGhostEntries.Add(entry);
#if UNITY_EDITOR
            LoggerTool.Log($"Data Ghost added: ID={id}, Time={entry.Timestamp}");
#endif
        }
#if UNITY_EDITOR
        else {
            LoggerTool.Log($"Data ID={id} already exist!");
        }
#endif
    }

    // Edit an existing data ghost entry by id
    public void EditGhostData(string id) {
        var entry = dataEntries.Find(e => e.ID == id);
        if (entry != null) {
            entry.Timestamp = DateTime.Now;
#if UNITY_EDITOR
            LoggerTool.Log($"Data Ghost edited: ID={id}, New Time={entry.Timestamp}");
#endif
        } else {
            AddGhostData(id);
        }
    }

    // Add a new data entry
    public void AddData(string id, string type, object value) {
        var entry = dataEntries.Find(e => e.ID == id);
        if (entry == null) {
            entry = new DataEntry(id, type, value);
            dataEntries.Add(entry);
#if UNITY_EDITOR
            LoggerTool.Log($"Data added: ID={id}, Type={type}, Value={value}, Time={entry.Timestamp}");
#endif
        }
#if UNITY_EDITOR
        else {
            LoggerTool.Log($"Data ID={id} already exist!");
        }
#endif
    }

    // Edit an existing data entry by id
    public void EditData(string id, string type, object newValue) {
        var entry = dataEntries.Find(e => e.ID == id);
        if (entry != null) {
            entry.Value = newValue;
            entry.Timestamp = DateTime.Now;
#if UNITY_EDITOR
            LoggerTool.Log($"Data edited: ID={id}, Type={type}, New Value={newValue}, New Time={entry.Timestamp}");
#endif
        } else {
            AddData(id, type, newValue);
        }
    }

    // Calculate the time difference between two data entries by id
    public TimeSpan CalculateTimeDifference(string id1, string id2) {
        var entry1 = dataEntries.Find(e => e.ID == id1);
        var entry2 = dataEntries.Find(e => e.ID == id2);

        if (entry1 != null && entry2 != null) {
            return entry2.Timestamp - entry1.Timestamp;
        } else {
#if UNITY_EDITOR
            LoggerTool.Log("One or both data entries not found.", LoggerTool.Level.Warning);
#endif
            return TimeSpan.Zero;
        }
    }

    // Calculate the time since a data entry
    public TimeSpan CalculateSinceEntry(string id) {
        var entry = dataGhostEntries.Find(e => e.ID == id);

        if (entry != null) {
            return DateTime.Now - entry.Timestamp;
        } else {
#if UNITY_EDITOR
            LoggerTool.Log("Data entry not found.", LoggerTool.Level.Warning);
#endif
            return TimeSpan.Zero;
        }
    }

    // Calculate the total time
    public TimeSpan CalculateTotalGhostTime(string id) {
        var entry = dataGhostEntries.Find(e => e.ID == id);

        if (entry != null) {
            return entry.Timestamp - startTime;
        } else {
#if UNITY_EDITOR
            LoggerTool.Log("Data entry not found.", LoggerTool.Level.Warning);
#endif
            return TimeSpan.Zero;
        }
    }

    // Calculate the total time
    public TimeSpan CalculateTotalTime(string id) {
        var entry = dataEntries.Find(e => e.ID == id);

        if (entry != null) {
            return entry.Timestamp - startTime;
        } else {
#if UNITY_EDITOR
            LoggerTool.Log("Data entry not found.", LoggerTool.Level.Warning);
#endif
            return TimeSpan.Zero;
        }
    }

    // Save data entries to a file depending on the platform
    public void SaveData(bool avoidCommasIssues = true) {
        string jsonFilePath = GetDocumentsPath($"data_entries_{fileDateTime}.json");
        string csvFilePath = GetDocumentsPath($"data_entries_{fileDateTime}.csv");

        try {
            // Save JSON
            string json = JsonConvert.SerializeObject(dataEntries, Formatting.Indented);
            File.WriteAllText(jsonFilePath, json);
#if UNITY_EDITOR
            Debug.Log("Data saved to: " + jsonFilePath);
#endif

            // Save CSV
            StringBuilder csvContent = new();
            csvContent.AppendLine(csvFirstLine);
            foreach (var entry in dataEntries) {
                string valueString = entry.Value.ToString();
                if (avoidCommasIssues) valueString.Replace(csvSeparator, ' '); // Avoid issues with commas in CSV
                csvContent.AppendLine($"{entry.Type}{csvSeparator}{valueString}");
            }
            File.WriteAllText(csvFilePath, csvContent.ToString());
#if UNITY_EDITOR
            Debug.Log("Data saved to: " + csvFilePath);
#endif
        }
        catch (Exception e) {
            Debug.LogError("Error saving data: " + e.Message);
        }
    }

    // Load data entries from a file
    public void LoadData() {
        string filePath = GetDocumentsPath("data_entries.json");

        try {
            if (File.Exists(filePath)) {
                string json = File.ReadAllText(filePath);
                SerializableDataList dataList = JsonConvert.DeserializeObject<SerializableDataList>(json);
                dataEntries = dataList.Entries;
#if UNITY_EDITOR
                LoggerTool.Log("Data loaded from: " + filePath);
#endif
            }
#if UNITY_EDITOR
            else {
                LoggerTool.Log("No data file found at: " + filePath, LoggerTool.Level.Warning);
            }
#endif
        }
        catch (Exception e) {
            LoggerTool.Log("Error loading data: " + e.Message, LoggerTool.Level.Error);
        }
    }

    // Get the documents path based on the platform
    private string GetDocumentsPath(string fileName) {
        string folderPath = "";

#if UNITY_ANDROID || UNITY_IOS || UNITY_METAQUEST
        folderPath = Application.persistentDataPath;
#elif UNITY_STANDALONE_WIN
        folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyGameData");
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".mygamedata");
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Documents/MyGameData");
#elif UNITY_EDITOR_WIN
            folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EditorGameData");
#elif UNITY_WSA
            folderPath = Application.persistentDataPath;
#else
            Debug.LogWarning("Unsupported platform for documents path.");
#endif

        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }

        return Path.Combine(folderPath, fileName);
    }

    [Serializable]
    private class SerializableDataList {
        public List<DataEntry> Entries;
    }
}
