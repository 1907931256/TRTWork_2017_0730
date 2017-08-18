using System;

namespace IntegrationSys.CustomControl
{
	internal class CellRange
	{
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

		public CellRange(int row, int col, int rowCount, int colCount)
		{
			this.Row = row;
			this.Col = col;
			this.Rows = rowCount;
			this.Cols = colCount;
		}
	}
}
