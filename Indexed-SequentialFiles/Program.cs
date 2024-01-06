// See https://aka.ms/new-console-template for more information
using Indexed_SequentialFiles;
void PrintActions()
{
    Console.WriteLine();
    Console.WriteLine("List of possible actions:");
    Console.WriteLine("a <key> <5 integers> - to add new record");
    Console.WriteLine("r <key> - to read record");
    Console.WriteLine("o - to reorganize file");
    Console.WriteLine("p - print the file in order");
    Console.WriteLine("f - toogle printing the file using pages");
    Console.WriteLine("d <key> - delete record");
    Console.WriteLine("u <key to update> <new key> <5 integers> - update record");
    Console.WriteLine("t <file name> - read commands from txt file");
    Console.WriteLine();
}
DBMS dBMS = new DBMS("index", "mainarea", "overflow");

char action = (char)0;
bool printing = false;
while(action != 'q')
{
    PrintActions();
    string line = Console.ReadLine();
    var words = line.Split(' ');
    action = line[0];
    if(action < 'a')
    {
        int tmp = action + 'a' - 'A';
        action = (char)tmp;
    }
    switch (action)
    {
        case 'a':
            //var line1 = Console.ReadLine();
            //var numbers = line1.Split(' ');
            if (words.Length != 7) 
            {
                Console.WriteLine("Wrong input");
            }
            else
            {
                int[] ints = new int[6];
                for(int i = 0; i < 6; ++i)
                {
                    ints[i] = Int32.Parse(words[i + 1]);
                }
                if (ints[0] < 1)
                {
                    Console.WriteLine("Key < 1");
                    break;
                }
                Record record = new Record(ints[0], ints[1], ints[2], ints[3], ints[4], ints[5], -1, (byte)Flag.Normal);
                Console.WriteLine("Number of operations: {0}", dBMS.AddRecord(record));
                if (printing == true)
                {
                    dBMS.PrintPages();
                }
            }
            break;
        case 'r':
            int key = Int32.Parse(words[1]);
            if (key < 1)
            {
                Console.WriteLine("Key < 1");
                break;
            }
            int operations = dBMS.GetNumberOfOperations();
            FindRecordInfo findRecordInfo = dBMS.FindRecord(new Record(key));
            if (findRecordInfo.record.GetKey() == key)
            {
                Console.WriteLine();
                Console.WriteLine(findRecordInfo.record.ToString());
            }
            Console.WriteLine("Number of operations: {0}", dBMS.GetNumberOfOperations() - operations);
            break;
        case 'o':           
            Console.WriteLine("Number of operations: {0}", dBMS.ReorganizeFile());
            if (printing == true)
            {
                dBMS.PrintPages();
            }
            break;
        case 'p':
            Console.WriteLine("Number of operations: {0}", dBMS.PrintRecordsInOrder());
            break;
        case 'f':
            printing = !printing;
            Console.WriteLine("Printing after each action: {0}", printing);
            break;
        case 'd':
            key = Int32.Parse(words[1]);
            if (key < 1)
            {
                Console.WriteLine("Key < 1");
                break;
            }
            Console.WriteLine("Number of operations: {0}", dBMS.DeleteRecord(key));
            if (printing == true)
            {
                dBMS.PrintPages();
            }
            break;
        case 'u':
            if (words.Length != 8)
            {
                Console.WriteLine("Wrong input");
            }
            else
            {
                int[] ints = new int[7];
                for (int i = 0; i < 7; ++i)
                {
                    ints[i] = Int32.Parse(words[i + 1]);
                }
                if (ints[1] < 1 || ints[0] < 1)
                {
                    Console.WriteLine("Key < 1");
                    break;
                }
                Record record = new Record(ints[1], ints[2], ints[3], ints[4], ints[5], ints[6], -1, (byte)Flag.Normal);
                Console.WriteLine("Number of operations: {0}", dBMS.UpdateRecord(ints[0], record));
                if (printing == true)
                {
                    dBMS.PrintPages();
                }
            }
            break;
        case 't':
            var fileName = words[1];
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fs);
            int n = dBMS.GetNumberOfOperations();
            char action1 = '\0';
            while ((line = streamReader.ReadLine()) != null) 
            {
                action = line[0];
                words = line.Split(' ');
                switch (action)
                {
                    case 'a':
                        if (words.Length != 7)
                        {
                            Console.WriteLine("Wrong input");
                        }
                        else
                        {
                            int[] ints = new int[6];
                            for (int i = 0; i < 6; ++i)
                            {
                                ints[i] = Int32.Parse(words[i + 1]);
                            }
                            if (ints[0] < 1)
                            {
                                break;
                            }
                            Record record = new Record(ints[0], ints[1], ints[2], ints[3], ints[4], ints[5], -1, (byte)Flag.Normal);
                            dBMS.AddRecord(record);
                            if (printing == true)
                            {
                                dBMS.PrintPages();
                            }
                        }
                        break;
                    case 'r':
                        key = Int32.Parse(words[1]);
                        if (key < 1)
                        {
                            break;
                        }
                        operations = dBMS.GetNumberOfOperations();
                        findRecordInfo = dBMS.FindRecord(new Record(key));
                        if (findRecordInfo.record.GetKey() == key)
                        {
                            Console.WriteLine();
                            Console.WriteLine(findRecordInfo.record.ToString());
                        }
                        break;
                    case 'o':                        
                        dBMS.ReorganizeFile();
                        Console.WriteLine("File reorganized");
                        if (printing == true)
                        {
                            dBMS.PrintPages();
                        }
                        break;
                    case 'p':
                        dBMS.PrintRecordsInOrder();
                        break;
                    case 'd':
                        key = Int32.Parse(words[1]);
                        if (key < 1)
                        {
                            break;
                        }
                        dBMS.DeleteRecord(key);
                        if (printing == true)
                        {
                            dBMS.PrintPages();
                        }
                        break;
                    case 'u':
                        if (words.Length != 8)
                        {
                            Console.WriteLine("Wrong input");
                        }
                        else
                        {
                            int[] ints = new int[7];
                            for (int i = 0; i < 7; ++i)
                            {
                                ints[i] = Int32.Parse(words[i + 1]);
                            }
                            if (ints[0] < 1 || ints[1] < 1)
                            {
                                break;
                            }
                            Record record = new Record(ints[1], ints[2], ints[3], ints[4], ints[5], ints[6], -1, (byte)Flag.Normal);
                            dBMS.UpdateRecord(ints[0], record);
                            if (printing == true)
                            {
                                dBMS.PrintPages();
                            }
                        }
                        break;                   
                    default:
                        Console.WriteLine("Couldn't recognize the action");
                        break;
                }
            }
            Console.WriteLine("Number of operations: {0}", dBMS.GetNumberOfOperations() - n);
            break;
        case 'q':
            break;
        default:
            Console.WriteLine("Couldn't recognize the action");
            break;
    }
}


