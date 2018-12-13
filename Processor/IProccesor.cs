using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Proc
{
    public interface IProccesor
    {
        int GetMemorySize();
        int GetMemCell(int cell);
        void SetMemoryStorage(IMemoryDevice memoryDivece);
        void Store(string register, int memory);
        void Load(string register, int memory);
        ObservableCollection<int> getFlatPresentOfMemory();
    }
}
