using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Proc
{
    public class Pair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public Pair(TKey k, TValue v)
        {
            Key = k;
            Value = v;
        }
    }
    
    public class Processor : IProccesor
    {
        public static readonly List<string> Registers = new List<string>{"EAX", "EBX", "ECX", "EDX", "EFX", "EGX", "ESX", "EPX", "INP", "RET", "DRF", "STE", "CRF", "IAR" };
        IMemoryDevice memoryDevice;
        int StackSize;


        public ObservableCollection<Pair<string, int>> RegisterOfProcessor { get; set; }
        public ObservableCollection<int> Memory { get => (memoryDevice as UsualMemoryDevice).FlatMemory; }


        public Processor()
        {
            memoryDevice = new UsualMemoryDevice(128); // 128 elements for example
            RegisterOfProcessor = new ObservableCollection<Pair<string, int>>();
            foreach (string reg in Registers)
                RegisterOfProcessor.Add(new Pair<string, int>(reg, 0));
            StackSize = memoryDevice.SizeMemory() / 2;
            RegisterOfProcessor.ToList().Find(el => el.Key == "RET").Value = -1;
            UpdateReg();
        }

        public void UpdateReg() // Для того что бы коллекция послала сигналы о том что надо обновится
        {
            var buf = RegisterOfProcessor.ToList();
            RegisterOfProcessor.Clear();
            foreach(var b in buf)
            {
                RegisterOfProcessor.Add(b);
            }
        }


        public int GetMemCell(int cell) => memoryDevice.GetCell(cell);


        public int GetMemorySize() => memoryDevice.SizeMemory();

        public void Load(string Register, int memory)
        {
            RegisterOfProcessor.ToList().Find(el=>el.Key == Register).Value = memoryDevice.GetCell(memory);
            UpdateReg();
        }

        public void Load(string first, string second)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value
                = memoryDevice.GetCell(RegisterOfProcessor.ToList().Find(el => el.Key == second).Value);
            UpdateReg();
        }

        public void SetMemoryStorage(IMemoryDevice memoryDevice) => this.memoryDevice = memoryDevice;

        public void Store(string Register, int memory)
        {
            memoryDevice.SetCell(memory, RegisterOfProcessor.ToList().Find(el=>el.Key == Register).Value);
        }

        public void Store(string first, string second)
        {
            memoryDevice.SetCell(RegisterOfProcessor.ToList().Find(el => el.Key == second).Value,
                RegisterOfProcessor.ToList().Find(el => el.Key == first).Value);
        }

        #region Система комманд процессора
        public void ADD(string first, string second)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value += RegisterOfProcessor.ToList().Find(el => el.Key == second).Value;
            UpdateReg();
        }
        public void ADD(string first, int data)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value += data;
            UpdateReg();
        }

        public void SUB(string first, string second)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value -= RegisterOfProcessor.ToList().Find(el => el.Key == second).Value;
            UpdateReg();
        }

        public void SUB(string first, int data)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value -= data;
            UpdateReg();
        }

        public void DIV(string first, string second)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value /= RegisterOfProcessor.ToList().Find(el => el.Key == second).Value;
            UpdateReg();
        }

        public void DIV(string first, int data)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value /= data;
            UpdateReg();
        }

        public void MUL(string first, string second)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value *= RegisterOfProcessor.ToList().Find(el => el.Key == second).Value;
            UpdateReg();
        }

        public void MUL(string first, int data)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value *= data;
            UpdateReg();
        }

        public void PUSH(string data)
        {
            memoryDevice.SetCell(RegisterOfProcessor.ToList().Find(el => el.Key == "EPX").Value, RegisterOfProcessor.ToList().Find(el => el.Key == data).Value);
            RegisterOfProcessor.ToList().Find(el => el.Key == "EPX").Value += 1;
            UpdateReg();
        }

        public void PUSH(int data)
        {
            memoryDevice.SetCell(RegisterOfProcessor.ToList().Find(el => el.Key == "EPX").Value, data);
            RegisterOfProcessor.ToList().Find(el => el.Key == "EPX").Value += 1;
            UpdateReg();
        }

        public void POP(string reg)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == reg).Value += memoryDevice.GetCell(RegisterOfProcessor.ToList().Find(el => el.Key == "EPX").Value);
            RegisterOfProcessor.ToList().Find(el => el.Key == "EPX").Value -= 1;
            UpdateReg();
        }
        
        public void EXCH(string first, string second)
        {
            var buf = RegisterOfProcessor.ToList().Find(el => el.Key == first).Value;
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value
                = RegisterOfProcessor.ToList().Find(el => el.Key == second).Value;
            RegisterOfProcessor.ToList().Find(el => el.Key == second).Value = buf;
            UpdateReg();
        }

        public void MOV(string first, int data)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value = data;
            UpdateReg();
        }

        public void MOV(string first, string second)
        {
            RegisterOfProcessor.ToList().Find(el => el.Key == first).Value = RegisterOfProcessor.ToList().Find(el => el.Key == second).Value;
            UpdateReg();
        }

        public void CMP(string first, string second)
        {
            int res = RegisterOfProcessor.ToList().Find(el => el.Key == first).Value - RegisterOfProcessor.ToList().Find(el => el.Key == second).Value;
            if (res > 0)
                RegisterOfProcessor.ToList().Find(el => el.Key == "CRF").Value = 1;
            else if (res == 0)
                RegisterOfProcessor.ToList().Find(el => el.Key == "CRF").Value = 0;
            else
                RegisterOfProcessor.ToList().Find(el => el.Key == "CRF").Value = -1;
            UpdateReg();
        }

        public ObservableCollection<int> getFlatPresentOfMemory()
        {
            return (memoryDevice as UsualMemoryDevice).FlatMemory;
        }
        #endregion
    }
}
