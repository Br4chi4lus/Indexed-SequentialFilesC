using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexed_SequentialFiles
{
    internal class Index
    {
        private int key;
        private int pageNumber;


        public static int GetSize() { return 2 * sizeof(int); }
        //konstruktor
        public Index(int key, int pageNumber)
        {
            this.key = key;
            this.pageNumber = pageNumber;
        }
        public Index()
        {
            this.key = -1;
            this.pageNumber = -1;
        }
        public Index(byte[] bytes)
        {
            this.key = BitConverter.ToInt32(bytes, 0);
            this.pageNumber = BitConverter.ToInt32(bytes, 4);
        }

        public int GetKey()
        {
            return this.key;
        }
        public int GetPageNumber()
        {
            return this.pageNumber;
        }
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Index.GetSize()];
            Utils.MoveBytes(BitConverter.GetBytes(this.key), bytes, 0, 0, 4);
            Utils.MoveBytes(BitConverter.GetBytes(this.pageNumber), bytes, 0, 4, 4);
            return bytes;
        }

    }
}
