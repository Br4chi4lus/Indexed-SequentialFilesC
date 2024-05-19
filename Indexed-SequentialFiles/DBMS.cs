using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexed_SequentialFiles
{
    struct FindRecordInfo
    {
        public bool readingMainArea;                   // page containing record
        public Record? record;               
        public ReaderWriter readerWriter;   
        public int pageNumber;
        public int positionOnPage;
    }
    internal class DBMS
    {
        private ReaderWriter mainAreaReaderWriter;      
        private ReaderWriter overflowAreaReaderWriter;
        private ReaderWriter indexReaderWriter;
        private Page Page;
        private Page reorganizeFilePage;
        private Index[]? indices = null;
        private bool readingMainArea;
        private int numberOfIndices;
        private int numberOfDeletedRecords;
        private int numberOfRecordsMainArea;
        private int pageNumber;
        private int numberOfRecordsOverflowArea;
        private int currentIndexNumber;
        private int currentRecordPosition;
        private Record? previousRecord;
        public DBMS(String indexFileName, String mainAreaFileName, String overflowAreaFileName)
        {

            this.readingMainArea = true;
            this.mainAreaReaderWriter = new ReaderWriter(mainAreaFileName);
            this.overflowAreaReaderWriter = new ReaderWriter(overflowAreaFileName);
            this.indexReaderWriter = new ReaderWriter(indexFileName);
            System.IO.File.Delete(this.overflowAreaReaderWriter.GetFileName());
            System.IO.File.Delete(this.mainAreaReaderWriter.GetFileName());
            System.IO.File.Delete(this.indexReaderWriter.GetFileName());
            this.numberOfDeletedRecords = 0;
            this.Page = new Page();
            this.pageNumber = -1;
            this.numberOfRecordsOverflowArea = 0;
            this.numberOfRecordsMainArea = 1;
            this.currentIndexNumber = -1;
            this.currentRecordPosition = -1;
            this.previousRecord = null;
            Page page = new Page();
            page.SetEmptyRecords();        
            page.SetRecord(0, new Record(0, (byte)Flag.First));
            this.mainAreaReaderWriter.WritePageOfRecords(0, page.GetRecords());
            this.indices = new Index[1];
            this.indices[0] = new Index(0, 0);
            this.indices = this.ExtendToFullPage(this.indices);
            this.indexReaderWriter.WritePageOfIndices(0, this.indices);
        }
        public Index[] ExtendToFullPage(Index[] indices)
        {
            int newSize = (int)Math.Ceiling(this.GetIndicesCount() / (double)Utils.numberOfIndicesInPage) * Utils.numberOfIndicesInPage;
            Index[] newIndices = new Index[newSize];
            for(int i = 0; i < newSize; ++i)
            {
                if (i < this.GetIndicesCount())
                {
                    newIndices[i] = indices[i];
                }
                else
                {
                    newIndices[i] = new Index();
                }
            }
            return newIndices;
        }
        public int GetIndicesCount()
        {
            for(int i = 0; i < indices.Length; ++i)
            {
                if (indices[i] == null || indices[i].GetKey() == -1)
                {
                    return i;
                }
            }
            return indices.Length;
        }
        public int GetNumberOfOperations()
        {
            return this.indexReaderWriter.GetNumberOfOperations() + this.mainAreaReaderWriter.GetNumberOfOperations() + this.overflowAreaReaderWriter.GetNumberOfOperations();
        }
        /*
        *  Returns page number 
        * 
        */
        public int FindIndexForRecord(int key)
        {
            if (indices == null)
            {
                return -1;
            }
            for(int i = 0; i < this.GetIndicesCount(); ++i)
            {
                if (indices[i].GetKey() > key)
                {
                    return indices[i - 1].GetPageNumber();
                }
            }
            return indices[this.GetIndicesCount() - 1].GetPageNumber();
        }

        public Record FindRecordMainArea(int key, int pageNumber)
        {
            if (pageNumber != this.pageNumber || this.readingMainArea == false)
            {
                this.readingMainArea = true;
                this.pageNumber = pageNumber;
                this.Page.SetRecords(mainAreaReaderWriter.ReadPageOfRecords(this.pageNumber));
            }
            for (int i = 0; i < Utils.numberOfRecordsInPage; ++i)
            {
                if (this.Page.GetRecord(i).GetKey() > key)
                {
                    return this.Page.GetRecord(i - 1);
                }
            }
            return this.Page.GetRecord(Utils.numberOfRecordsInPage - 1);
        }

        public FindRecordInfo FindRecordInOverflowArea(int key, short position)
        {
            FindRecordInfo findRecordInfo = new FindRecordInfo();
            findRecordInfo.readerWriter = this.overflowAreaReaderWriter;
            findRecordInfo.readingMainArea = false;
            findRecordInfo.pageNumber = -1;
            findRecordInfo.positionOnPage = -1;
            if (position == -1)
            {
                findRecordInfo.record = null;
                
                return findRecordInfo;
            }
            int pageNumber = Utils.CalculatePageNumberRecord(position);
            if (pageNumber != this.pageNumber || this.readingMainArea == true)
            {
                this.readingMainArea = false;
                this.pageNumber = pageNumber;
                this.Page.SetRecords(overflowAreaReaderWriter.ReadPageOfRecords(this.pageNumber));
            }
            int positionOnPage = Utils.CalculatePositionOnPage(position);
            Record record = this.Page.GetRecord(positionOnPage);            
            if (record.GetKey() > key)
            {
                findRecordInfo.record = null;
                return findRecordInfo;
            }
            else if (record.GetKey() < key)
            {
                if (record.GetNextRecord() == -1)
                {
                    findRecordInfo.record = record;
                    findRecordInfo.positionOnPage = positionOnPage;
                    findRecordInfo.pageNumber = pageNumber;
                    return findRecordInfo;
                }
                else
                {
                    FindRecordInfo tmp = this.FindRecordInOverflowArea(key, Page.GetRecord(positionOnPage).GetNextRecord());
                    if (tmp.record == null)
                    {
                        findRecordInfo.record = record;
                        findRecordInfo.positionOnPage = positionOnPage;
                        findRecordInfo.pageNumber = pageNumber;
                        return findRecordInfo;
                    }
                    else
                    {
                        return tmp;
                    }
                }
            }else if (record.GetKey() == key)
            {
                findRecordInfo.record = record;
                findRecordInfo.positionOnPage = positionOnPage;
                findRecordInfo.pageNumber = pageNumber;
                return findRecordInfo;
            }
            findRecordInfo.record = null;
            return findRecordInfo;
        }
        /*
         *  If record returned in struct contains:
         *  - key = -1 than record with given key doesn't exist and record with given key can be inserted there
         *  - key = record.Key than record with given key exists
         *  - key < record.Key than record with given key doesn't exist and record can be inserted in first open spot in overflow area
        */ 
        public FindRecordInfo FindRecord(Record record)
        {
            FindRecordInfo findRecordInfo = new FindRecordInfo();
            int pageNumber = this.FindIndexForRecord(record.GetKey());
            if (pageNumber != this.pageNumber || this.readingMainArea == false) 
            {
                this.readingMainArea = true;
                this.pageNumber = pageNumber;
                this.Page.SetRecords(mainAreaReaderWriter.ReadPageOfRecords(this.pageNumber));
            }
            int pageCount = this.Page.GetCount();
            if (pageCount < Utils.numberOfRecordsInPage)
            {
                int position = this.Page.FindKey(record.GetKey());
                if (position != -1)
                {
                    findRecordInfo.positionOnPage = position;
                    findRecordInfo.record = this.Page.GetRecord(position);
                    findRecordInfo.readingMainArea = true;
                    findRecordInfo.readerWriter = this.mainAreaReaderWriter;
                    findRecordInfo.pageNumber = this.pageNumber;
                    return findRecordInfo;
                }
                else
                {
                    findRecordInfo.readerWriter = this.mainAreaReaderWriter;
                    findRecordInfo.readingMainArea = true;
                    findRecordInfo.record = this.Page.GetRecord(pageCount);
                    findRecordInfo.pageNumber = pageNumber;
                    findRecordInfo.positionOnPage = pageCount;
                    return findRecordInfo;
                }                
            }
            else
            {
                Record recordMainArea = this.FindRecordMainArea(record.GetKey(), pageNumber);
                int position = this.Page.FindLargestSmaller(record.GetKey());
                FindRecordInfo findRecordInfoOverflow = this.FindRecordInOverflowArea(record.GetKey(), recordMainArea.GetNextRecord());
                if (findRecordInfoOverflow.record == null)
                {                 
                    findRecordInfo.readingMainArea = true;
                    findRecordInfo.record = recordMainArea;
                    findRecordInfo.readerWriter = this.mainAreaReaderWriter;
                    findRecordInfo.pageNumber = pageNumber;
                    findRecordInfo.positionOnPage = position;
                    return findRecordInfo;
                }
                else
                {
                    findRecordInfo.readingMainArea = false;
                    findRecordInfo.record = findRecordInfoOverflow.record;
                    findRecordInfo.readerWriter = this.overflowAreaReaderWriter;
                    findRecordInfo.pageNumber = findRecordInfoOverflow.pageNumber;
                    findRecordInfo.positionOnPage = findRecordInfoOverflow.positionOnPage;
                    return findRecordInfo;           
                }
            }
        }
        /*
         * returns true if record was succesfully inserted
         * returns false if record with given key was already present in the file
         */
        public bool InsertRecord(Record record, FindRecordInfo findRecordInfo)
        {            
            if (record.GetKey() == findRecordInfo.record.GetKey())
            {
                return false;
            }
            else if (findRecordInfo.record.GetFlag() == (byte)Flag.Empty)
            {
                if (this.pageNumber != findRecordInfo.pageNumber || this.readingMainArea != findRecordInfo.readingMainArea)
                {
                    this.pageNumber = findRecordInfo.pageNumber;
                    this.Page.SetRecords(this.mainAreaReaderWriter.ReadPageOfRecords(this.pageNumber));
                }
                this.Page.SetRecord(findRecordInfo.positionOnPage, record);
                this.Page.SortRecords();
                ++this.numberOfRecordsMainArea;
                findRecordInfo.readerWriter.WritePageOfRecords(findRecordInfo.pageNumber, this.Page.GetRecords());
            }
            else if (record.GetKey() > findRecordInfo.record.GetKey())
            {
                record.SetNextRecord(findRecordInfo.record.GetNextRecord());
                if (this.pageNumber != findRecordInfo.pageNumber || this.readingMainArea != findRecordInfo.readingMainArea)
                {
                    this.pageNumber = findRecordInfo.pageNumber;
                    this.readingMainArea = findRecordInfo.readingMainArea;
                    this.Page.SetRecords(findRecordInfo.readerWriter.ReadPageOfRecords(findRecordInfo.pageNumber));
                }
                findRecordInfo.record.SetNextRecord((short)this.numberOfRecordsOverflowArea);
                this.Page.SetRecord(findRecordInfo.positionOnPage,findRecordInfo.record);
                int pageNumber = Utils.CalculatePageNumberRecord((short)this.numberOfRecordsOverflowArea);
                int positionOnPage = Utils.CalculatePositionOnPage((short)this.numberOfRecordsOverflowArea);
                if (pageNumber != this.pageNumber || findRecordInfo.readerWriter == this.mainAreaReaderWriter)
                {
                    findRecordInfo.readerWriter.WritePageOfRecords(findRecordInfo.pageNumber, this.Page.GetRecords());
                    this.pageNumber = pageNumber;
                    this.readingMainArea = false;
                    if (positionOnPage == 0)
                    {
                        this.Page.SetEmptyRecords();
                    }
                    else
                    {
                        this.Page.SetRecords(this.overflowAreaReaderWriter.ReadPageOfRecords(pageNumber));
                    }
                }               
                this.Page.SetRecord(positionOnPage, record);
                this.overflowAreaReaderWriter.WritePageOfRecords(this.pageNumber, this.Page.GetRecords());
                ++this.numberOfRecordsOverflowArea;
                
            }
            return true;
        }
        public int ReorganizeFile()
        {
            int numberOfOperations = this.GetNumberOfOperations();
            int recordsInPage = (int)Math.Floor(Utils.numberOfRecordsInPage * Utils.alpha);
            Page page = new Page();
            int positionOnPage = 0;
            int pageNumber = 0;
            int newNumberOfRecordsMainArea = 0;
            page.SetEmptyRecords();
            this.currentIndexNumber = -1;
            this.currentRecordPosition = -1;
            this.previousRecord = null;
            this.readingMainArea = true;
            this.pageNumber = -1;
            int numberOfIndices = (int)Math.Ceiling((this.numberOfRecordsMainArea + this.numberOfRecordsOverflowArea - this.numberOfDeletedRecords) / (double)recordsInPage) + 1;
            Index[] newIndices = new Index[numberOfIndices];
            ReaderWriter readerWriter = new("tmp.bin");
            Record record = this.GetNextRecord();
            while (record != null && pageNumber != numberOfIndices)
            {               
                if (record.GetFlag() == (byte)Flag.Normal || record.GetFlag() == (byte)Flag.First)
                {
                    if (positionOnPage == 0)
                    {
                        newIndices[pageNumber] = new Index(record.GetKey(), pageNumber);
                    }
                    Record tmp = (Record)record.Clone();
                    tmp.SetNextRecord(-1);
                    page.SetRecord(positionOnPage, tmp);
                    ++newNumberOfRecordsMainArea;
                    ++positionOnPage;
                    if (positionOnPage == recordsInPage)
                    {
                        readerWriter.WritePageOfRecords(pageNumber, page.GetRecords());
                        page.SetEmptyRecords();
                        positionOnPage = 0;
                        ++pageNumber;
                        
                    }
                    
                }
                record = this.GetNextRecord();

            }
            if (positionOnPage != 0)
            {
                readerWriter.WritePageOfRecords(pageNumber, page.GetRecords());
            }
            File.Delete(this.mainAreaReaderWriter.GetFileName());
            System.IO.File.Move(readerWriter.GetFileName(), this.mainAreaReaderWriter.GetFileName());
            this.indices = newIndices;
            this.indices = this.ExtendToFullPage(newIndices);
            for (int i = 0; i < Math.Ceiling(this.indices.Length / (double)Utils.numberOfIndicesInPage); ++i)
            {
                Index[] indicesPage = this.GetPageIndices(i);
                this.indexReaderWriter.WritePageOfIndices(i, indicesPage);
            }
            this.pageNumber = -1;
            this.numberOfRecordsMainArea = newNumberOfRecordsMainArea;
            this.numberOfRecordsOverflowArea = 0;
            this.numberOfDeletedRecords = 0;
            System.IO.File.Delete(this.overflowAreaReaderWriter.GetFileName());
            this.mainAreaReaderWriter.AddOperations(readerWriter.GetNumberOfOperations());
            return this.GetNumberOfOperations() - numberOfOperations ;
        }

        /*
         * returns positive value if record was inserted
         * otherwise returns negative value
         */
        public int AddRecord(Record record)
        {
            int numberOfOperations = this.GetNumberOfOperations();
            FindRecordInfo findRecordInfo = this.FindRecord(record);
            bool inserted = this.InsertRecord(record, findRecordInfo);
            numberOfOperations = this.GetNumberOfOperations() - numberOfOperations;
            if (inserted == false) 
            {
                Console.WriteLine("Record with given key = {0} already exists in the file", record.GetKey());
                numberOfOperations = 0 - numberOfOperations;
            }
            if (this.numberOfRecordsOverflowArea + this.numberOfDeletedRecords == Math.Ceiling(this.numberOfRecordsMainArea * Utils.beta))
            {
                numberOfOperations += this.ReorganizeFile();
                Console.WriteLine("Had to reorganize file");
            }
            return numberOfOperations;
        }
        public Index[] GetPageIndices(int pageNumber)
        {
            Index[] page = new Index[Utils.numberOfIndicesInPage];
            for (int i = 0; i < Utils.numberOfIndicesInPage; i++) 
            {
                page[i] = this.indices[pageNumber*Utils.numberOfIndicesInPage + i];
            }
            return page;
        }
        public int DeleteRecord(int key)
        {
            int numberOfOperations = this.GetNumberOfOperations();
            Record record = new Record(key);
            FindRecordInfo  findRecordInfo = this.FindRecord(record);
            if (findRecordInfo.record.GetKey() == key && findRecordInfo.record.GetFlag() != (byte)Flag.First) 
            {
                if (findRecordInfo.pageNumber != this.pageNumber || this.readingMainArea != findRecordInfo.readingMainArea)
                {
                    this.pageNumber = findRecordInfo.pageNumber;
                    this.readingMainArea = findRecordInfo.readingMainArea;
                    this.Page.SetRecords(findRecordInfo.readerWriter.ReadPageOfRecords(this.pageNumber));
                }
                ++this.numberOfDeletedRecords;
                this.Page.GetRecord(findRecordInfo.positionOnPage).SetFlag((byte)Flag.Delete);
                findRecordInfo.readerWriter.WritePageOfRecords(findRecordInfo.pageNumber, this.Page.GetRecords());
                if (this.numberOfRecordsOverflowArea + this.numberOfDeletedRecords == Math.Ceiling(this.numberOfRecordsMainArea * Utils.beta))
                {
                    this.ReorganizeFile();
                    Console.WriteLine("Had to reorganize file");
                }
            }
            return this.GetNumberOfOperations() - numberOfOperations;
        }

        public Record? GetNextRecord()
        {
            Record record;
            if (previousRecord == null)
            {
                
                ++this.currentRecordPosition;
                this.currentIndexNumber = this.currentRecordPosition / Utils.numberOfRecordsInPage;
                int position = this.currentRecordPosition % Utils.numberOfRecordsInPage;
                this.pageNumber = this.indices[this.currentIndexNumber].GetPageNumber();
                this.Page.SetRecords(mainAreaReaderWriter.ReadPageOfRecords(this.pageNumber));
                record = this.Page.GetRecord(position);
            }
            else if (previousRecord.GetNextRecord() != (short)-1)
            {
                int pageNumber = Utils.CalculatePageNumberRecord(previousRecord.GetNextRecord());
                int position = Utils.CalculatePositionOnPage(previousRecord.GetNextRecord());
                if (this.pageNumber != pageNumber || this.readingMainArea == true)
                {
                    this.readingMainArea = false;
                    this.pageNumber = pageNumber;
                    this.Page.SetRecords(this.overflowAreaReaderWriter.ReadPageOfRecords(pageNumber));
                }
                record = this.Page.GetRecord(position);
            }
            else
            {
                ++this.currentRecordPosition;
                this.currentIndexNumber = this.currentRecordPosition / Utils.numberOfRecordsInPage;
                int position = this.currentRecordPosition % Utils.numberOfRecordsInPage;
                if(this.currentIndexNumber >= this.GetIndicesCount()) 
                {
                    this.previousRecord = null;
                    this.currentIndexNumber = -1;
                    this.currentRecordPosition = -1;
                    return null;
                }
                if (this.pageNumber != this.indices[this.currentIndexNumber].GetPageNumber() || this.readingMainArea == false)
                {
                    this.readingMainArea = true;
                    this.pageNumber = this.indices[this.currentIndexNumber].GetPageNumber();
                    this.Page.SetRecords(mainAreaReaderWriter.ReadPageOfRecords(this.pageNumber));
                }
                record = this.Page.GetRecord(position);
            }
            this.previousRecord = record;
            return record;
        }

        public int PrintRecordsInOrder()
        {
            int numberOfOperations = this.GetNumberOfOperations();
            this.currentIndexNumber = -1;
            this.currentRecordPosition = -1;
            this.previousRecord = null;
            this.readingMainArea = true;
            Record record = this.GetNextRecord();
            while (record != null)
            {
                if(record.GetFlag() != (byte)Flag.Empty)
                {
                    Console.WriteLine(record.ToString());
                }               
                record = this.GetNextRecord();
            }
            Console.WriteLine("Number of records in file: {0}", this.numberOfRecordsMainArea + this.numberOfRecordsOverflowArea);
            return this.GetNumberOfOperations() - numberOfOperations;
        }

        public int PrintPages()
        {
            int numberOfOperations = this.GetNumberOfOperations();
            Console.WriteLine("Index:");
            for(int i =0; i < indices.Length; ++i)
            {
                Console.WriteLine(indices[i].ToString());
            }
            Console.WriteLine("Main area:");
            for (int i = 0; i < this.GetIndicesCount(); ++i)
            {
                this.Page.SetRecords(mainAreaReaderWriter.ReadPageOfRecords(i));
                this.pageNumber = i;
                this.readingMainArea = true;
                Console.WriteLine("Page number: {0}", i);
                for (int j = 0; j < Utils.numberOfRecordsInPage; ++j)
                {
                    Console.WriteLine(this.Page.GetRecord(j));
                }
            }

            Console.WriteLine("Overflow area:");
            int numberOfPages = (int)Math.Ceiling(this.numberOfRecordsOverflowArea / (double)Utils.numberOfRecordsInPage);
            if(numberOfPages == 0)
            {
                Console.WriteLine("Overflow area is empty");
            }
            for (int i = 0; i < numberOfPages; ++i)
            {
                this.Page.SetRecords(overflowAreaReaderWriter.ReadPageOfRecords(i));
                this.pageNumber = i;
                this.readingMainArea = false;
                Console.WriteLine("Page number: {0}", i);
                for (int j = 0; j < Utils.numberOfRecordsInPage; ++j)
                {
                    Console.WriteLine(this.Page.GetRecord(j));
                }
            }
            return this.GetNumberOfOperations() - numberOfOperations;
        }

        

        public int UpdateRecord(int key, Record record)
        {
            if (key < 1)
            {
                return 0;
            }
            int numberOfOperations = this.GetNumberOfOperations();
            Record tmp = new Record(key, 1, 1, 1, 1, 1, -1, (byte)Flag.Empty);
            FindRecordInfo findRecordInfo = this.FindRecord(tmp);
            if (key != findRecordInfo.record.GetKey())
            {
                return this.GetNumberOfOperations() - numberOfOperations;
            }
            if (key == record.GetKey())
            {
                if (this.readingMainArea != findRecordInfo.readingMainArea || this.pageNumber != findRecordInfo.pageNumber)
                {
                    this.readingMainArea = findRecordInfo.readingMainArea;
                    this.pageNumber = findRecordInfo.pageNumber;
                    this.Page.SetRecords(findRecordInfo.readerWriter.ReadPageOfRecords(this.pageNumber));
                }
                if(findRecordInfo.record.GetFlag() == (byte)Flag.Delete)
                {
                    --this.numberOfDeletedRecords;
                }
                record.SetFlag((byte)Flag.Normal);
                record.SetNextRecord(findRecordInfo.record.GetNextRecord());
                this.Page.SetRecord(findRecordInfo.positionOnPage, record);
                findRecordInfo.readerWriter.WritePageOfRecords(this.pageNumber, this.Page.GetRecords());
            }
            else
            {
                if (this.AddRecord(record) < 0)
                {                   
                    return this.GetNumberOfOperations() - numberOfOperations;
                }

                if (this.readingMainArea != findRecordInfo.readingMainArea || this.pageNumber != findRecordInfo.pageNumber)
                {
                    this.readingMainArea = findRecordInfo.readingMainArea;
                    this.pageNumber = findRecordInfo.pageNumber;
                    this.Page.SetRecords(findRecordInfo.readerWriter.ReadPageOfRecords(this.pageNumber));
                }
                this.Page.MarkToDelete(findRecordInfo.positionOnPage);
                findRecordInfo.readerWriter.WritePageOfRecords(this.pageNumber, this.Page.GetRecords());
                ++this.numberOfDeletedRecords;
                if (this.numberOfRecordsOverflowArea + this.numberOfDeletedRecords == Math.Ceiling(this.numberOfRecordsMainArea * Utils.beta))
                {
                    this.ReorganizeFile();
                    Console.WriteLine("Had to reorganize file");
                }
            }
            return this.GetNumberOfOperations() - numberOfOperations;
        }
    }
}
