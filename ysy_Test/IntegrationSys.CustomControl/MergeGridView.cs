using IntegrationSys.LogUtil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IntegrationSys.CustomControl
{
	internal class MergeGridView : DataGridView
	{
		private List<CellRange> mergeRangeList_;

		public void Merge(int row, int col, int rowCount, int colCount)
		{
			if (this.mergeRangeList_ == null)
			{
				this.mergeRangeList_ = new List<CellRange>();
			}
			this.mergeRangeList_.Add(new CellRange(row, col, rowCount, colCount));
		}

		private void MergeGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			bool flag = false;
			CellRange cellRange = null;
			if (this.CellInMergeRange(e.RowIndex, e.ColumnIndex, out flag, out cellRange))
			{
				using (Brush brush = new SolidBrush(base.GridColor))
				{
					using (Brush brush2 = new SolidBrush(e.CellStyle.BackColor))
					{
						using (Pen pen = new Pen(brush))
						{
							Rectangle mergeRangeBounds = this.GetMergeRangeBounds(cellRange);
							e.Graphics.FillRectangle(brush2, e.CellBounds);
							if (e.RowIndex == cellRange.Row + cellRange.Rows - 1)
							{
								e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
							}
							if (e.ColumnIndex == cellRange.Col + cellRange.Cols - 1)
							{
								e.Graphics.DrawLine(pen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);
							}
							Log.Debug(string.Concat(new object[]
							{
								"Cell[",
								e.RowIndex,
								", ",
								e.ColumnIndex,
								"] bounds = ",
								e.CellBounds,
								", content = ",
								e.Value,
								", merge bounds = ",
								mergeRangeBounds
							}));
						}
					}
				}
				e.Handled = true;
			}
		}

		private bool CellInMergeRange(int row, int col, out bool firstCell, out CellRange mergeRange)
		{
			firstCell = false;
			mergeRange = null;
			if (this.mergeRangeList_ != null)
			{
				foreach (CellRange current in this.mergeRangeList_)
				{
					if (current.Row <= row && row < current.Row + current.Rows && current.Col <= col && col < current.Col + current.Cols)
					{
						if (current.Row == row && current.Col == col)
						{
							firstCell = true;
						}
						mergeRange = current;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private Rectangle GetMergeRangeBounds(CellRange mergeRange)
		{
			int num = 2147483647;
			int num2 = 2147483647;
			int num3 = 0;
			int num4 = 0;
			for (int i = mergeRange.Row; i < mergeRange.Row + mergeRange.Rows; i++)
			{
				for (int j = mergeRange.Col; j < mergeRange.Col + mergeRange.Cols; j++)
				{
					Rectangle cellDisplayRectangle = base.GetCellDisplayRectangle(j, i, false);
					Log.Debug(string.Concat(new object[]
					{
						"Cell[",
						i,
						", ",
						j,
						"] bounds = ",
						cellDisplayRectangle
					}));
					if (!cellDisplayRectangle.IsEmpty)
					{
						if (num > cellDisplayRectangle.Left)
						{
							num = cellDisplayRectangle.Left;
						}
						if (num2 > cellDisplayRectangle.Top)
						{
							num2 = cellDisplayRectangle.Top;
						}
						if (num3 < cellDisplayRectangle.Right)
						{
							num3 = cellDisplayRectangle.Right;
						}
						if (num4 < cellDisplayRectangle.Bottom)
						{
							num4 = cellDisplayRectangle.Bottom;
						}
					}
				}
			}
			return new Rectangle(num, num2, num3 - num, num4 - num2);
		}
	}
}
