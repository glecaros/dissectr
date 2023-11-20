using System;
using System.Collections.Generic;
using System.Linq;
using Dissectr.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Dissectr.Util;
public class Exporter
{
    private static Dictionary<Guid, int> GenerateMapping(Project project)
    {
        var mapping = new Dictionary<Guid, int>();
        int i = 0;
        foreach (var dimension in project.Dimensions.OrderBy(d => d.Order))
        {
            mapping.Add(dimension.Id, i++);
        }
        return mapping;
    }

    private static Row GenerateHeader(Project project, Dictionary<Guid, int> mapping)
    {
        Row row = new();
        row.Append(new Cell(new CellValue("Start")));
        row.Append(new Cell(new CellValue("Transcription")));
        foreach (var dimension in project.Dimensions.OrderBy(d => mapping[d.Id]))
        {
            row.Append(new Cell(new CellValue($"{dimension.Name} (Code)")));
            row.Append(new Cell(new CellValue(dimension.Name)));
        }
        return row;
    }

    private static Row GenerateRow(IntervalEntry entry, Dictionary<Guid, int> mapping)
    {
        Row row = new();
        row.Append(new Cell(new CellValue(entry.Start.ToString(@"hh\:mm\:ss"))));
        row.Append(new Cell(new CellValue(entry.Transcription ?? string.Empty)));
        foreach (var dimension in entry.Dimensions.OrderBy(d => mapping[d.Dimension.Id]))
        {
            var selection = dimension.Selection;
            if (selection is null)
            {
                row.Append(new Cell(new CellValue()));
                row.Append(new Cell(new CellValue()));
            }
            else
            {
                row.Append(new Cell(new CellValue(selection.Code)));
                row.Append(new Cell(new CellValue(selection.Name)));
            }
        }
        return row;
    }

    private static Worksheet GenerateDataSheet(Project project, IEnumerable<IntervalEntry> entries)
    {
        var sheetData = new SheetData();
        var mapping = GenerateMapping(project);
        var headerRow = GenerateHeader(project, mapping);
        sheetData.Append(headerRow);
        foreach (var entry in entries)
        {
            var row = GenerateRow(entry, mapping);
            sheetData.Append(row);
        }
        return new Worksheet(sheetData);
    }

    public static void ExportToXLS(Project project, IEnumerable<IntervalEntry> entries, string path)
    {
        using var spreadsheetDocument = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
        var workbook = spreadsheetDocument.AddWorkbookPart();
        workbook.Workbook = new Workbook();

        var sheets = workbook.Workbook.AppendChild(new Sheets());

        var dataWorksheetPart = workbook.AddNewPart<WorksheetPart>();
        dataWorksheetPart.Worksheet = GenerateDataSheet(project, entries);

        var dataSheet = new Sheet() { Id = workbook.GetIdOfPart(dataWorksheetPart), SheetId = 1, Name = "Data" };
        sheets.Append(dataSheet);
    }
}

