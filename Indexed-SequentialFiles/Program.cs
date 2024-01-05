// See https://aka.ms/new-console-template for more information
using Indexed_SequentialFiles;
void PrintActions()
{
    Console.WriteLine("List of possible actions:");
    Console.WriteLine("a - to add new record");
    Console.WriteLine("r - to read record");
    Console.WriteLine("o - to reorganize file");
    Console.WriteLine("p - print the file in order");
    Console.WriteLine("f - print the file using pages");
    Console.WriteLine("d - delete record");
    Console.WriteLine("u - update record");
    Console.WriteLine("t - read commands from txt file");

}
Console.WriteLine("Hello, World!");
Page page = new Page();
page.SetEmptyRecords();
page.MarkToDelete(1);

/*ReaderWriter readerWriter = new ReaderWriter("overflow");
readerWriter.WritePageOfRecords(0, page.GetRecords());
Page page1 = new Page(Utils.numberOfRecordsInPage);
page1.SetRecords(readerWriter.ReadPageOfRecords(0));
Console.WriteLine("Hello, World!");*/

DBMS dBMS = new DBMS("index", "mainarea", "overflow");
/*dBMS.PrintRecordsInOrder();

Record r = new Record(8, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.AddRecord(r);
dBMS.PrintRecordsInOrder();
r = new Record(10, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.AddRecord(r);

r = new Record(1, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.AddRecord(r);

r = new Record(4, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.AddRecord(r);

r = new Record(2, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.AddRecord(r);
r = new Record(13, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.AddRecord(r);

r = new Record(9, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.AddRecord(r);

r = new Record(11, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.AddRecord(r);

r = new Record(12, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.PrintPages();
dBMS.AddRecord(r);
dBMS.PrintRecordsInOrder();
dBMS.UpdateRecord(4, new Record(4, 2, 2, 2, 2, 2, -1 , (byte)Flag.Normal));
dBMS.DeleteRecord(4);
dBMS.PrintRecordsInOrder();
dBMS.UpdateRecord(4, new Record(4, 3, 2, 2, 2, 2, -1, (byte)Flag.Normal));
dBMS.UpdateRecord(13, new Record(17, 2, 2, 2, 2, 2, -1, (byte)Flag.Normal));
dBMS.UpdateRecord(11, new Record(11, 2, 2, 2, 2, 2, -1, (byte)Flag.Normal));
dBMS.PrintPages();
dBMS.PrintRecordsInOrder();
dBMS.ReorganizeFile();
dBMS.PrintPages();
dBMS.PrintRecordsInOrder();*/
char action = (char)0;
while(action != 'q')
{
    PrintActions();
    string line = Console.ReadLine();
    action = line[0];
    if(action < 'a')
    {
        int tmp = action + 'a' - 'A';
        action = (char)tmp;
    }
    switch (action)
    {
        case 'a':
            var line1 = Console.ReadLine();
            var numbers = line1.Split(' ');
            if (numbers.Length != 6) 
            {
                Console.WriteLine("Wrong input");
            }
            else
            {
                int[] ints = new int[6];
                for(int i = 0; i < 6; ++i)
                {
                    ints[i] = Int32.Parse(numbers[i]);
                }
                Record record = new Record(ints[0], ints[1], ints[2], ints[3], ints[4], ints[5], -1, (byte)Flag.Normal);
                dBMS.AddRecord(record);
            }
            break;
        case 'r':

            break;
        case 'o':
            dBMS.ReorganizeFile();
            Console.WriteLine("File reorganized");
            break;
        case 'p':
            dBMS.PrintRecordsInOrder();
            break;
        case 'f':
            dBMS.PrintPages();
            break;
        case 'd':
            var number = Console.ReadLine();
            int key = Int32.Parse(number);
            dBMS.DeleteRecord(key);
            break;
        case 'u':
            line1 = Console.ReadLine();
            numbers = line1.Split(' ');
            if (numbers.Length != 7)
            {
                Console.WriteLine("Wrong input");
            }
            else
            {
                int[] ints = new int[7];
                for (int i = 0; i < 7; ++i)
                {
                    ints[i] = Int32.Parse(numbers[i]);
                }
                Record record = new Record(ints[1], ints[2], ints[3], ints[4], ints[5], ints[6], -1, (byte)Flag.Normal);
                dBMS.UpdateRecord(ints[0], record);
            }
            break;
        case 't':
            var fileName = Console.ReadLine();
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fs);
            char action1 = '\0';
            while ((line = streamReader.ReadLine()) != null) 
            {
                action = line[0];
                switch (action)
                {
                    case 'a':
                        line1 = streamReader.ReadLine();
                        numbers = line1.Split(' ');
                        if (numbers.Length != 6)
                        {
                            Console.WriteLine("Wrong input");
                        }
                        else
                        {
                            int[] ints = new int[6];
                            for (int i = 0; i < 6; ++i)
                            {
                                ints[i] = Int32.Parse(numbers[i]);
                            }
                            Record record = new Record(ints[0], ints[1], ints[2], ints[3], ints[4], ints[5], -1, (byte)Flag.Normal);
                            dBMS.AddRecord(record);
                        }
                        break;
                    case 'r':

                        break;
                    case 'o':
                        dBMS.ReorganizeFile();
                        Console.WriteLine("File reorganized");
                        break;
                    case 'p':
                        dBMS.PrintRecordsInOrder();
                        break;
                    case 'f':
                        dBMS.PrintPages();
                        break;
                    case 'd':
                        number = streamReader.ReadLine();
                        key = Int32.Parse(number);
                        dBMS.DeleteRecord(key);
                        break;
                    case 'u':
                        line1 = streamReader.ReadLine();
                        numbers = line1.Split(' ');
                        if (numbers.Length != 7)
                        {
                            Console.WriteLine("Wrong input");
                        }
                        else
                        {
                            int[] ints = new int[7];
                            for (int i = 0; i < 7; ++i)
                            {
                                ints[i] = Int32.Parse(numbers[i]);
                            }
                            Record record = new Record(ints[1], ints[2], ints[3], ints[4], ints[5], ints[6], -1, (byte)Flag.Normal);
                            dBMS.UpdateRecord(ints[0], record);
                        }
                        break;                   
                    case 'q':
                        break;
                    default:
                        Console.WriteLine("Couldn't recognize the action");
                        break;
                }
            }
            break;
        case 'q':
            break;
        default:
            Console.WriteLine("Couldn't recognize the action");
            break;
    }
}


