﻿using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    class ExcelManager
    {
        private static ExcelManager instance = new ExcelManager();

        public static ExcelManager Manager { get { return instance; } }

        private ExcelManager() { }

        public List<Dictionary<string, string>> LoadTableFromExcel(string path)
        {
            XSSFWorkbook hssfwb;
            // open the file
            using (FileStream file = new FileStream(@path, FileMode.Open, FileAccess.Read))
            {
                // open the file with XSSF
                hssfwb = new XSSFWorkbook(file);
            }
            // create formating objects to read the columns
            DataFormatter objDefaultFormat = new DataFormatter();
            XSSFFormulaEvaluator objFormulaEvaluator = new XSSFFormulaEvaluator((XSSFWorkbook)hssfwb);
            // get the sheet at the first position
            ISheet sheet = hssfwb.GetSheetAt(0);
            Dictionary<string, int> tableStart = FindTableCoordinates(sheet);
            return GetTable(sheet, tableStart["rowBegin"], tableStart["colBegin"], objDefaultFormat, objFormulaEvaluator);
        }

        private List<Dictionary<string, string>> GetTable(ISheet sheet, int rowBegin, int colBegin, DataFormatter objDefaultFormat, XSSFFormulaEvaluator objFormulaEvaluator)
        {
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            string[] colNames = new string[sheet.GetRow(rowBegin).LastCellNum - colBegin];
            // get the names of the columns
            for (int i = colBegin; i < colNames.Length + colBegin; i++)
            {
                if (sheet.GetRow(rowBegin).GetCell(i) != null)
                {
                    var coll = sheet.GetRow(rowBegin).GetCell(i);
                    objFormulaEvaluator.Evaluate(coll);
                    String cellValueStr = objDefaultFormat.FormatCellValue(coll, objFormulaEvaluator);
                    colNames[i] = cellValueStr;
                }
                else { return null; }
            }
            // get the data rows from the sheet
            for (int row = rowBegin + 1; row <= sheet.LastRowNum; row++)
            {
                Dictionary<string, string> tableRow = new Dictionary<string, string>();
                if (sheet.GetRow(row) == null)
                    // empty row
                    continue;
                for (int col = colBegin; col < colNames.Length + colBegin; col++)
                {
                    // read the column value
                    var coll = sheet.GetRow(row).GetCell(col);
                    objFormulaEvaluator.Evaluate(coll);
                    String cellValueStr = objDefaultFormat.FormatCellValue(coll, objFormulaEvaluator);
                    tableRow[colNames[col - colBegin]] = cellValueStr;
                }
                table.Add(tableRow);
            }

            return table;
        }

        private Dictionary<string, int> FindTableCoordinates(ISheet sheet)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            int rowBegin = 0, colBegin = 0;
            int maxRoxLength = 0;
            // attempt to find the starting row of the largest table, and the starting column
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null)
                {
                    if (sheet.GetRow(row).LastCellNum > maxRoxLength)
                    {
                        // found a new max
                        maxRoxLength = sheet.GetRow(row).LastCellNum;
                        rowBegin = row;
                        for (int col = 0; col <= sheet.GetRow(row).LastCellNum; col++)
                        {
                            if (sheet.GetRow(row).GetCell(col) != null
                                && sheet.GetRow(row).GetCell(col).CellType != CellType.Blank)
                            {
                                colBegin = col;
                                break;
                            }
                        }
                    }
                }
            }
            dict["rowBegin"] = rowBegin;
            dict["colBegin"] = colBegin;
            return dict;
        }
    }
}
