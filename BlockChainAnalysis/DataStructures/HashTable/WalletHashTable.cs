using System;
using System.Collections.Generic;
using BlockChainAnalysis.Models;

namespace BlockChainAnalysis.DataStructures.HashTable
{
    // Author: Yiğit Sağım
    // Açıklama: Proje gereklilikleri kapsamında Hash Table veri yapısı tarafımca kodlanmıştır.
    public class WalletHashTable
    {
        private LinkedList<(string Key, Wallet Value)>[] _buckets;
        private int _capacity;
        private int _count;
        private const double LoadFactorThreshold = 0.75;

        public WalletHashTable(int initialCapacity = 97)
        {
            _capacity = initialCapacity;
            _buckets = new LinkedList<(string, Wallet)>[_capacity];
            _count = 0;
        }

        private int HashFunction(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            int hash = 0;
            int p = 31;
            foreach (char c in key)
                unchecked { hash = hash * p + c; }
            return Math.Abs(hash) % _capacity;
        }

        public void Insert(string walletId, Wallet walletData)
        {
            if (walletId == null) throw new ArgumentNullException(nameof(walletId));
            if (walletData == null) throw new ArgumentNullException(nameof(walletData));
            if ((double)_count / _capacity >= LoadFactorThreshold) Rehash();
            int index = HashFunction(walletId);
            if (_buckets[index] == null)
                _buckets[index] = new LinkedList<(string, Wallet)>();
            var node = _buckets[index].First;
            while (node != null)
            {
                if (node.Value.Key == walletId)
                {
                    _buckets[index].Remove(node);
                    _buckets[index].AddFirst((walletId, walletData));
                    return;
                }
                node = node.Next;
            }
            _buckets[index].AddFirst((walletId, walletData));
            _count++;
        }

        public Wallet Search(string walletId)
        {
            if (walletId == null) throw new ArgumentNullException(nameof(walletId));
            int index = HashFunction(walletId);
            if (_buckets[index] == null) return null;
            foreach (var (key, value) in _buckets[index])
                if (key == walletId) return value;
            return null;
        }

        public void Delete(string walletId)
        {
            if (walletId == null) throw new ArgumentNullException(nameof(walletId));
            int index = HashFunction(walletId);
            if (_buckets[index] == null) return;
            var node = _buckets[index].First;
            while (node != null)
            {
                if (node.Value.Key == walletId)
                {
                    _buckets[index].Remove(node);
                    _count--;
                    return;
                }
                node = node.Next;
            }
        }

        public int Count => _count;

        private void Rehash()
        {
            int newCapacity = NextPrime(_capacity * 2);
            var newBuckets = new LinkedList<(string, Wallet)>[newCapacity];
            _capacity = newCapacity;
            foreach (var bucket in _buckets)
            {
                if (bucket == null) continue;
                foreach (var (key, value) in bucket)
                {
                    int newIndex = HashFunction(key);
                    if (newBuckets[newIndex] == null)
                        newBuckets[newIndex] = new LinkedList<(string, Wallet)>();
                    newBuckets[newIndex].AddFirst((key, value));
                }
            }
            _buckets = newBuckets;
        }

        private static int NextPrime(int min)
        {
            int candidate = min % 2 == 0 ? min + 1 : min;
            while (!IsPrime(candidate)) candidate += 2;
            return candidate;
        }

        private static bool IsPrime(int n)
        {
            if (n < 2) return false;
            if (n < 4) return true;
            if (n % 2 == 0 || n % 3 == 0) return false;
            for (int i = 5; i * i <= n; i += 6)
                if (n % i == 0 || n % (i + 2) == 0) return false;
            return true;
        }
    }
}
