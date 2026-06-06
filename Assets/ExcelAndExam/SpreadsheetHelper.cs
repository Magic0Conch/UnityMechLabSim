using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using UnityEngine;

/// <summary>
/// 统一加载 .xls / .xlsx / .csv 为 NPOI 工作表。
/// </summary>
public static class SpreadsheetHelper
{
    static readonly string[] ExamExtensions = { ".xlsx", ".xls", ".csv" };

    public static ISheet LoadFirstSheet(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            throw new FileNotFoundException("表格文件不存在", filePath);

        string ext = Path.GetExtension(filePath).ToLowerInvariant();
        if (ext == ".csv")
            return CsvToSheet(filePath);

        IWorkbook workbook = WorkbookFactory.Create(filePath);
        return workbook.GetSheetAt(0);
    }

    /// <summary>
    /// 在 StreamingAssets 下按 Test1 / Test2 等基名查找已下载题库（优先 xlsx > xls > csv）。
    /// </summary>
    public static string ResolveStreamingAsset(string baseName)
    {
        string dir = Application.streamingAssetsPath;
        foreach (string ext in ExamExtensions)
        {
            string path = Path.Combine(dir, baseName + ext);
            if (File.Exists(path))
                return path;
        }
        return Path.Combine(dir, baseName + ".xls");
    }

    public static bool StreamingAssetExists(string baseName)
    {
        string dir = Application.streamingAssetsPath;
        foreach (string ext in ExamExtensions)
        {
            if (File.Exists(Path.Combine(dir, baseName + ext)))
                return true;
        }
        return false;
    }

    public static void DeleteStreamingAssetVariants(string baseName)
    {
        string dir = Application.streamingAssetsPath;
        foreach (string ext in ExamExtensions)
        {
            string path = Path.Combine(dir, baseName + ext);
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    static ISheet CsvToSheet(string filePath)
    {
        string text = ReadAllTextAuto(filePath);
        var workbook = new HSSFWorkbook();
        ISheet sheet = workbook.CreateSheet("Sheet1");
        int rowIndex = 0;
        foreach (string line in SplitCsvLines(text))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            string[] fields = ParseCsvLine(line);
            IRow row = sheet.CreateRow(rowIndex++);
            for (int c = 0; c < fields.Length; c++)
                row.CreateCell(c).SetCellValue(fields[c]);
        }
        return sheet;
    }

    static IEnumerable<string> SplitCsvLines(string text)
    {
        using (var reader = new StringReader(text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }
    }

    static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;
        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }
            if (ch == ',' && !inQuotes)
            {
                fields.Add(current.ToString());
                current.Length = 0;
                continue;
            }
            current.Append(ch);
        }
        fields.Add(current.ToString());
        return fields.ToArray();
    }

    static string ReadAllTextAuto(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

        string utf8 = Encoding.UTF8.GetString(bytes);
        if (utf8.IndexOf('\uFFFD') < 0)
            return utf8;

        try
        {
            return Encoding.GetEncoding(936).GetString(bytes);
        }
        catch
        {
            return utf8;
        }
    }
}
