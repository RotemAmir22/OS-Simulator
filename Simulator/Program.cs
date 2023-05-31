using SharableSpreadSheet;
public class Program
{

    static void Main(String[] args)
    {
        try
        {
            Simulator(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]));
            Console.ReadLine();
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }
    static void Simulator(int row, int col, int nThreads, int nOper, int mssleep)
    {
        SharableSpreadSheet.SharableSpreadSheet spreadSheet = new SharableSpreadSheet.SharableSpreadSheet(row,col,nThreads);
        Random rand = new Random();
        for (int i = 0; i < row; i++) {
            for(int j = 0; j < col; j++)
            {                
                int c = rand.Next(65,122);
                spreadSheet.SetCell(i, j, ((char)c).ToString());
            }
        }

        for(int i = 0; i < nThreads; i++) 
        {
            Thread thread = new Thread(() => run(spreadSheet, nOper, mssleep));
            thread.Start();
        }

        

    }

    static void run(SharableSpreadSheet.SharableSpreadSheet spreadSheet, int nOper, int sleep)
    {
        Random rand = new Random();
        Random key = new Random();
        bool sencase;
        for ( int i = 0; i < nOper; i++) 
        {
            int choice = rand.Next(1, 14);
            switch (choice)
            {
                case 1:
                    spreadSheet.getSize();
                    break;
                case 2:
                    string old = ((char)key.Next(65, 122)).ToString();
                    string newstr = ((char)key.Next(65, 122)).ToString();
                    sencase = key.Next(2) == 1;
                    spreadSheet.SetAll(old, newstr, sencase);
                    break;
                case 3:
                    string str = ((char)key.Next(65, 122)).ToString();
                    sencase = key.Next(2) == 1;
                    spreadSheet.FindAll(str, sencase);
                    break;
                case 4:
                    int num = key.Next(0, spreadSheet.nC);
                    spreadSheet.AddCol(num);
                    break;
                case 5:
                    int num_ = key.Next(0, spreadSheet.nR);
                    spreadSheet.AddCol(num_);
                    break;
                case 6:
                    int startC = key.Next(0, spreadSheet.nC);
                    int endC = key.Next(startC, spreadSheet.nC);
                    int startR = key.Next(0, spreadSheet.nR);
                    int endR = key.Next(startR, spreadSheet.nR);
                    string toSearch = ((char)key.Next(65, 122)).ToString();
                    spreadSheet.SearchInRange(startC, endC, startR, endR, toSearch);
                    break; 
                case 7:
                    int stC = key.Next(0, spreadSheet.nC);
                    string toSC = ((char)key.Next(65, 122)).ToString();
                    spreadSheet.SearchInCol(stC, toSC);
                    break;
                case 8:
                    int stR = key.Next(0, spreadSheet.nR);
                    string toSR = ((char)key.Next(65, 122)).ToString();
                    spreadSheet.SearchInRow(stR, toSR);
                    break;
                case 9:
                    int col1 = key.Next(0, spreadSheet.nC);
                    int col2 = key.Next(0, spreadSheet.nC);
                    spreadSheet.ExchangeCols(col1, col2);
                    break;
                case 10:
                    int row1 = key.Next(0, spreadSheet.nR);
                    int row2 = key.Next(0, spreadSheet.nR);
                    spreadSheet.ExchangeRows(row1, row2);
                    break;
                case 11:
                    string findS = ((char)key.Next(65, 122)).ToString();
                    spreadSheet.SearchString(findS);
                    break;
                case 12:
                    int col = key.Next(0, spreadSheet.nC);
                    int row = key.Next(0, spreadSheet.nR);
                    string set = ((char)key.Next(65, 122)).ToString();
                    spreadSheet.SetCell(row, col, set);
                    break;
                case 13:
                    int indxC = key.Next(0, spreadSheet.nC);
                    int indxR = key.Next(0, spreadSheet.nR);
                    spreadSheet.GetCell(indxR, indxC);
                    break;
                default: break;
            }
            Thread.Sleep(sleep);
        }
        
    }
}