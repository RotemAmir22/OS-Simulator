namespace SharableSpreadSheet
{
    using System.Windows.Forms;
    class SharableSpreadSheet
    {
            private readonly ReaderWriterLockSlim lockObject = new ReaderWriterLockSlim();
            DataGridView dataGrid;
            int nR;
            int nC;
            public SharableSpreadSheet(int nRows, int nCols, int nUsers = -1)
            {
                // nUsers used for setConcurrentSearchLimit, -1 mean no limit.
                // construct a nRows*nCols spreadsheet
                dataGrid = new DataGridView();
                nR = nRows;
                nC = nCols;
                dataGrid.SetBounds(0, 0, nCols, nRows);

            }
            public String getCell(int row, int col)
            {
            // return the string at [row,col]
                lockObject.EnterReadLock();
                try
                {
                    return dataGrid.Rows[row].Cells[col].Value?.ToString();
                }
                finally
                {
                    lockObject.ExitReadLock();
                }
            }
            public void setCell(int row, int col, String str)
            {
                // set the string at [row,col]
                lockObject.EnterWriteLock();
                try
                {
                    dataGrid.Rows[row].Cells[col].Value = str;
                }
                finally
                {
                    lockObject.ExitWriteLock();
                }

            }
            public Tuple<int, int> searchString(String str)
            {
                int row = -1, col = -1;
                lockObject.EnterReadLock();
                try
                {
                    for (int i = 0; i < dataGrid.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGrid.Columns.Count; j++)
                        {
                            if (dataGrid.Rows[i].Cells[j].Value?.ToString().Equals(str) == true)
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

                // return first cell indexes that contain the string (search from first row to the last row)
                return Tuple.Create(row, col);
            }
            public void exchangeRows(int row1, int row2)
            {
                lockObject.EnterWriteLock();
                try
                {
                    for (int i = 0; i < dataGrid.Columns.Count; i++)
                    {
                        DataGridViewCell tmp = dataGrid.Rows[row1].Cells[i];
                        dataGrid.Rows[row1].Cells[i].Value = dataGrid.Rows[row2].Cells[i].Value;
                        dataGrid.Rows[row2].Cells[i].Value = tmp.Value;
                    }
                }
                finally
                {
                    lockObject.ExitWriteLock();
                }
               }
            public void exchangeCols(int col1, int col2)
            {
                lockObject.EnterWriteLock();
                try
                {
                    for (int i = 0; i < dataGrid.Rows.Count; i++)
                    {
                        DataGridViewCell tmp = dataGrid.Rows[i].Cells[col1];
                        dataGrid.Rows[i].Cells[col1].Value = dataGrid.Rows[i].Cells[col2].Value;
                        dataGrid.Rows[i].Cells[col2].Value = tmp.Value;
                    }
                }
                finally
                {
                    lockObject.ExitWriteLock();
                }
            }
            public int searchInRow(int row, String str)
            {
                lockObject.EnterReadLock();
                try
                {
                    int col = -1;
                    for (int i = 0; i < dataGrid.Columns.Count; i++)
                    {
                        if (dataGrid.Rows[row].Cells[i].Value?.ToString().Equals(str) == true)
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
            public int searchInCol(int col, String str)
            {
                lockObject.EnterReadLock();
                try
                {
                    int row = -1;
                    for (int i = 0; i < dataGrid.Rows.Count; i++) 
                    { 
                        if (dataGrid.Rows[i].Cells[col].Value.Equals(str))
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
            public Tuple<int, int> searchInRange(int col1, int col2, int row1, int row2, String str)
            {
                lockObject.EnterReadLock();
                try
                {
                // perform search within spesific range: [row1:row2,col1:col2] 
                //includes col1,col2,row1,row2
                for (int i = col1; i <= col2; i++)
                {
                    for(int j = row1; j <= row2; j++)
                    {
                        if (getCell(j, i).Equals(str))
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
            public void addRow(int row1)
            {
                lockObject.EnterReadLock();
                try
                {
                    DataGridViewRow newRow = new DataGridViewRow();
                    for (int i = 0; i < dataGrid.Columns.Count; i++)
                        newRow.Cells.Add(new DataGridViewTextBoxCell());
                    dataGrid.Rows.Insert(row1 + 1, newRow);
                    nR++;
                }
                finally
                {
                    lockObject.ExitReadLock();
                }
            }
            public void addCol(int col1)
            {
                lockObject.EnterReadLock();
                try
                {
                    //add a column after col1
                    DataGridViewColumn newCol = new DataGridViewColumn();
                    dataGrid.Columns.Insert(col1 + 1, newCol);
                    nC++;
                }
                finally
                {
                    lockObject.ExitReadLock();
                }
            }
            public Tuple<int, int>[] findAll(String str,bool caseSensitive)
            {
                lockObject.EnterReadLock();
                try
                {
                    List<Tuple<int, int>> matchingCells = new List<Tuple<int, int>>();

                    // Iterate over each row in the DataGridView
                    foreach (DataGridViewRow row in dataGrid.Rows)
                    {
                        // Iterate over each cell in the row
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            // Perform the search based on case sensitivity flag
                            bool isMatch;
                            if (caseSensitive)
                                isMatch = cell.Value?.ToString().Contains(str) == true;
                            else
                                isMatch = cell.Value?.ToString().IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0;

                            // If the cell value matches, add the cell coordinates to the list
                            if (isMatch)
                            {
                                int rowIndex = cell.RowIndex;
                                int columnIndex = cell.ColumnIndex;
                                matchingCells.Add(new Tuple<int, int>(rowIndex, columnIndex));
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
            public void setAll(String oldStr, String newStr ,bool caseSensitive)
            {
                // replace all oldStr cells with the newStr str according to caseSensitive param
                lockObject.EnterWriteLock();
                try
                {
                    Tuple<int, int>[] cellsToSet = FindAll(oldStr, caseSensitive);
                    foreach (Tuple<int, int> cell in cellsToSet)
                    {
                        SetCell(cell.Item1, cell.Item2, newStr);
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

            public void save(String fileName)
            {
            lockObject.EnterReadLock();
            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    for (int row = 0; row < nR; row++)
                    {
                        for (int col = 0; col < nC; col++)
                        {
                            string cellValue = getCell(row, col);
                            writer.Write(cellValue);
                            if (col < nC - 1)
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
    
            public void load(String fileName)
            {
                lockObject.EnterWriteLock();
                try
                {
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        string line;
                        int row = 0;

                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] cellValues = line.Split(',');
                            int col = 0;

                            foreach (string cellValue in cellValues)
                            {
                                setCell(row, col, cellValue.Trim());
                                col++;
                            }

                            row++;
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