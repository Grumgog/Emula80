using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proc
{
    class UsualMemoryDevice : IMemoryDevice
    {

        int[] FlatMemory; // simple flat model of memory

        public UsualMemoryDevice(int size)
        {
            FlatMemory = new int[size];
        }

        public int GetCell(int cell)
        {
            if (cell < FlatMemory.Length)
                return FlatMemory[cell];
            else throw new IndexOutOfRangeException("Выход за пределы памяти устройства хранения");
        }

        public void SetCell(int cell, int data)
        {
            if (cell < FlatMemory.Length)
                FlatMemory[cell] = data;
            else throw new IndexOutOfRangeException("Выход за пределы памяти устройства хранения");
        }

        public int SizeMemory() => FlatMemory.Length;
    }
}
