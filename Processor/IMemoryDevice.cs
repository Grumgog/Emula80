using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proc
{
    public interface IMemoryDevice
    {
        int GetCell(int cell);
        void SetCell(int cell, int data);
        int SizeMemory();
    }
}
