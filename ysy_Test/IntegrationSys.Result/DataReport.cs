using IntegrationSys.Flow;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace IntegrationSys.Result
{
	public static class DataReport
	{
		private const string SHEET_NAME = "Data";

		//public static void Save()
		//{
		//	DateTime now = DateTime.Now;
		//	string hostName = Dns.GetHostName();
		//	string text = DataReport.CreateDirectory();
		//	string text2 = string.Concat(new string[]
		//	{
		//		text,
		//		now.ToString("yyyy_MM_dd"),
		//		"_",
		//		hostName,
		//		".xlsx"
		//	});
		//	FlowControl instance = FlowControl.Instance;
		//	if (File.Exists(text2))
		//	{
		//		bool flag = false;
		//		FileStream fileStream = File.OpenRead(text2);
		//		IWorkbook workbook = new XSSFWorkbook(fileStream);
		//		fileStream.Close();
		//		ISheet sheet = workbook.GetSheet("Data");
		//		if (sheet != null)
		//		{
		//			IRow row = sheet.GetRow(0);
		//			IRow row2 = sheet.GetRow(1);
		//			IRow row3 = sheet.GetRow(2);
		//			if (row == null || row2 == null || row3 == null)
		//			{
		//				flag = true;
		//				goto IL_1F1;
		//			}
		//			int num = 2;
		//			using (List<FlowItem>.Enumerator enumerator = instance.FlowItemList.GetEnumerator())
		//			{
		//				while (enumerator.MoveNext())
		//				{
		//					FlowItem current = enumerator.Current;
		//					if (!current.Item.Property.Disable && !current.IsAuxiliaryItem() && current.SpecValueList != null)
		//					{
		//						ICell cell = row.GetCell(num);
		//						if (cell == null || !cell.StringCellValue.Equals(current.Name))
		//						{
		//							flag = true;
		//							break;
		//						}
		//						foreach (SpecValue current2 in current.SpecValueList)
		//						{
		//							if (!current2.Disable)
		//							{
		//								ICell cell2 = row2.GetCell(num);
		//								if (cell2 == null || !cell2.StringCellValue.Equals(current2.SpecDescription))
		//								{
		//									flag = true;
		//									break;
		//								}
		//								ICell cell3 = row3.GetCell(num);
		//								if (cell3 == null || !cell3.StringCellValue.Equals(current2.Spec))
		//								{
		//									flag = true;
		//									break;
		//								}
		//								num++;
		//							}
		//						}
		//						if (flag)
		//						{
		//							break;
		//						}
		//					}
		//				}
		//				goto IL_1F1;
		//			}
		//		}
		//		flag = true;
		//		IL_1F1:
		//		if (!flag)
		//		{
		//			IRow row4 = sheet.CreateRow(sheet.LastRowNum + 1);
		//			ICell cell4 = row4.CreateCell(0);
		//			cell4.SetCellValue(AppInfo.PhoneInfo.SN);
		//			ICell cell5 = row4.CreateCell(1);
		//			cell5.SetCellValue(now.ToString("HH:mm:ss"));
		//			int num2 = 2;
		//			foreach (FlowItem current3 in instance.FlowItemList)
		//			{
		//				if (!current3.Item.Property.Disable && !current3.IsAuxiliaryItem() && current3.SpecValueList != null)
		//				{
		//					foreach (SpecValue current4 in current3.SpecValueList)
		//					{
		//						if (!current4.Disable)
		//						{
		//							ICell cell6 = row4.CreateCell(num2);
		//							cell6.SetCellValue(current4.MeasuredValue);
		//							num2++;
		//						}
		//					}
		//				}
		//			}
		//			fileStream = File.OpenWrite(text2);
		//			workbook.Write(fileStream);
		//			fileStream.Close();
		//		}
		//		if (flag)
		//		{
		//			string destFileName = string.Concat(new string[]
		//			{
		//				text,
		//				now.ToString("yyyy_MM_dd_HH_mm_ss"),
		//				"_",
		//				hostName,
		//				".xlsx"
		//			});
		//			File.Move(text2, destFileName);
		//			DataReport.CreateNewDataReport(text2);
		//			return;
		//		}
		//	}
		//	else
		//	{
		//		DataReport.CreateNewDataReport(text2);
		//	}
		//}

		private static string CreateDirectory()
		{
			string text = "d:\\trt\\data\\";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		private static void CreateNewDataReport(string filename)
		{
			using (FileStream fileStream = File.Create(filename))
			{
				DateTime now = DateTime.Now;
				IWorkbook workbook = new XSSFWorkbook();
				ISheet sheet = workbook.CreateSheet("Data");
				IFont font = workbook.CreateFont();
				font.IsBold = true;
				ICellStyle cellStyle = workbook.CreateCellStyle();
				cellStyle.SetFont(font);
				cellStyle.Alignment = HorizontalAlignment.Center;
				cellStyle.VerticalAlignment = VerticalAlignment.Center;
				cellStyle.BorderLeft = BorderStyle.Thin;
				cellStyle.BorderTop = BorderStyle.Thin;
				cellStyle.BorderRight = BorderStyle.Thin;
				cellStyle.BorderBottom = BorderStyle.Thin;
				IRow row = sheet.CreateRow(0);
				IRow row2 = sheet.CreateRow(1);
				IRow row3 = sheet.CreateRow(2);
				IRow row4 = sheet.CreateRow(3);
				ICell cell = row.CreateCell(0);
				cell.SetCellValue("SN");
				cell.CellStyle = cellStyle;
				row2.CreateCell(0).CellStyle = cellStyle;
				row3.CreateCell(1).CellStyle = cellStyle;
				sheet.AddMergedRegion(new CellRangeAddress(0, 2, 0, 0));
				ICell cell2 = row4.CreateCell(0);
				cell2.SetCellValue(AppInfo.PhoneInfo.SN);
				ICell cell3 = row.CreateCell(1);
				cell3.SetCellValue("时间");
				sheet.AddMergedRegion(new CellRangeAddress(0, 2, 1, 1));
				cell3.CellStyle = cellStyle;
				ICell cell4 = row4.CreateCell(1);
				cell4.SetCellValue(now.ToString("HH:mm:ss"));
				int num = 2;
				int num2 = num;
				FlowControl instance = FlowControl.Instance;
				foreach (FlowItem current in instance.FlowItemList)
				{
					if (!current.Item.Property.Disable && !current.IsAuxiliaryItem() && current.SpecValueList != null)
					{
						num2 = num;
						foreach (SpecValue current2 in current.SpecValueList)
						{
							if (!current2.Disable)
							{
								ICell cell5 = row2.CreateCell(num);
								cell5.SetCellValue(current2.SpecDescription);
								cell5.CellStyle = cellStyle;
								ICell cell6 = row3.CreateCell(num);
								cell6.SetCellValue(current2.Spec);
								cell6.CellStyle = cellStyle;
								ICell cell7 = row4.CreateCell(num);
								cell7.SetCellValue(current2.MeasuredValue);
								num++;
							}
						}
						ICell cell8 = row.CreateCell(num2);
						cell8.SetCellValue(current.Name);
						cell8.CellStyle = cellStyle;
						if (num - num2 > 1)
						{
							sheet.AddMergedRegion(new CellRangeAddress(0, 0, num2, num - 1));
						}
					}
				}
				workbook.Write(fileStream);
			}
		}
	}
}
