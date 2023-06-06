using SharableSpreadSheet;
public class Program
{
    private static Random random = new Random();
    public static string GenerateRandomCharacter()
    {
        int range1Min = 65;  // ASCII range for uppercase letters
        int range1Max = 90;
        int range2Min = 97;  // ASCII range for lowercase letters
        int range2Max = 122;

        // Randomly select one of the two ranges
        int selectedRange = random.Next(0, 2);

        // Generate a random integer within the selected range
        int randomNum;
        if (selectedRange == 0)
            randomNum = random.Next(range1Min, range1Max + 1);
        else
            randomNum = random.Next(range2Min, range2Max + 1);

        // Convert the integer to its corresponding ASCII character
        char randomChar = (char)randomNum;

        return randomChar.ToString();
    }

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
        if(nThreads <=0) { nThreads = 1; }
        SharableSpreadSheet.SharableSpreadSheet spreadSheet = new SharableSpreadSheet.SharableSpreadSheet(row,col,nThreads);
        for (int i = 0; i < row; i++) {
            for(int j = 0; j < col; j++)
            {
                string randomStr = GenerateRandomCharacter() + GenerateRandomCharacter() + GenerateRandomCharacter();
                spreadSheet.SetCell(i, j, randomStr);
            }
        }
        //spreadSheet.PrintDataTable();
        Thread[] threads = new Thread[nThreads];
        for (int i = 0; i < nThreads; i++)
        {
            Thread thread = new Thread(() => run(spreadSheet, nOper, mssleep));
            thread.Name = "Thread " + i.ToString();
            threads[i] = thread; // Store the thread in an array or list
            thread.Start();
        }

        for (int i = 0; i < nThreads; i++)
        {
            threads[i].Join();
        }

        //spreadSheet.PrintDataTable();
        Console.ReadLine();



    }

    static void run(SharableSpreadSheet.SharableSpreadSheet spreadSheet, int nOper, int sleep)
    {
        string name = Thread.CurrentThread.Name + ": ";
        Random rand = new Random();
        Random key = new Random();
        bool sencase;
        for ( int i = 0; i < nOper; i++) 
        {
            string randomStr1 = GenerateRandomCharacter() + GenerateRandomCharacter() + GenerateRandomCharacter();
            string randomStr2 = GenerateRandomCharacter() + GenerateRandomCharacter() + GenerateRandomCharacter();
            int choice = rand.Next(1, 14);
            switch (choice)
            {
                case 1:
                    Tuple<int, int> size = spreadSheet.getSize();
                    Console.WriteLine(name + "getSize() -> " + size.ToString());
                    break;
                case 2: 
                    sencase = key.Next(2) == 1;
                    spreadSheet.SetAll(randomStr1, randomStr2, sencase);
                    Console.WriteLine(name + "SetAll() -> Old string: " + randomStr1 + ", New string: " + randomStr2 + ". Case sensitive: " + sencase);
                    break;
                case 3:
                    string str = randomStr1;
                    sencase = key.Next(2) == 1;
                    Tuple<int, int>[] result = spreadSheet.FindAll(str, sencase);
                    string resultString = string.Join("\n\t", result.Select(tuple => tuple.ToString()));

                    Console.WriteLine(name + "FindAll() -> String: " + str + ", Case sensitive: " + sencase + "\n\t  Found points: " + resultString);
                    break;
                case 4:
                    int num = key.Next(0, spreadSheet.nC);
                    spreadSheet.AddCol(num + 1);
                    Console.WriteLine(name + "AddCol() -> After added col: " + num);
                    break;
                case 5:
                    int num_ = key.Next(0, spreadSheet.nR);
                    spreadSheet.AddRow(num_ + 1);
                    Console.WriteLine(name + "AddRow() -> After added row: " + num_);
                    break;
                case 6:
                    int startC = key.Next(0, spreadSheet.nC);
                    int endC = key.Next(startC, spreadSheet.nC);
                    int startR = key.Next(0, spreadSheet.nR);
                    int endR = key.Next(startR, spreadSheet.nR);
                    string toSearch = randomStr1;
                    Tuple <int, int> searchRes = spreadSheet.SearchInRange(startC, endC, startR, endR, toSearch);
                    Console.WriteLine(name + "SearchInRange() -> String: " + toSearch + " Range: cols[" + startC + "-" + endC + "], rows[" + startR + "-" + endR + "]\n\t  Found: " + searchRes.ToString());
                    break; 
                case 7:
                    int stC = key.Next(0, spreadSheet.nC);
                    string toSC = randomStr1;
                    int idx = spreadSheet.SearchInCol(stC, toSC);
                    Console.WriteLine(name + "SearchInCol() -> String: " + toSC + " Col: " + stC + "\n\t  Found: " + idx);
                    break;
                case 8:
                    int stR = key.Next(0, spreadSheet.nR);
                    string toSR = randomStr1;
                    int indx = spreadSheet.SearchInRow(stR, toSR);
                    Console.WriteLine(name + "SearchInRow() -> String: " + toSR + " Col: " + stR + "\n\t  Found: " + indx);
                    break;
                case 9:
                    int col1 = key.Next(0, spreadSheet.nC);
                    int col2 = key.Next(0, spreadSheet.nC);
                    spreadSheet.ExchangeCols(col1, col2);
                    Console.WriteLine(name + "ExchangeCols() -> cols " + col1 + " and " + col2 + " have been switches successfully");
                    break;
                case 10:
                    int row1 = key.Next(0, spreadSheet.nR);
                    int row2 = key.Next(0, spreadSheet.nR);
                    spreadSheet.ExchangeRows(row1, row2);
                    Console.WriteLine(name + "ExchangeRows() -> rows " + row1 + " and " + row2 + " have been switches successfully");
                    break;
                case 11:
                    string findS = randomStr1;
                    Tuple<int, int> location = spreadSheet.SearchString(findS);
                    Console.WriteLine(name + "SearchString() -> string " + findS + " is found in cell " + location.ToString());
                    break;
                case 12:
                    int col = key.Next(0, spreadSheet.nC);
                    int row = key.Next(0, spreadSheet.nR);
                    string set = randomStr1;
                    spreadSheet.SetCell(row, col, set);
                    Console.WriteLine(name + "SetCell() -> cell " + "(" + row + ", " + col + ")" + " value has been changed to: " + set);
                    break;
                case 13:
                    int indxC = key.Next(0, spreadSheet.nC);
                    int indxR = key.Next(0, spreadSheet.nR);
                    string value = spreadSheet.GetCell(indxR, indxC);
                    Console.WriteLine(name + "GetCell() -> cell " + "(" + indxR + ", " + indxC + ")" + " value is: " + value);
                    break;
                default: 
                    break;
            }
            Thread.Sleep(sleep);
            //bye
        }
        
    }
}