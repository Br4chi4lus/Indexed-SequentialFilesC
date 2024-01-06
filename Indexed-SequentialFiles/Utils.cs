using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexed_SequentialFiles
{
    internal static class Utils
    {
        public static int numberOfRecordsInPage = 4;
        public static int numberOfIndicesInPage = 25;
        public static double beta = 0.1;
        public static double alpha = 0.5;
        public static void MoveBytes(byte[] srcBytes, byte[] dstBytes, int srcPos, int dstPos, int size)
        {
            for(int i = 0; i < size; ++i)
            {
                dstBytes[dstPos + i] = srcBytes[srcPos + i];
            }
        }
        
        public static int CalculatePageNumberRecord(short position)
        {
            return (int) position / numberOfRecordsInPage;
        }
        public static int CalculatePositionOnPage(short position)
        {
            return (int)position % numberOfRecordsInPage;
        }
    }
}
