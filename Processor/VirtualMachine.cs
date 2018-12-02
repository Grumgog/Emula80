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

        public void Execute(List<Token> stream)
        {
            int i = 0;
            while (i < stream.Count)
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
                        continue;
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
                        continue;
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
                        continue;
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
                        continue;
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
                        continue;
                    }
                    else if (string.Compare(stream[i].Value, "POP", true) == 0)
                    {
                        Console.WriteLine(stream[i]);
                        if (stream[i + 1].Type == TokenType.REGISTR)
                            proc.POP(stream[i + 1].Value);
                        else
                            throw new Exception("not right command!");
                        i += 2;
                        continue;
                    }
                    else if (string.Compare(stream[i].Value, "LOAD", true) == 0)
                    {
                        Console.WriteLine(stream[i]);
                        if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.DATA)
                            proc.Load(stream[i + 1].Value, int.Parse(stream[i + 2].Value));
                        else
                            throw new Exception("not right command!");
                        i += 3;
                        continue;
                    }
                    else if (string.Compare(stream[i].Value, "STORE", true) == 0)
                    {
                        Console.WriteLine(stream[i]);
                        if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.DATA)
                            proc.Store(stream[i + 1].Value, int.Parse(stream[i + 2].Value));
                        else
                            throw new Exception("not right command!");
                        i += 3;
                        continue;
                    }
                    else if (string.Compare(stream[i].Value, "EXCH", true) == 0)
                    {
                        Console.WriteLine(stream[i]);
                        if (stream[i + 1].Type == TokenType.REGISTR && stream[i + 2].Type == TokenType.REGISTR)
                            proc.EXCH(stream[i + 1].Value, stream[i + 2].Value);
                        else
                            throw new Exception("not right command!");
                        i += 3;
                        continue;
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
                        continue;
                    }
                }
                else
                    throw new Exception("Сия комманда начинается не с оператора! Начните команду с оператора пожалуйста " + stream[i]);
            }
        }
    }
}
