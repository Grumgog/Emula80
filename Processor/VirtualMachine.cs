using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lexer;
using Processor;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows;
using System.IO;

namespace Proc
{
    public class EmuTokenizer : ITokenizer
    {
        public Token Tokenize(string token)
        {
            int num = 0;
            if (token != string.Empty)
            {
                if (VirtualMachine.operators.Contains<string>(token))
                    return new Token(TokenType.OPERATOR, token);
                else if (Processor.Registers.Contains<string>(token))
                    return new Token(TokenType.REGISTR, token);
                else if (int.TryParse(token, out num))
                    return new Token(TokenType.DATA, token);
                else if (Regex.IsMatch(token, "^[a-zA-Z]+$"))
                    return new Token(TokenType.LABEL, token);
                else if (Regex.IsMatch(token, "^[a-zA-Z]+:$"))
                    return new Token(TokenType.LABELTO, token);
                else
                    return new Token(TokenType.NONE, token);
            }
            else
                return Token.EMPTY_TOKEN;
        }
    }

    public class EmuDivider : ILexDivider
    {
        public bool IsDivide(char c)
        {
            return c == ' ' || c == ',' || c == '\n';
        }
    }

    public class RandomNumDevice : IInputDevice
    {
        Processor p;
        Random r;
        public RandomNumDevice(Processor processor)
        {
            p = processor;
            r = new Random();
        }

        public int Addres => 42;

        public void Input()
        {
            p.RegisterOfProcessor.ToList().Find(el => el.Key == "EAX").Value = r.Next();
        }
    }

    public class OutPrinter : IOutputDevice
    {
        List<int> buffer;
        public List<int> Buf { get => buffer; set => buffer = value; }

        public int Addres => 55;
        Processor Proc;
        public OutPrinter(Processor p)
        {
            Proc = p;
        }

        public void Flush()
        {
            string outbuf = "";
            foreach(var ch in Buf)
            {
                outbuf += (char)ch;
            }

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument fd = new FlowDocument(); // Печатаем потоковый документ
                Paragraph mainBlock = new Paragraph(); // У нас один параграф
                mainBlock.Margin = new Thickness(0);
                foreach(string line in outbuf.Split('\n')) // Заполняем параграф строками
                {
                    mainBlock.Inlines.Add(new Run(line));
                }
                fd.Blocks.Add(mainBlock);
                DocumentPaginator paginator = ((IDocumentPaginatorSource)fd).DocumentPaginator; // получаем листы
                printDialog.PrintDocument(paginator, "Printer OUTPUT"); // печатаем
            }
        }

