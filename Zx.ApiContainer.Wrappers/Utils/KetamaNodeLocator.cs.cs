using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using Zx.ApiContainer.Wrappers.Cache;

namespace Zx.ApiContainer.Wrappers.Utils
{
    public class KetamaNodeLocator
    {
        //private SortedList<long, string> ketamaNodes = new SortedList<long, string>();
        //private HashAlgorithm hashAlg;
        //private int numReps = 160;

        //public KetamaNodeLocator(List<string> nodes, int nodeCopies)
        //{
        //    ketamaNodes = new SortedList<long, string>();

        //    numReps = nodeCopies;
        //    //对所有节点，生成nCopies个虚拟结点
        //    foreach (string node in nodes)
        //    {
        //        //每四个虚拟结点为一组
        //        for (int i = 0; i < numReps / 4; i++)
        //        {
        //            //getKeyForNode方法为这组虚拟结点得到惟一名称 
        //            byte[] digest = HashAlgorithm.ComputeMd5(node + i);
        //            /** Md5是一个16字节长度的数组，将16字节的数组每四个字节一组，分别对应一个虚拟结点，这就是为什么上面把虚拟结点四个划分一组的原因*/
        //            for (int h = 0; h < 4; h++)
        //            {
        //                long m = HashAlgorithm.Hash(digest, h);
        //                ketamaNodes[m] = node;
        //            }
        //        }
        //    }
        //}

        //public string GetPrimary(string key)
        //{
        //    byte[] digest = HashAlgorithm.ComputeMd5(key);
        //    string rv = GetNodeForKey(HashAlgorithm.Hash(digest, 0));
        //    return rv;
        //}

        //private string GetNodeForKey(long hash)
        //{
        //    string rv;
        //    long key = hash;
        //    int pos = 0;
        //    if (!ketamaNodes.ContainsKey(key))
        //    {
        //        int low, high, mid;
        //        low = 1;
        //        high = ketamaNodes.Count - 1;
        //        while (low <= high)
        //        {
        //            mid = (low + high) / 2;
        //            if (key < ketamaNodes.Keys[mid])
        //            {
        //                high = mid - 1;
        //                pos = high;
        //            }
        //            else if (key > ketamaNodes.Keys[mid])
        //                low = mid + 1;

        //        }
        //    }
        //    rv = ketamaNodes.Values[pos + 1].ToString();
        //    return rv;
        //}    

        private Dictionary<long, PooledRedisClientManager> ketamaNodes;
        private int numReps = 160;
        private long[] keys;

        public KetamaNodeLocator(List<RedisHash> nodes)
        {
            ketamaNodes = new Dictionary<long, PooledRedisClientManager>();

            //对所有节点，生成nCopies个虚拟结点
            foreach (RedisHash node in nodes)
            {
                //每四个虚拟结点为一组
                for (int i = 0; i < numReps / 4; i++)
                {
                    //getKeyForNode方法为这组虚拟结点得到惟一名称 
                    byte[] digest = HashAlgorithm.ComputeMd5(string.Format("{0}:{1}_{2}", node.Host, node.Port, i));
                    /** Md5是一个16字节长度的数组，将16字节的数组每四个字节一组，分别对应一个虚拟结点，这就是为什么上面把虚拟结点四个划分一组的原因*/
                    for (int h = 0; h < 4; h++)
                    {
                        long m = HashAlgorithm.Hash(digest, h);
                        ketamaNodes[m] = node.PoolRedisClientManager;
                    }
                }
            }

            keys = ketamaNodes.Keys.OrderBy(p => p).ToArray();
        }

        public PooledRedisClientManager GetWorkerNode(string k)
        {
            byte[] digest = HashAlgorithm.ComputeMd5(k);
            return GetNodeInner(HashAlgorithm.Hash(digest, 0));
        }

        PooledRedisClientManager GetNodeInner(long hash)
        {
            if (ketamaNodes.Count == 0)
                return null;
            long key = hash;
            int near = 0;
            int index = Array.BinarySearch(keys, hash);
            if (index < 0)
            {
                near = (~index);
                if (near == keys.Length)
                    near = 0;
            }
            else
            {
                near = index;
            }

            return ketamaNodes[keys[near]];
        }
    }
}
