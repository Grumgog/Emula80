using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Proc
{
    class UsualMemoryDevice : IMemoryDevice
    {
         
        public ObservableCollection<int> FlatMemory { get; set; } // simple flat model of memory

        public int Addres => -1; // Устоойство памяти (поэтому -1)
        public string Name => "Usual Memory Device";
        public int[] InMem => FlatMemory.ToArray();

        public UsualMemoryDevice(int size)
        {
            FlatMemory = new ObservableCollection<int>();
            for (int i = 0; i < 128; i++) FlatMemory.Add(0);
        }

        public int GetCell(int cell)
        {
            if (cell < FlatMemory.Count && cell >= 0)
                return FlatMemory[cell];
            else throw new IndexOutOfRangeException("Выход за пределы памяти устройства хранения");
        }

        public void SetCell(int cell, int data)
        {
            if (cell < FlatMemory.Count && cell>=0)
                FlatMemory[cell] = data;
            else throw new IndexOutOfRangeException("Выход за пределы памяти устройства хранения");
        }

        public int SizeMemory() => FlatMemory.Count;
    }
}
