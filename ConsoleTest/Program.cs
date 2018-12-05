using System;
using System.Collections.Generic;
using System.Linq;
using Proc;
using Lexer;
using System.Text.RegularExpressions;

namespace ConsoleTest
{
    class SimpleDivider : ILexDivider
    {
        public bool IsDivide(char c)
        {
            return c == ' ' || c == ',' || c == '\n';   
        }
    }

    class SimpleTokenizer : ITokenizer
    {
        public Token Tokenize(string token)
        {
            int num = 0;
            if (token != string.Empty)
            {
                if (Program.operators.Contains<string>(token))
                    return new Token(TokenType.OPERATOR, token);
                else if (Program.REGS.Contains<string>(token))
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

    class Program
    {

        static public List<String> operators = new List<string>{ "MOV", "PUSH", "ADD", "SUB", "MUL", "DIV", "STORE",
                                                                "LOAD", "JMP", "CALL", "PROC", "ENDPROC", "EXCH", "CMP",
                                                                "JGT", "JLT", "JEQ", "JGE", "JLE", "JNE"};
        static public List<string> REGS = Processor.Registers;
        static void Main(string[] args)
        {
            Analizator analizator = new Analizator(new SimpleDivider(), new SimpleTokenizer());
            List<Token> l = analizator.Process("PROC TOECX:\n" +
                                               "MOV ECX, 124\n" +
                                               "ENDPROC\n" +
                                               "MOV EAX, 2024\n" +
                                               "MOV EBX, 1024\n" +
                                               "ADD EBX, EAX\n" +
                                               "CMP EAX, EBX\n" +
                                               "JLT next\n" +
                                               "MOV ECX, 500\n" +
                                               "next:\n" +
                                               "ADD EDX, 300\n" +
                                               "CALL TOECX\n" +
                                               "STORE ECX, 1\n");
            l.ForEach(el => Console.WriteLine(el));
            Console.WriteLine("Закончили создавать токены.... Нажмите любую клавишу...");
            Console.ReadKey();

            Console.WriteLine("Создаем процессор");
            Processor processor = new Processor();
            Console.WriteLine("Создаем виртуальную машину");
            VirtualMachine machine = new VirtualMachine(processor);
            Console.WriteLine("Компилируем токены в код");
            l = l.FindAll(el => el.Type != TokenType.NONE);
            Console.WriteLine("Выполняем");
            //machine.Execute(l);
            int i = 0;
            while (i < l.Count)
               i = machine.Step(l, i);
            Console.WriteLine("Закончили выполнять.... Нажмите любую клавишу...");
            Console.ReadKey();

        }

    }
}
