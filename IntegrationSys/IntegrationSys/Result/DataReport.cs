/**
 * 生成excel数据报表
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using IntegrationSys.Flow;

namespace IntegrationSys.Result
{
    public static class DataReport
    {
        const string SHEET_NAME = "Data";

        /// <summary>
        /// 将测试结果保存到excel文件中
        /// 
        /// </summary>
        public static void Save()
        {
            DateTime now = DateTime.Now;
            string hostName = Dns.GetHostName();
            string directory = CreateDirectory();
            string filename = directory + now.ToString("yyyy_MM_dd") + "_" + hostName + ".xlsx";

            FlowControl flowControl = FlowControl.Instance;

            if (File.Exists(filename))
            {
                bool conflict = false;
                FileStream fs = File.OpenRead(filename);
                IWorkbook workbook = new XSSFWorkbook(fs);
                fs.Close();

                ISheet sheet = workbook.GetSheet(SHEET_NAME);

                if (sheet != null)
                {
                    IRow row0 = sheet.GetRow(0);
                    IRow row1 = sheet.GetRow(1);
                    IRow row2 = sheet.GetRow(2);

                    if (row0 == null || row1 == null || row2 == null)
                    {
                        conflict = true;
                    }
                    else
                    {
                        int col = 2;

                        foreach (FlowItem flowItem in flowControl.FlowItemList)
                        {
                            if (!flowItem.Item.Property.Disable && !flowItem.IsAuxiliaryItem() && flowItem.SpecValueList != null)
                            {
                                ICell nameCell = row0.GetCell(col);

                                if (nameCell == null || !nameCell.StringCellValue.Equals(flowItem.Name))
                                {
                                    conflict = true;
                                    break;
                                }
                                foreach (SpecValue specValue in flowItem.SpecValueList)
                                {
                                    if (!specValue.Disable)
                                    {
                                        ICell specDescripCell = row1.GetCell(col);

                                        if (specDescripCell == null || !specDescripCell.StringCellValue.Equals(specValue.SpecDescription))
                                        {
                                            conflict = true;
                                            break;
                                        }

                                        ICell specCell = row2.GetCell(col);
                                        if (specCell == null || !specCell.StringCellValue.Equals(specValue.Spec))
                                        {
                                            conflict = true;
                                            break;
                                        }

                                        col++;
                                    }
                                }

                                if (conflict) break;
                            }
                        }
                    }
                }
                else
                {
                    conflict = true;
                }

                if (!conflict)
                {
                    IRow row = sheet.CreateRow(sheet.LastRowNum + 1);

                    ICell snCell = row.CreateCell(0);
                    snCell.SetCellValue(AppInfo.PhoneInfo.SN);

                    ICell timeCell = row.CreateCell(1);
                    timeCell.SetCellValue(now.ToString("HH:mm:ss"));

                    int col = 2;
                    foreach (FlowItem flowItem in flowControl.FlowItemList)
                    {
                        if (!flowItem.Item.Property.Disable && !flowItem.IsAuxiliaryItem() && flowItem.SpecValueList != null)
                        {
                            foreach (SpecValue specValue in flowItem.SpecValueList)
                            {
                                if (!specValue.Disable)
                                {
                                    ICell valueCell = row.CreateCell(col);
                                    valueCell.SetCellValue(specValue.MeasuredValue);

                                    col++;
                                }
                            }
                        }
                    }

                    fs = File.OpenWrite(filename);
                    workbook.Write(fs);
                    fs.Close();
                }

                if (conflict)
                {
                    string renamefile = directory + now.ToString("yyyy_MM_dd_HH_mm_ss") + "_" + hostName + ".xlsx";
                    File.Move(filename, renamefile);
                    CreateNewDataReport(filename);
                }
            }
            else
            {
                CreateNewDataReport(filename);
            }
        }

        private static string CreateDirectory()
        {
            string path = @"d:\trt\data\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        private static void CreateNewDataReport(string filename)
        {
            using (FileStream fs = File.Create(filename))
            {
                DateTime now = DateTime.Now;

                IWorkbook workbook = new XSSFWorkbook();

                ISheet sheet = workbook.CreateSheet(SHEET_NAME);

                IFont boldFont = workbook.CreateFont();
                boldFont.IsBold = true;

                ICellStyle headStyle = workbook.CreateCellStyle();
                headStyle.SetFont(boldFont);
                headStyle.Alignment = HorizontalAlignment.Center;
                headStyle.VerticalAlignment = VerticalAlignment.Center;
                headStyle.BorderLeft = BorderStyle.Thin;
                headStyle.BorderTop = BorderStyle.Thin;
                headStyle.BorderRight = BorderStyle.Thin;
                headStyle.BorderBottom = BorderStyle.Thin;

                IRow row0 = sheet.CreateRow(0);
                IRow row1 = sheet.CreateRow(1);
                IRow row2 = sheet.CreateRow(2);
                IRow row3 = sheet.CreateRow(3);

                ICell snHeadCell = row0.CreateCell(0);
                snHeadCell.SetCellValue("SN");
                snHeadCell.CellStyle = headStyle;
                row1.CreateCell(0).CellStyle = headStyle;
                row2.CreateCell(1).CellStyle = headStyle;
                sheet.AddMergedRegion(new CellRangeAddress(0, 2, 0, 0));

                ICell snCell = row3.CreateCell(0);
                snCell.SetCellValue(AppInfo.PhoneInfo.SN);

                ICell timeHeadCell = row0.CreateCell(1);
                timeHeadCell.SetCellValue("时间");
                sheet.AddMergedRegion(new CellRangeAddress(0, 2, 1, 1));
                timeHeadCell.CellStyle = headStyle;

                ICell timeCell = row3.CreateCell(1);
                timeCell.SetCellValue(now.ToString("HH:mm:ss"));

                int col = 2;
                int firstCol = col;
                FlowControl flowControl = FlowControl.Instance;
                foreach (FlowItem flowItem in flowControl.FlowItemList)
                {
                    if (!flowItem.Item.Property.Disable && !flowItem.IsAuxiliaryItem() && flowItem.SpecValueList != null)
                    {
                        firstCol = col;
                        foreach (SpecValue specValue in flowItem.SpecValueList)
                        {
                            if (!specValue.Disable)
                            {
                                ICell specDescripCell = row1.CreateCell(col);
                                specDescripCell.SetCellValue(specValue.SpecDescription);
                                specDescripCell.CellStyle = headStyle;

                                ICell specCell = row2.CreateCell(col);
                                specCell.SetCellValue(specValue.Spec);
                                specCell.CellStyle = headStyle;

                                ICell valueCell = row3.CreateCell(col);
                                valueCell.SetCellValue(specValue.MeasuredValue);

                                col++;
                            }
                        }

                        ICell nameCell = row0.CreateCell(firstCol);
                        nameCell.SetCellValue(flowItem.Name);
                        nameCell.CellStyle = headStyle;
                        if (col - firstCol > 1)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(0, 0, firstCol, col - 1));
                        }
                    }

                }

                workbook.Write(fs);
            }            
        }
    }
}
