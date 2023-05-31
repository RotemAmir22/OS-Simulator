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
    }
}