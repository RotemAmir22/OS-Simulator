namespace SharableSpreadSheet
{
    using System.Data;
    class SharableSpreadSheet
    {
        private readonly ReaderWriterLockSlim lockObject = new ReaderWriterLockSlim();
        DataTable dataTable;
        public int nR;
        public int nC;

        public SharableSpreadSheet(int nRows, int nCols, int nUsers = -1)
        {
            // nUsers used for setConcurrentSearchLimit, -1 means no limit.
            // Construct a nRows*nCols spreadsheet
            dataTable = new DataTable();
            nR = nRows;
            nC = nCols;

            // Add columns to the DataTable
            for (int col = 0; col < nCols; col++)
            {
                dataTable.Columns.Add(col.ToString(), typeof(string));
            }

            // Add rows to the DataTable
            for (int row = 0; row < nRows; row++)
            {
                DataRow dataRow = dataTable.NewRow();
                dataTable.Rows.Add(dataRow);
            }
        }

        public string GetCell(int row, int col)
        {
            // Return the string at [row, col]
            lockObject.EnterReadLock();
            try
            {
                return (string)dataTable.Rows[row][col];
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }

        public void SetCell(int row, int col, String str)
        {
            // Return the string at [row, col]
            lockObject.EnterReadLock();
            try
            {
                dataTable.Rows[row][col] = str;
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }
        public Tuple<int, int> SearchString(string str)
        {
            int row = -1, col = -1;
            lockObject.EnterReadLock();
            try
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i][j].Equals(str) == true)
                        {
                            row = i;
                            col = j;
                            break;
                        }
                    }
                }
            }
            finally
            {
                lockObject.ExitReadLock();
            }

            // Return the first cell indexes that contain the string (search from the first row to the last row)
            return Tuple.Create(row, col);
        }

        public void ExchangeRows(int row1, int row2)
        {
            lockObject.EnterWriteLock();
            try
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    object tmp = dataTable.Rows[row1][i];
                    dataTable.Rows[row1][i] = dataTable.Rows[row2][i];
                    dataTable.Rows[row2][i] = tmp;
                }
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }

        public void ExchangeCols(int col1, int col2)
        {
            lockObject.EnterWriteLock();
            try
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    object tmp = dataTable.Rows[i][col1];
                    dataTable.Rows[i][col1] = dataTable.Rows[i][col2];
                    dataTable.Rows[i][col2] = tmp;
                }
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }
        public int SearchInRow(int row, string str)
        {
            lockObject.EnterReadLock();
            try
            {
                int col = -1;
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (dataTable.Rows[row][i].Equals(str) == true)
                    {
                        col = i;
                        break;
                    }
                }
                return col;
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }

        public int SearchInCol(int col, string str)
        {
            lockObject.EnterReadLock();
            try
            {
                int row = -1;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i][col].Equals(str) == true)
                    {
                        row = i;
                        break;
                    }
                }
                return row;
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }

        public Tuple<int, int> SearchInRange(int col1, int col2, int row1, int row2, string str)
        {
            lockObject.EnterReadLock();
            try
            {
                // Perform search within a specific range: [row1:row2, col1:col2]
                // Includes col1, col2, row1, row2
                for (int i = col1; i <= col2; i++)
                {
                    for (int j = row1; j <= row2; j++)
                    {
                        if (GetCell(j, i).Equals(str))
                            return Tuple.Create(i, j);
                    }
                }
                return Tuple.Create(-1, -1);
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }
        public void AddRow(int row1)
        {
            lockObject.EnterWriteLock();
            try
            {
                DataRow newRow = dataTable.NewRow();
                dataTable.Rows.InsertAt(newRow, row1 + 1);
                nR++;
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }

        public void AddCol(int col1)
        {
            lockObject.EnterWriteLock();
            try
            {
                DataColumn newCol = new DataColumn();
                dataTable.Columns.Add(newCol);

                // Reorder the columns
                for (int i = dataTable.Columns.Count - 1; i > col1 + 1; i--)
                {
                    for (int j = 0; j < dataTable.Rows.Count; j++)
                    {
                        object tmp = dataTable.Rows[j][i];
                        dataTable.Rows[j][i] = dataTable.Rows[j][i - 1];
                        dataTable.Rows[j][i - 1] = tmp;
                    }
                }
                
                nC++;
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }


        public Tuple<int, int>[] FindAll(string str, bool caseSensitive)
        {
            lockObject.EnterReadLock();
            try
            {
                List<Tuple<int, int>> matchingCells = new List<Tuple<int, int>>();

                // Iterate over each row in the DataTable
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    // Iterate over each column in the DataTable
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        // Perform the search based on case sensitivity flag
                        string cellValue = (string)dataTable.Rows[i][j];
                        bool isMatch;
                        if (caseSensitive)
                            isMatch = cellValue?.Contains(str) == true;
                        else
                            isMatch = cellValue?.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0;

                        // If the cell value matches, add the cell coordinates to the list
                        if (isMatch)
                        {
                            matchingCells.Add(new Tuple<int, int>(i, j));
                        }
                    }
                }
                return matchingCells.ToArray();
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }
        public void SetAll(string oldStr, string newStr, bool caseSensitive)
        {
            lockObject.EnterWriteLock();
            try
            {
                Tuple<int, int>[] cellsToSet = FindAll(oldStr, caseSensitive);
                foreach (Tuple<int, int> cell in cellsToSet)
                {
                    dataTable.Rows[cell.Item1][cell.Item2] = newStr;
                }
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }


        public Tuple<int, int> getSize()
        {
            // return the size of the spreadsheet in nRows, nCols
            return Tuple.Create(nR, nC);
        }

        public void Save(string fileName)
        {
            lockObject.EnterReadLock();
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        DataRow dataRow = dataTable.Rows[row];
                        for (int col = 0; col < dataTable.Columns.Count; col++)
                        {
                            string cellValue = (string)dataRow[col];
                            writer.Write(cellValue);
                            if (col < dataTable.Columns.Count - 1)
                            {
                                writer.Write(",");
                            }
                        }
                        writer.WriteLine();
                    }
                }
            }
            finally
            {
                lockObject.ExitReadLock();
            }
        }

        public void Load(string fileName)
        {
            lockObject.EnterWriteLock();
            try
            {
                dataTable.Clear();
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] cellValues = line.Split(',');
                        DataRow dataRow = dataTable.Rows.Add();

                        for (int col = 0; col < cellValues.Length; col++)
                        {
                            dataRow[col] = cellValues[col].Trim();
                        }
                    }
                }
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }

    }
}