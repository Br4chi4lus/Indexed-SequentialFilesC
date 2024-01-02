using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexed_SequentialFiles
{
    enum Flag : byte
    {
        Normal = 0x00,
        First = 0x01,
        Empty = 0xFE,
        Delete = 0xFF
    };
    internal class Record : ICloneable
    {
        int key;
        int a;
        int b;
        int c;                                                                          //  flag values:
        int d;                                                                          //  0xFF - marked to delete
        int e;                                                                          //  0xFE - empty record
        short nextRecord;                                                               //  0x00 - just record
        byte flag;                                                                      //  0x01 - first record not to be manipulated
        
        public static int GetSize() { return 6 * sizeof(int) + sizeof(short) + sizeof(byte); }

        public Record(int key, int a, int b, int c, int d, int e, short nextRecord, byte flag)
        {
            this.key = key;
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.nextRecord = nextRecord;
            this.flag = flag;
        }

        public Record(byte[] bytes)
        {
            this.key = BitConverter.ToInt32(bytes, 0);
            this.a = BitConverter.ToInt32(bytes, 4);
            this.b = BitConverter.ToInt32(bytes, 8);
            this.c = BitConverter.ToInt32(bytes, 12);
            this.d = BitConverter.ToInt32(bytes, 16);
            this.e = BitConverter.ToInt32(bytes, 20);
            this.nextRecord = BitConverter.ToInt16(bytes, 24);
            this.flag = bytes[26];
        }

        public Record(int key)
        {
            this.key = key;
            this.a = 0;
            this.b = 0;
            this.c = 0;
            this.d = 0;
            this.e = 0;
            this.nextRecord = -1;
            this.flag = (byte) Flag.Empty;
        }
        public int GetKey() { return key; }
        public short GetNextRecord() { return nextRecord; }
        public byte GetFlag() { return flag; } 

        public void SetKey(int key) { this.key = key;}
        public void SetA(int a) { this.a = a;}
        public void SetB(int b) {  this.b = b;}
        public void SetC(int c) {  this.c = c;}
        public void SetD(int d) {  this.d = d;}
        public void SetE(int e) {  this.e = e;}
        public void SetNextRecord(short nextRecord) { this.nextRecord = nextRecord;}
        public void SetFlag(byte flag) {  this.flag = flag;}
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Record.GetSize()];
            Utils.MoveBytes(BitConverter.GetBytes(this.key), bytes, 0, 0, 4);
            Utils.MoveBytes(BitConverter.GetBytes(this.a), bytes, 0, 4, 4);
            Utils.MoveBytes(BitConverter.GetBytes(this.b), bytes, 0, 8, 4);
            Utils.MoveBytes(BitConverter.GetBytes(this.c), bytes, 0, 12, 4);
            Utils.MoveBytes(BitConverter.GetBytes(this.d), bytes, 0, 16, 4);
            Utils.MoveBytes(BitConverter.GetBytes(this.e), bytes, 0, 20, 4);
            Utils.MoveBytes(BitConverter.GetBytes(this.nextRecord), bytes, 0, 24, 2);
            bytes[26] = this.flag;
            return bytes;
        }
        public override string ToString()
        {
            return "Key: " + this.key + " a: " + this.a + " b: " + this.b + " c: " + this.c + " d: " + this.d + " e: " + this.e + " next: " + this.nextRecord + " flag: " + Enum.GetName(typeof(Flag), this.flag);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
