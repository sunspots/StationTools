using System;

namespace StationTools
{
    // CRC32 hashing
    class ItemHash
    {
        private static uint polynomial = 0xedb88320u;
        private static uint[] table = null;
        public static void GenerateTable()
        {
            if (table != null) return;

            table = new uint[256];

            for (int i = 0; i < 256; i++)
            {
                uint entry = (uint)i;
                for (uint j = 0; j < 8; j++)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry = entry >> 1;
                table[i] = entry;
            }
        }
        public static int Compute(string input)
        {
            GenerateTable();

            uint crc32 = 0xffffffff;
            foreach (uint c in input)
            {
                uint lookupIndex = (crc32 ^ c) & 0xFF;
                crc32 = (crc32 >> 8) ^ table[lookupIndex];
            }
            crc32 = crc32 ^ 0xFFFFFFFF;

            return (int)crc32;
        }
    }
}