using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexed_SequentialFiles
{
    internal class RecordComparator : IComparer<Record>
    {
        public int Compare(Record? x, Record? y)
        {
            
            return x.GetKey() - y.GetKey();
        }
    }
    internal class Page
    {
        private Record[]? records;
        public Page()
        {
        
        }
        
        public void SetEmptyRecords()
        {
            this.records = new Record[Utils.numberOfRecordsInPage];
            for (int i = 0; i < Utils.numberOfRecordsInPage; i++) 
            {
                this.records[i] = new Record(-1, 0, 0, 0, 0, 0, -1, (byte)Flag.Empty);
            }
        }

        public void SetRecords(Record[] records)
        {
            this.records = records;
        }

        public Record GetRecord(int position)
        {
            return records[position];
        }

        public void MarkToDelete(int positionOnPage)
        {
            this.records[positionOnPage].SetFlag((byte)Flag.Delete);
        }

        public Record[]? GetRecords() { return records; }

        public int GetCount()
        {
            int count = 0;
            for (int i = 0; i < Utils.numberOfRecordsInPage; ++i)
            {
                if (records[i].GetFlag() != (byte)Flag.Empty)
                {
                    ++count;
                }
            }
            return count;
        }
        public void SetRecord(int position, Record record)
        {
            this.records[position] = record;
        }

        public int FindKey(int key)
        {
            for (int i = 0; i < Utils.numberOfRecordsInPage; ++i)
            {
                if (records[i].GetKey() == key)
                {
                    return i;
                }
            }
            return -1;
        }

        public int FindLargestSmaller(int key)
        {
            for (int i = 0; i < Utils.numberOfRecordsInPage; ++i)
            {
                if (records[i].GetKey() > key)
                {
                    return i - 1;
                }
            }
            return Utils.numberOfRecordsInPage - 1;
        }

        public void SortRecords()
        {
            Array.Sort(records, 0, this.GetCount(), new RecordComparator());
        }
    }
}
