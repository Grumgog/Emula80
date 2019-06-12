using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proc
{
    public interface IDevice
    {
        int Addres { get; } // адрес устройства в памяти
        string Name { get; } // Имя устройства
    }

    
    public interface IMemoryDevice : IDevice
    {
        int GetCell(int cell);
        void SetCell(int cell, int data);
        int SizeMemory();
    }

    public interface IInputDevice : IDevice
    {
        void Input(); // Одиночный ввод
    }

    public interface IOutputDevice : IDevice
    {
        List<int> Buf { get; set; }
        void Flush(); // Принудительный вывод информации из буфера на устройство вывода
        void Print(); // Добавляет информацию в буфер (но не выводит)
    }

    public interface IIODevice : IInputDevice, IOutputDevice
    {
        bool ModeIO { get; set; }
    }
}