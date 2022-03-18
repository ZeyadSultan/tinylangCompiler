using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Identifier, DatatypeInt, DatatypeFloat, DatatypeString, LeftParentheses, RightParentheses,
    Comma, LeftBraces, RightBraces, Repeat, Assign, Until, SemiColon, Number,
    Return, Write, Read, EndLine, Plus, Division, Multiply, Minuse, LessThan, GreaterThan,
    NotEqual, End, If, ElseIf, Else, Then, EqualOp, Comment, String, Dot, AndOp, OrOp, main
}
namespace TINY_Compiler
{

    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("else if", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("endl", Token_Class.EndLine);
            ReservedWords.Add("int", Token_Class.DatatypeInt);
            ReservedWords.Add("float", Token_Class.DatatypeFloat);
            ReservedWords.Add("string", Token_Class.DatatypeString);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("main", Token_Class.main);

            Operators.Add(":=", Token_Class.Assign);
            Operators.Add(";", Token_Class.SemiColon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LeftBraces);
            Operators.Add(")", Token_Class.RightBraces);
            Operators.Add("{", Token_Class.LeftParentheses);
            Operators.Add("}", Token_Class.RightParentheses);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThan);
            Operators.Add(">", Token_Class.GreaterThan);
            Operators.Add("<>", Token_Class.NotEqual);
            Operators.Add("+", Token_Class.Plus);
            Operators.Add("-", Token_Class.Minuse);
            Operators.Add("*", Token_Class.Multiply);
            Operators.Add("/", Token_Class.Division);
            Operators.Add(".", Token_Class.Dot);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
        }

        public void StartScanning(string SourceCode)
        {
            // i: Outer loop to check on lexemes.
            for (int i = 0; i < SourceCode.Length; i++)
            {
                // j: Inner loop to check on each character in a single lexeme.
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = "";
                Token Tok = new Token();

                if (char.IsWhiteSpace(CurrentChar))
                {
                    continue;
                }
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;
                //single char token ;

                if (char.IsLetter(SourceCode[j]))
                {
                    while (true)
                    {
                        if (j == SourceCode.Length)
                        {
                            break;
                        }

                        else if (!char.IsLetterOrDigit(SourceCode[j]))
                        {

                            if (FindTokenClass(CurrentLexeme))
                            {

                                CurrentLexeme = "";
                                break;
                            }
                        }
                        else if (char.IsLetterOrDigit(SourceCode[j]))
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }

                    }

                    i = j - 1;
                    if (!FindTokenClass(CurrentLexeme) && CurrentLexeme != "")
                    {
                        Errors.Error_List.Add(CurrentLexeme);
                    }
                }//end if
                //ana akhadt copy mn henaaa paste HENA AWL HAGA GHAYRTAHA
                else if (SourceCode[j] == '.' || SourceCode[j] == '!')
                {
                    CurrentLexeme += SourceCode[j];
                    j++;

                    Errors.Error_List.Add(CurrentLexeme);
                    CurrentLexeme = "";
                    i = j - 1;
                }
                else if (char.IsDigit(SourceCode[j]))
                {
                    int count = 0;
                    while (true)
                    {
                        if (j == SourceCode.Length)
                        {
                            break;
                        }
                        else if (char.IsDigit(SourceCode[j]))
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else if (SourceCode[j] == '.')
                        {
                            count++;
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else if (!char.IsDigit(SourceCode[j]))
                        {
                            if (count > 1)
                            {
                                Errors.Error_List.Add(CurrentLexeme);
                                CurrentLexeme = "";
                                break;
                            }
                            if (FindTokenClass(CurrentLexeme))
                            {
                                CurrentLexeme = "";
                                break;
                            }
                        }
                    }
                    if (!FindTokenClass(CurrentLexeme) && CurrentLexeme != "")
                    {
                        Errors.Error_List.Add(CurrentLexeme);
                    }
                    i = j - 1;
                }

                /*condition that control the comment statement */
                else if (SourceCode[j] == '/')
                {
                    while (true)
                    {
                        if (j == SourceCode.Length)
                        {
                            break;
                        }
                        else if (SourceCode[j] == '/')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                            if (j == SourceCode.Length)
                            {
                                break;
                            }
                            else if (SourceCode[j] == '*')
                            {
                                CurrentLexeme += SourceCode[j];
                                j++;
                            }
                        }
                        if (FindTokenClass(CurrentLexeme))
                        {
                            CurrentLexeme = "";
                            break;
                        }
                        else
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                    }
                    if (!FindTokenClass(CurrentLexeme) && CurrentLexeme != "")
                    {
                        Errors.Error_List.Add(CurrentLexeme);
                    }
                    i = j - 1;
                }
                // MN HENA HA7OT CODE ZEYADAA 
                else if (CurrentChar == '&')
                {
                    while (true)
                    {
                        if (j == SourceCode.Length)
                        {
                            break;
                        }
                        else if (FindTokenClass(CurrentLexeme))
                        {
                            CurrentLexeme = "";
                            break;
                        }
                        else if (SourceCode[j + 1] == '&')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                            break;
                        }
                        if (FindTokenClass(CurrentLexeme))
                        {
                            CurrentLexeme = "";
                            break;
                        }
                    }
                    if (!FindTokenClass(CurrentLexeme) && CurrentLexeme != "")
                    {
                        Errors.Error_List.Add(CurrentLexeme);
                    }
                    i = j - 1;
                }
                else if (CurrentChar == '|')
                {
                    while (true)
                    {
                        if (j == SourceCode.Length)
                        {
                            break;
                        }
                        else if (FindTokenClass(CurrentLexeme))
                        {
                            CurrentLexeme = "";
                            break;
                        }
                        else if (SourceCode[j + 1] == '|')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                            break;
                        }
                        if (FindTokenClass(CurrentLexeme))
                        {
                            CurrentLexeme = "";
                            break;
                        }
                    }
                    if (!FindTokenClass(CurrentLexeme) && CurrentLexeme != "")
                    {
                        Errors.Error_List.Add(CurrentLexeme);
                    }
                    i = j - 1;
                }
                else if (SourceCode[j] == '<')
                {
                    while (true)
                    {
                        if (j == SourceCode.Length)
                        {
                            break;
                        }
                        else if (SourceCode[j] == '<')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                            if (j == SourceCode.Length)
                            {
                                break;
                            }
                            else if (SourceCode[j] == '>')
                            {
                                CurrentLexeme += SourceCode[j];
                                j++;
                            }
                            if (char.IsWhiteSpace(CurrentChar))
                            {
                                continue;
                            }
                            if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                                continue;
                        }
                        if (FindTokenClass(CurrentLexeme))
                        {
                            CurrentLexeme = "";
                            break;
                        }
                        else
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                    }
                    if (!FindTokenClass(CurrentLexeme) && CurrentLexeme != "")
                    {
                        Errors.Error_List.Add(CurrentLexeme);
                    }
                    i = j - 1;
                }
                //LHAD HENA
                //HENA ELELSE DY TANY HAGA AGHAYRHAA AHOO
                else
                {
                    while (true)
                    {
                        if (j == SourceCode.Length)
                        {
                            break;
                        }
                        else if (FindTokenClass(CurrentLexeme))
                        {
                            CurrentLexeme = "";
                            break;
                        }
                        else
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;

                        }
                        if (FindTokenClass(CurrentLexeme))
                        {
                            CurrentLexeme = "";
                            break;
                        }
                    }
                    if (!FindTokenClass(CurrentLexeme) && CurrentLexeme != "")
                    {
                        Errors.Error_List.Add(CurrentLexeme);
                    }
                    i = j - 1;
                }

                TINY_Compiler.TokenStream = Tokens;
            }


            bool FindTokenClass(string Lex)
            {
                Token_Class TC;
                Token Tok = new Token();
                Tok.lex = Lex;
                //Is it a reserved word?
                if (ReservedWords.ContainsKey(Lex))
                {
                    Tok.token_type = ReservedWords[Lex];
                    Tokens.Add(Tok);
                    return true;
                }

                else if (Operators.ContainsKey(Lex))
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                    return true;
                }

                //Is it an identifier?
                else if (isIdentifier(Lex))
                {
                    Tok.token_type = Token_Class.Identifier;
                    Tokens.Add(Tok);
                    return true;
                }
                //Is it a Number?
                else if (isNumber(Lex))
                {
                    Tok.token_type = Token_Class.Number;
                    Tokens.Add(Tok);
                    return true;
                }

                //Is it a string?
                else if (isString(Lex))
                {
                    Tok.token_type = Token_Class.String;
                    Tokens.Add(Tok);
                    return true;
                }

                //Is it a Comment?
                else if (isComment(Lex))
                {
                    Tok.token_type = Token_Class.Comment;
                    Tokens.Add(Tok);
                    return true;
                }

                //Is it an undefined?
                else
                {

                    return false;
                }
            }

            bool isIdentifier(string lex)
            {
                bool isValid = true;
                // Check if the lex is an identifier or not.
                var r = new Regex("^[a-zA-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);
                if (r.IsMatch(lex))
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }

                return isValid;
            }
            bool isNumber(string lex)
            {
                bool isValid = true;
                // Check if the lex is a constant (Number) or not.
                var r = new Regex("^[0-9]+(.([0-9]+))?$", RegexOptions.Compiled);
                if (r.IsMatch(lex))
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }

                return isValid;
            }
            bool isComment(string lex)
            {
                bool isValid = true;
                // check if the lex is a comment or not
                //hena ghayart fy elregex
                var r = new Regex(@"^\/\*(.|\n)*\*\/$", RegexOptions.Compiled);

                if (r.IsMatch(lex))
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }

                return isValid;
            }
            bool isString(string lex)
            {
                bool isValid = true;
                // check if the lex is a STRING or not
                var r = new Regex("^\".*\"$", RegexOptions.Compiled);
                if (r.IsMatch(lex))
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }

                return isValid;
            }

        }

    }
}