using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexed_SequentialFiles
{
    internal class ReaderWriter
    {
        private int numberOfOperations;
        private string fileName;

        public ReaderWriter(string fileName)
        {
            this.fileName = fileName;
            this.numberOfOperations = 0;
            
        }

        public int GetNumberOfOperations() { return numberOfOperations; }
        public string GetFileName() { return fileName; }

        public Record[] ReadPageOfRecords(int pageNumber)
        {   
            FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Seek(pageNumber * Record.GetSize() * Utils.numberOfRecordsInPage, SeekOrigin.Begin);
            Record[] records = new Record[Utils.numberOfRecordsInPage];
            byte[] bytes = new byte[Record.GetSize() * Utils.numberOfRecordsInPage];
            fileStream.Read(bytes, 0, Record.GetSize() * Utils.numberOfRecordsInPage);
            byte[] tmp = new byte[Record.GetSize()];
            for (int i = 0; i < Utils.numberOfRecordsInPage; ++i)
            {
                Utils.MoveBytes(bytes, tmp, i * Record.GetSize(), 0, Record.GetSize());
                records[i] = new Record(tmp);
            }
            fileStream.Close();
            ++this.numberOfOperations;
            return records;
        }

        public Index[] ReadPageOfIndices(int pageNumber)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Index[] indices = new Index[Utils.numberOfIndicesInPage];
            byte[] bytes = new byte[Utils.numberOfIndicesInPage * Index.GetSize()];
            fileStream.Seek(pageNumber * Utils.numberOfIndicesInPage * Index.GetSize(), SeekOrigin.Begin);
            fileStream.Read(bytes, 0, Record.GetSize() * Utils.numberOfIndicesInPage);
            byte[] tmp = new byte[Index.GetSize()];
            for (int i = 0; i < Utils.numberOfIndicesInPage; ++i)
            {
                Utils.MoveBytes(bytes, tmp, i * Index.GetSize(), 0, Index.GetSize());
                indices[i] = new Index(tmp);
            }
            ++this.numberOfOperations;
            fileStream.Close();
            return indices;
        }
        public void WritePageOfRecords(int pageNumber, Record[] records)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Seek(pageNumber * Record.GetSize() * Utils.numberOfRecordsInPage, SeekOrigin.Begin);
            for (int i = 0; i < Utils.numberOfRecordsInPage; ++i)
            {
                fileStream.Write(records[i].GetBytes());
            }
            fileStream.Close();
            ++this.numberOfOperations;
        }
    }
}
