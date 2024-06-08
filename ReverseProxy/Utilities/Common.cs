using System;
using System.Security.Cryptography;

namespace ReverseProxy.Utilities
{
    public static class Common
    {
        public static int CalculatePartitionIndex(string valueToPartitionOn, int maxNumberOfPartitions)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(valueToPartitionOn);
                byte[] hashedBytes = md5.ComputeHash(inputBytes);
                //use first byte
                int unMappedPartitionNr = hashedBytes[0];
                unMappedPartitionNr = Math.Abs(unMappedPartitionNr);
                int partitionIndex = unMappedPartitionNr % maxNumberOfPartitions;
                return partitionIndex;
            }
        }
    }
}
