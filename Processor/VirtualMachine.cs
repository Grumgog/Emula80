using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer;
using Proc;

namespace Proc
{

    public class VirtualMachine
    {
        public Processor proc { get; set; }

        public VirtualMachine(Processor proccesor)
        {
            proc = proccesor;
        }

        /// <summary>
        /// Отдельный коди для совершения прыжков
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
                    if (proc.RegisterOfProcessor["CRF"] == 1)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                }
                else if (string.Compare(stream[i].Value, "JLT", true) == 0)
                {
                    if (proc.RegisterOfProcessor["CRF"] == -1)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                }
                else if (string.Compare(stream[i].Value, "JEQ", true) == 0)
                {
                    if (proc.RegisterOfProcessor["CRF"] == 0)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                }
                else if (string.Compare(stream[i].Value, "JGE", true) == 0)
                {
                    if (proc.RegisterOfProcessor["CRF"] >= 0)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                }
                else if (string.Compare(stream[i].Value, "JLE", true) == 0)
                {
                    if (proc.RegisterOfProcessor["CRF"] <= 0)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
                }
                else if (string.Compare(stream[i].Value, "JNE", true) == 0)
                {
                    if (proc.RegisterOfProcessor["CRF"] != 0)
                    {
                        int newpos = JUMP(stream, i);
                        if (newpos != -1)
                            i = newpos;
                    }
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
                    if (proc.RegisterOfProcessor["RET"] != -1)
                    {
                        i = proc.RegisterOfProcessor["RET"];
                        proc.RegisterOfProcessor["RET"] = -1;
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
                            proc.RegisterOfProcessor["RET"] = i + 2; // Адресс возрата следущая комманда
                            i = stream.IndexOf(findLabels[0]) + 1; // переходим на первую коммманду процедуры
                        }

                    }
                }
            }
            else if (stream[i].Type == TokenType.LABELTO)
            {
                //ignore label
                i++;
            }
            else
                throw new Exception("Сия комманда начинается не с оператора! Начните команду с оператора пожалуйста " + stream[i]);

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
