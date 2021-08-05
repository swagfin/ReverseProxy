using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ReverseProxy.Utilities
{
    public static class Partitioner
    {
        public static int CalculatePartitionIndex(string valueToPartitionOn, int maxNumberOfPartitions)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(valueToPartitionOn);
                byte[] hashedBytes = md5.ComputeHash(inputBytes);
                //Use First Byte
                byte firstByte = hashedBytes[0];
                int unMappedPartitionNr = firstByte;
                unMappedPartitionNr = Math.Abs(unMappedPartitionNr);
                int partitionIndex = unMappedPartitionNr % maxNumberOfPartitions;
                return partitionIndex;
            }
        }
    }
}