        public void Print()
        {
            Buf.Add(Proc.RegisterOfProcessor.ToList().Find(el => el.Key == "EBX").Value);
            if (Proc.RegisterOfProcessor.ToList().Find(el => el.Key == "EAX").Value == 1)
                Flush();
        }
    }

    public class VirtualMachine
    {
        static public List<string> operators = new List<string>{ "MOV", "PUSH", "ADD", "SUB", "MUL", "DIV", "STORE",
                                                                "LOAD", "JMP", "CALL", "PROC", "ENDPROC", "EXCH", "CMP",
                                                                "JGT", "JLT", "JEQ", "JGE", "JLE", "JNE", "IN", "OUT"};
        public Processor proc { get; set; }

        public ObservableCollection<IInputDevice> InputDevices { get; set; }
        public ObservableCollection<IOutputDevice> OutputDevices { get; set; }

        public VirtualMachine(Processor proccesor)
        {
            proc = proccesor;
            InputDevices = new ObservableCollection<IInputDevice>();
            OutputDevices = new ObservableCollection<IOutputDevice>();
            // LoadDevices
            LoadDevices();
            

            // StandartDevices
            InputDevices.Add(new RandomNumDevice(proc));
            OutputDevices.Add(new OutPrinter(proc));
        }

        private void LoadDevices()
        {
            string PluggableFolder = StandartSettings.PluggableDevicesFolderPath;

            if (Directory.Exists(PluggableFolder))
            {
                if (Directory.Exists(StandartSettings.PluggableInputDevicesPath))
                    LoadInputDevices();
                else
                    _createSubInput();

                if (Directory.Exists(StandartSettings.PluggableOutputDevicesPath))
                    LoadOutputDevices();
                else
                    _createSubInput();
            }
            else
                _createSubFolder();
        }

        /// <summary>
        /// Загружает устройства ввода в виртуальную машину
        /// </summary>
        private void LoadInputDevices()
        {
            DirectoryInfo inf = new DirectoryInfo(StandartSettings.PluggableInputDevicesPath);
            var files = inf.GetFiles();
            var assembles = from asm in files
                            where (string.Compare(asm.Extension, ".EXE", true) == 0 || string.Compare(asm.Extension, ".DLL", true) == 0)
                            select asm;
            foreach(var asm in assembles)
            {
                Assembly assembly = Assembly.LoadFile(asm.FullName);
                var types = assembly.GetTypes();
                var inputtypes = from type in types
                        where type.GetInterface("IInputDevice") != null
                        select type;
                inputtypes.ToList().ForEach(el => InputDevices.Add((IInputDevice)Activator.CreateInstance(el)));
            }
        }

        /// <summary>
        /// Загружает устройства вывода в виртуальную машину
        /// </summary>
        private void LoadOutputDevices()
        {
            DirectoryInfo inf = new DirectoryInfo(StandartSettings.PluggableOutputDevicesPath);
            var files = inf.GetFiles();
            var assembles = from asm in files
                            where (string.Compare(asm.Extension, ".EXE", true) == 0 || string.Compare(asm.Extension, ".DLL", true) == 0)
                            select asm;
            foreach (var asm in assembles)
            {
                Assembly assembly = Assembly.LoadFile(asm.FullName);
                var types = assembly.GetTypes();
                var outputtypes = from type in types
                                  where type.GetInterface("IOutputDevice") != null
                                  select type;
                outputtypes.ToList().ForEach(el => OutputDevices.Add((IOutputDevice)Activator.CreateInstance(el)));
            }
        }

        /// <summary>
        /// Создает поддиректорию для загрузки модулей устройств
        /// </summary>
        private void _createSubFolder()
        {
            Directory.CreateDirectory(StandartSettings.PluggableDevicesFolderPath);
            _createSubInput();
            _createSubOutput();
        }

        private void _createSubInput() => Directory.CreateDirectory(StandartSettings.PluggableInputDevicesPath);

        private void _createSubOutput() => Directory.CreateDirectory(StandartSettings.PluggableOutputDevicesPath);

        /// <summary>
        /// Отдельный код для совершения прыжков
        /// </summary>
        /// <param name="stream">Поток токенов программы</param>
        /// <param name="pos">Позиция указателя на исполняемую инструкцию</param>
        /// <returns>Новое положение указателя на исполняемую инструкцию после совершения прыжка</returns>
        protected int JUMP(List<Token> stream, int pos)
        {
            int newPos = -1;
            if (stream[pos + 1].Type == TokenType.LABEL)
            {
                List<Token> labels = stream.FindAll(el => el.Type == TokenType.LABELTO && (string.Compare(el.Value, stream[pos + 1].Value + ":", true) == 0));

                if (labels.Count > 1)
                    throw new Exception($"Существует больше одной метки {stream[pos + 1].Value}");
                else if (labels.Count == 0)
                    throw new Exception($"Несущетвует метки в коде: {stream[pos + 1].Value}");
                else
                {
                    newPos = stream.FindIndex(el => el == labels[0]);
                }
            }
            return newPos;
        }

        /// <summary>
        /// Выполняет один смысловой оператор программы
        /// </summary>
        /// <param name="stream">Поток токенов созданный из текста программ</param>
        /// <param name="i">Указатель на инструкцию для выполнения</param>
        /// <returns>Новое значение указателя на инструкцию</returns>
        public int Step(List<Token> stream, int i)
        {
            proc.RegisterOfProcessor.ToList().Find(el => el.Key == "IAR").Value = i;
            proc.UpdateReg();// Заносим номер исполняемого оператора в регистор хранения адреса комманды
            if (stream[i].Type == TokenType.OPERATOR)
            {
                if (string.Compare(stream[i].Value, "ADD", true) == 0)
                {
                    Console.WriteLine(stream[i]);
                    if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.REGISTR)
                        proc.ADD(stream[i + 1].Value, stream[i + 2].Value);
                    else if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.DATA)
                        proc.ADD(stream[i + 1].Value, int.Parse(stream[i + 2].Value));
                    else
                        throw new Exception("not right command!");
                    i += 3;
                }
                else if (string.Compare(stream[i].Value, "SUB", true) == 0)
                {
                    Console.WriteLine(stream[i]);
                    if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.REGISTR)
                        proc.SUB(stream[i + 1].Value, stream[i + 2].Value);
                    else if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.DATA)
                        proc.SUB(stream[i + 1].Value, int.Parse(stream[i + 2].Value));
                    else
                        throw new Exception("not right command!");
                    i += 3;
                }
                else if (string.Compare(stream[i].Value, "MUL", true) == 0)
                {
                    Console.WriteLine(stream[i]);
                    if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.REGISTR)
                        proc.MUL(stream[i + 1].Value, stream[i + 2].Value);
                    else if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.DATA)
                        proc.MUL(stream[i + 1].Value, int.Parse(stream[i + 2].Value));
                    else
                        throw new Exception("not right command!");
                    i += 3;
                }
                else if (string.Compare(stream[i].Value, "DIV", true) == 0)
                {
                    Console.WriteLine(stream[i]);
                    if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.REGISTR)
                        proc.DIV(stream[i + 1].Value, stream[i + 2].Value);
                    else if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.DATA)
                        proc.DIV(stream[i + 1].Value, int.Parse(stream[i + 2].Value));
                    else
                        throw new Exception("not right command!");
                    i += 3;
                }
                else if (string.Compare(stream[i].Value, "PUSH", true) == 0)
                {
                    Console.WriteLine(stream[i]);
                    if (stream[i + 1].Type == TokenType.REGISTR)
                        proc.PUSH(stream[i + 1].Value);
                    else if (stream[i + 1].Type == TokenType.DATA)
                        proc.PUSH(int.Parse(stream[i + 2].Value));
                    else
                        throw new Exception("not right command!");
                    i += 2;
                }
                else if (string.Compare(stream[i].Value, "POP", true) == 0)
                {
                    Console.WriteLine(stream[i]);
                    if (stream[i + 1].Type == TokenType.REGISTR)
                        proc.POP(stream[i + 1].Value);
                    else
                        throw new Exception("not right command!");
                    i += 2;
                }
                else if (string.Compare(stream[i].Value, "LOAD", true) == 0)
                {
                    Console.WriteLine(stream[i]);
                    if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.DATA)
                        proc.Load(stream[i + 1].Value, int.Parse(stream[i + 2].Value));
                    else if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.REGISTR)
                        proc.Load(stream[i + 1].Value, stream[i + 2].Value);
                    else
                        throw new Exception("not right command!");
                    i += 3;
                }
                else if (string.Compare(stream[i].Value, "STORE", true) == 0)
                {
                    Console.WriteLine(stream[i]);
                    if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.DATA)
                        proc.Store(stream[i + 1].Value, int.Parse(stream[i + 2].Value));
                    else if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.REGISTR)
                        proc.Store(stream[i + 1].Value, stream[i + 2].Value);
                    else
                        throw new Exception("not right command!");
                    i += 3;
                }
                else if (string.Compare(stream[i].Value, "EXCH", true) == 0)
                {
                    Console.WriteLine(stream[i]);
                    if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.REGISTR)
                        proc.EXCH(stream[i + 1].Value, stream[i + 2].Value);
                    else
                        throw new Exception("not right command!");
                    i += 3;
                }
                else if (string.Compare(stream[i].Value, "MOV", true) == 0)
                {
                    Console.WriteLine(stream[i] + " to mov inst");
                    if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.REGISTR)
                        proc.MOV(stream[i + 1].Value, stream[i + 2].Value);
                    else if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.DATA)
                        proc.MOV(stream[i + 1].Value, int.Parse(stream[i + 2].Value));
                    else
                        throw new Exception("not right command!");
                    i += 3;
                }
                else if (string.Compare(stream[i].Value, "CMP", true) == 0)
                {
                    if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.REGISTR)
                        proc.CMP(stream[i + 1].Value, stream[i + 2].Value);
                    else
                        throw new Exception("CMP compare only registers!");
                    i += 3;
                }
                else if (string.Compare(stream[i].Value, "JMP", true) == 0)
                {
                    int newpos = JUMP(stream, i);
                    if (newpos != -1)
                        i = newpos;
                }
                else if (string.Compare(stream[i].Value, "JGT", true) == 0)
                {
                    // Условный прыжок, если больше
                    if (proc.RegisterOfProcessor.ToList().Find(el => el.Key == "CRF").Value == 1)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                    else
                        i += 2;
                }
                else if (string.Compare(stream[i].Value, "JLT", true) == 0)
                {
                    if (proc.RegisterOfProcessor.ToList().Find(el => el.Key == "CRF").Value == -1)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                    else
                        i += 2;
                }
                else if (string.Compare(stream[i].Value, "JEQ", true) == 0)
                {
                    if (proc.RegisterOfProcessor.ToList().Find(el => el.Key == "CRF").Value == 0)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                    else
                        i += 2;
                }
                else if (string.Compare(stream[i].Value, "JGE", true) == 0)
                {
                    if (proc.RegisterOfProcessor.ToList().Find(el => el.Key == "CRF").Value >= 0)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                    else
                        i += 2;
                }
                else if (string.Compare(stream[i].Value, "JLE", true) == 0)
                {
                    if (proc.RegisterOfProcessor.ToList().Find(el => el.Key == "CRF").Value <= 0)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                    else
                        i += 2;
                }
                else if (string.Compare(stream[i].Value, "JNE", true) == 0)
                {
                    if (proc.RegisterOfProcessor.ToList().Find(el => el.Key == "CRF").Value != 0)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                    else
                        i += 2;
                }
                else if (string.Compare(stream[i].Value, "PROC", true) == 0)
                {
                    Console.WriteLine("PROC begins!!");
                    if (stream[i + 1].Type == TokenType.LABELTO)
                    {
                        i += 2; // пропустить proc и label
                        while (string.Compare(stream[i].Value, "ENDPROC", true) != 0) // пропускаем и не исполняем операторы
                        {
                            if (i == stream.Count)
                                throw new Exception("Нет конца начатой процедуры");
                            else if (stream[i].Type == TokenType.OPERATOR && stream[i].Value == "PROC")
                                throw new Exception("Вложенные процедуры, ЭТО НЕ ДОПУСТИМО!");
                            i++;
                        }
                        i++; // переходим на следующий оператор после endproc
                    }
                    else
                        throw new Exception("Нужна метка перехода для обозначения имени функции!");
                }
                else if (string.Compare(stream[i].Value, "ENDPROC", true) == 0)
                {
                    if (proc.RegisterOfProcessor.ToList().Find(el => el.Key == "RET").Value != -1)
                    {
                        i = proc.RegisterOfProcessor.ToList().Find(el => el.Key == "RET").Value;
                        proc.RegisterOfProcessor.ToList().Find(el => el.Key == "RET").Value = -1;
                        proc.UpdateReg();
                    }
                    else
                        throw new Exception($"ENDPROC без соответствующей PROC! : {stream[i]}");
                }
                else if (string.Compare(stream[i].Value, "CALL", true) == 0)
                {
                    if (stream[i + 1].Type == TokenType.LABEL)
                    {
                        var findLabels = stream.FindAll(el => el.Type == TokenType.LABELTO && (string.Compare(el.Value, stream[i + 1].Value + ":", true) == 0));
                        if (findLabels.Count > 1)
                            throw new Exception($"Больше одной процедуры используют данное имя: {stream[i + 1]}");
                        else if (findLabels.Count == 0)
                            throw new Exception($"Нет процедуры по имени  {stream[i + 1]}");
                        else
                        {
                            proc.RegisterOfProcessor.ToList().Find(el => el.Key == "RET").Value = i + 2; // Адресс возрата следущая комманда
                            i = stream.IndexOf(findLabels[0]) + 1; // переходим на первую коммманду процедуры
                            proc.UpdateReg();
                        }

                    }
                }
                else if(string.Compare(stream[i].Value, "IN", true) == 0)
                {
                    if(stream[i+1].Type == TokenType.DATA)
                    {
                        int adress = int.Parse(stream[i + 1].Value);
                        InputDevices.ToList().Find(el => el.Addres == adress).Input();
                    }
                    else if(stream[i+1].Type == TokenType.REGISTR)
                    {
                        int adress = proc.RegisterOfProcessor.ToList().Find(el => el.Key == stream[i + 1].Value).Value;
                        InputDevices.ToList().Find(el => el.Addres == adress).Input();
                    }
                    i += 2;
                }
                else if (string.Compare(stream[i].Value, "OUT", true) == 0)
                {
                    if (stream[i + 1].Type == TokenType.DATA)
                    {
                        int adress = int.Parse(stream[i + 1].Value);
                        OutputDevices.ToList().Find(el => el.Addres == adress).Print();
                    }
                    else if (stream[i + 1].Type == TokenType.REGISTR)
                    {
                        int adress = proc.RegisterOfProcessor.ToList().Find(el => el.Key == stream[i + 1].Value).Value;
                        OutputDevices.ToList().Find(el => el.Addres == adress).Print();
                    }
                    i += 2;
                }
            }
            else if (stream[i].Type == TokenType.LABELTO)
            {
                //ignore label
                i++;
            }
            else
                throw new Exception("Комманда начинается не с оператора! Начните команду с оператора пожалуйста " + stream[i]);

            return i;
        }


        /// <summary>
        /// Исполняет программу, беря информацию из токенов
        /// </summary>
        /// <param name="stream"></param>
        public void Execute(List<Token> stream)
        {
            int i = 0;
            while (i < stream.Count)
                i = Step(stream, i);
        }
    }
}
