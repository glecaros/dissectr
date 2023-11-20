using System;
using System.Collections.Generic;
using System.IO;
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
        row.Append(CreateTextCell("Start"));
        row.Append(CreateTextCell("Transcription"));
        foreach (var dimension in project.Dimensions.OrderBy(d => mapping[d.Id]))
        {
            row.Append(CreateTextCell($"{dimension.Name} (Code)"));
            row.Append(CreateTextCell(dimension.Name));
        }
        return row;
    }

    private static Cell CreateTextCell(string text)
    {
        var cell = new Cell();
        cell.DataType = CellValues.InlineString;
        cell.InlineString = new InlineString(new Text(text));
        return cell;

    }

    private static Row GenerateRow(IntervalEntry entry, Dictionary<Guid, int> mapping)
    {
        Row row = new();
        row.Append(CreateTextCell(entry.Start.ToString(@"hh\:mm\:ss")));
        row.Append(CreateTextCell(entry.Transcription ?? string.Empty));
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
                row.Append(CreateTextCell(selection.Name));
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

    public static void ExportToXLS(Project project, IEnumerable<IntervalEntry> entries, Stream stream)
    {
        using var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
        var workbookPart = spreadsheetDocument.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        var dataWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        dataWorksheetPart.Worksheet = GenerateDataSheet(project, entries);

        var sheets = workbookPart.Workbook.AppendChild(new Sheets());

        var dataSheet = new Sheet()
        {
            Id = workbookPart.GetIdOfPart(dataWorksheetPart),
            SheetId = 1,
            Name = "Data",
        };
        sheets.Append(dataSheet);

        workbookPart.Workbook.Save();
    }
}

