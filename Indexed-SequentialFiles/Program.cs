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
dBMS.PrintRecordsInOrder();

Record r = new Record(8, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.InsertRecord(r);
dBMS.PrintRecordsInOrder();
r = new Record(10, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.InsertRecord(r);

r = new Record(1, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.InsertRecord(r);

r = new Record(4, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.InsertRecord(r);

r = new Record(2, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.InsertRecord(r);
r = new Record(13, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.InsertRecord(r);

r = new Record(9, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.InsertRecord(r);

r = new Record(11, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.InsertRecord(r);

r = new Record(12, 1, 1, 1, 1, 1, -1, (byte)Flag.Normal);
dBMS.InsertRecord(r);
dBMS.PrintRecordsInOrder();
dBMS.ReorganizeFile();
dBMS.PrintPages();
dBMS.PrintRecordsInOrder();
Console.WriteLine("Hello, World!");

