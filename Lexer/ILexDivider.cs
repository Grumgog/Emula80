
using System.Collections.Generic;

namespace Lexer
{
    public interface ILexDivider
    {
        bool IsDivide(char c);
    }

    public interface ISynAn
    {
        bool IsCorrect(List<Token> stream);
    }
}
