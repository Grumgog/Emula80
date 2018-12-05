using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proc
{
    public class Processor : IProccesor
    {
        public static readonly List<string> Registers = new List<string>{"EAX", "EBX", "ECX", "EDX", "EFX", "EGX", "ESX", "EPX", "INP", "RET", "DRF", "STE", "CRF" };
        IMemoryDevice memoryDevice;
        int StackSize;


        public Dictionary<string, int> RegisterOfProcessor { get; set; }



        public Processor()
        {
            memoryDevice = new UsualMemoryDevice(128); // 128 elements for example
            RegisterOfProcessor = new Dictionary<string, int>();
            foreach (string reg in Registers)
                RegisterOfProcessor[reg] = 0;
            StackSize = memoryDevice.SizeMemory() / 2;
        }


        public int GetMemCell(int cell) => memoryDevice.GetCell(cell);


        public int GetMemorySize() => memoryDevice.SizeMemory();

        public void Load(string Register, int memory)
        {
            RegisterOfProcessor[Register] = memoryDevice.GetCell(memory);
        }

        public void Load(string first, string second)
        {
            RegisterOfProcessor[first] = memoryDevice.GetCell(RegisterOfProcessor[second]);
        }

        public void SetMemoryStorage(IMemoryDevice memoryDevice) => this.memoryDevice = memoryDevice;

        public void Store(string Register, int memory)
        {
            memoryDevice.SetCell(memory, RegisterOfProcessor[Register]);
        }

        public void Store(string fisrt, string second)
        {
            memoryDevice.SetCell(RegisterOfProcessor[second], RegisterOfProcessor[fisrt]);
        }

        #region Система комманд процессора
        public void ADD(string first, string second) => RegisterOfProcessor[first] += RegisterOfProcessor[second];
        public void ADD(string first, int data) => RegisterOfProcessor[first] += data;

        public void SUB(string first, string second) => RegisterOfProcessor[first] += RegisterOfProcessor[second];
        public void SUB(string first, int data) => RegisterOfProcessor[first] += data;

        public void DIV(string first, string second) => RegisterOfProcessor[first] += RegisterOfProcessor[second];
        public void DIV(string first, int data) => RegisterOfProcessor[first] += data;

        public void MUL(string first, string second) => RegisterOfProcessor[first] += RegisterOfProcessor[second];
        public void MUL(string first, int data) => RegisterOfProcessor[first] += data;

        public void PUSH(string data)
        {
            memoryDevice.SetCell(RegisterOfProcessor["EPX"], RegisterOfProcessor[data]);
            RegisterOfProcessor["EPX"] += 1;
        }

        public void PUSH(int data)
        {
            memoryDevice.SetCell(RegisterOfProcessor["EPX"], data);
            RegisterOfProcessor["EPX"] += 1;
        }

        public void POP(string reg)
        {
            RegisterOfProcessor[reg] += memoryDevice.GetCell(RegisterOfProcessor["EPX"]);
            RegisterOfProcessor["EPX"] -= 1;
        }
        
        public void EXCH(string first, string second)
        {
            var buf = RegisterOfProcessor[first];
            RegisterOfProcessor[first] = RegisterOfProcessor[second];
            RegisterOfProcessor[second] = buf;
        }

        public void MOV(string first, int data) => RegisterOfProcessor[first] = data;
        public void MOV(string first, string second) => RegisterOfProcessor[first] = RegisterOfProcessor[second];

        public void CMP(string first, string second)
        {
            int res = RegisterOfProcessor[first] - RegisterOfProcessor[second];
            if (res > 0)
                RegisterOfProcessor["CRF"] = 1;
            else if (res == 0)
                RegisterOfProcessor["CRF"] = 0;
            else
                RegisterOfProcessor["CRF"] = -1;
        }
        #endregion
    }
}
