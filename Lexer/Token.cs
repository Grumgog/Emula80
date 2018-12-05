
namespace Lexer
{
    public enum TokenType { DATA, OPERATOR, REGISTR, LABEL, LABELTO, NONE};


    public class Token
    {
        public static readonly string EMPTY_TOKEN_VALUE = string.Empty;

        public static readonly Token EMPTY_TOKEN = new Token();

        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token()
        {
            Type = TokenType.NONE;
            Value = EMPTY_TOKEN_VALUE;
        }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public Token(Token value)
        {
            Type = value.Type;
            Value = value.Value;
        }

        public override string ToString() => $"({Type}, {Value})";
    }

    public interface ITokenizer
    {
        Token Tokenize(string token);
    }
}
