using System.Collections.Generic;

namespace Lexer
{
    public class Analizator
    {
        private ILexDivider Divider { get; set; }
        private List<ITokenizer> Tokenizers { get; set; }

        public Analizator(ILexDivider divider, ITokenizer tokenizer)
        {
            Divider = divider;
            Tokenizers = new List<ITokenizer>{ tokenizer };     
        }

        public Analizator(ILexDivider divider, List<ITokenizer> tokenizers)
        {
            Divider = divider;
            Tokenizers = tokenizers;
        }

        public void AddTokenizer(ITokenizer tokenizer) => Tokenizers.Add(tokenizer);

        public List<string> Divide(string text)
        {
            string subLex = string.Empty;
            List<string> result = new List<string>();
            int begin = 0;
            for(int i = 0; i < text.Length; i++)
            {
                if (Divider.IsDivide(text[i]))
                {
                    subLex = text.Substring(begin, i - begin);
                    subLex = subLex.Trim().Trim(' ');
                    begin = i;
                    if (subLex != string.Empty)
                        result.Add(subLex);
                }
            }
            string buf = text.Substring(begin);
            buf = buf.Trim().Trim(' ');
            if (buf != string.Empty) result.Add(buf);
            return result;
        }

        public List<Token> Tokenize(List<string> lexs)
        {
            List<Token> result = new List<Token>();
            foreach(string lex in lexs)
            {
                foreach(ITokenizer t in Tokenizers)
                {
                    Token newToken = t.Tokenize(lex);
                    if(newToken != Token.EMPTY_TOKEN)
                    {
                        result.Add(newToken);
                        break;
                    }
                }
            }
            return result;
        }
        public List<Token> Process(string text) => Tokenize(Divide(text));
    }  
}
