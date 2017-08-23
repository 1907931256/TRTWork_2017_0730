using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace IntegrationSys.CustomControl
{
    #region MergeGridView
    class MergeGridView : DataGridView
    {
        private List<CellRange> mergeRangeList_;

        public void Merge(int row, int col, int rowCount, int colCount)
        {
            if (mergeRangeList_ == null)
            {
                mergeRangeList_ = new List<CellRange>();
            }

            mergeRangeList_.Add(new CellRange(row, col, rowCount, colCount));
        }

        //protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        //{
        //    bool IsFirstCell = false;
        //    CellRange mergeRange = null;
        //    if (CellInMergeRange(e.RowIndex, e.ColumnIndex, out IsFirstCell, out mergeRange))
        //    {
        //        DataGridViewCell firstCell = this.Rows[mergeRange.Row].Cells[mergeRange.Col];
        //        using (Brush gridBrush = new SolidBrush(this.GridColor), backColorBrush = new SolidBrush(e.CellStyle.BackColor))
        //        {
        //            using (Pen gridLinePen = new Pen(gridBrush))
        //            {
        //                // Erase the cell.
        //                e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

        //                // Draw the grid lines (only the right and bottom lines)
        //                if (e.RowIndex == mergeRange.Row + mergeRange.Rows - 1)
        //                {
        //                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
        //                                        e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
        //                                        e.CellBounds.Bottom - 1);
        //                }

        //                if (e.ColumnIndex == mergeRange.Col + mergeRange.Cols - 1)
        //                {
        //                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
        //                        e.CellBounds.Top, e.CellBounds.Right - 1,
        //                        e.CellBounds.Bottom);
        //                }

        //                //Draw string
        //                if (firstCell.Value != null)
        //                {
        //                    int fontHeight = (int)e.Graphics.MeasureString(e.Value.ToString(), e.CellStyle.Font).Height;
        //                    int fontWidth = (int)e.Graphics.MeasureString(e.Value.ToString(), e.CellStyle.Font).Width;
        //                    int cellHeight = e.CellBounds.Height;
        //                    int cellWidth = e.CellBounds.Width;

        //                    int x = e.CellBounds.X - (e.ColumnIndex - mergeRange.Col) * cellWidth + (cellWidth * mergeRange.Cols - fontWidth) / 2;
        //                    int y = e.CellBounds.Y - (e.RowIndex - mergeRange.Row) * cellHeight + (cellHeight * mergeRange.Rows - fontHeight) / 2;
        //                    e.Graphics.DrawString((String)firstCell.Value, e.CellStyle.Font, Brushes.Black, x, y);
        //                }
        //            }
        //        }

        //        e.Handled = true;
        //    }
        //}

        /// <summary>
        /// 判断Cell(row, col)是否在Merge区域
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="isFirstCell">是否是Merge区域的第一个cell</param>
        /// <param name="mergeRange">返回Cell(row, col)所在的合并区域</param>
        /// <returns></returns>
        private bool CellInMergeRange(int row, int col, out bool firstCell, out CellRange mergeRange)
        {
            firstCell = false;
            mergeRange = null;

            if (mergeRangeList_ != null)
            {
                foreach (CellRange range in mergeRangeList_)
                {
                    if (range.Row <= row && row < range.Row + range.Rows
                     && range.Col <= col && col < range.Col + range.Cols)
                    {
                        if (range.Row == row && range.Col == col)
                        {
                            firstCell = true;
                        }
                        mergeRange = range;
                        return true;
                    }
                }
            }

            return false;
        }

        private Rectangle GetMergeRangeBounds(CellRange mergeRange)
        {
            int left = Int32.MaxValue;
            int top = Int32.MaxValue;
            int right = 0;
            int bottom = 0;
            for (int row = mergeRange.Row; row < mergeRange.Row + mergeRange.Rows; row++)
            {
                for (int col = mergeRange.Col; col < mergeRange.Col + mergeRange.Cols; col++)
                {
                    Rectangle cellBounds = this.GetCellDisplayRectangle(col, row, false);

                    if (!cellBounds.IsEmpty)
                    {
                        if (left > cellBounds.Left) left = cellBounds.Left;

                        if (top > cellBounds.Top) top = cellBounds.Top;

                        if (right < cellBounds.Right) right = cellBounds.Right;

                        if (bottom < cellBounds.Bottom) bottom = cellBounds.Bottom;
                    }
                }
            }

            return new Rectangle(left, top, right - left, bottom - top);
        }
    }
    #endregion

    #region CellRange
    class CellRange
    {
        public CellRange(int row, int col, int rowCount, int colCount)
        {
            Row = row;
            Col = col;
            Rows = rowCount;
            Cols = colCount;
        }

        public int Row
        {
            get;
            set;
        }

        public int Col
        {
            get;
            set;
        }

        public int Rows
        {
            get;
            set;
        }


        public int Cols
        {
            get;
            set;
        }

    }
    #endregion
}
