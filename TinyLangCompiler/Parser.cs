using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TINY_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Progm()
        {
            Node progm = new Node("Progm");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comment)
            {
                progm.Children.Add(match(Token_Class.Comment));
            }

            else
            {
                progm.Children.Add(FunctionStatement());
            }
            return progm;
        }
        Node Prog()
        {
            Node prog = new Node("Prog");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer + 1].token_type == Token_Class.Identifier || TokenStream[InputPointer].token_type == Token_Class.Comment))
            {
                prog.Children.Add(Progm());
                prog.Children.Add(Prog());
            }
            else
            {
                return null;
            }
            return prog;
        }
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Prog());
            program.Children.Add(MainFunction());
            return program;
        }

        Node Arguments()
        {
            Node arguments = new Node("Arguments");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                arguments.Children.Add(match(Token_Class.Comma));
                arguments.Children.Add(Expression());
                arguments.Children.Add(Arguments());
            }
            else
            {
                return null;
            }
            return arguments;
        }
        Node ArgList()
        {
            Node argList = new Node("ArgList");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier
                || TokenStream[InputPointer].token_type == Token_Class.Number
                || TokenStream[InputPointer].token_type == Token_Class.LeftBraces
                || TokenStream[InputPointer].token_type == Token_Class.String))
            {
                argList.Children.Add(Expression());
                argList.Children.Add(Arguments());
                return argList;
            }
            else
            {
                return null;
            }

        }
        Node FunctionCall()
        {
            Node functionCall = new Node("Function Call");

            functionCall.Children.Add(match(Token_Class.Identifier));
            functionCall.Children.Add(match(Token_Class.LeftBraces));
            functionCall.Children.Add(ArgList());
            functionCall.Children.Add(match(Token_Class.RightBraces));

            return functionCall;
        }
        Node Term()
        {
            Node term = new Node("Term");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                term.Children.Add(match(Token_Class.Identifier));
                term.Children.Add(Ter());
            }

            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                term.Children.Add(match(Token_Class.Number));
            }
            return term;
        }
        Node Ter()
        {
            Node ter = new Node("Ter");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LeftBraces)
            {
                ter.Children.Add(match(Token_Class.LeftBraces));
                ter.Children.Add(ArgList());
                ter.Children.Add(match(Token_Class.RightBraces));
            }
            else
            {
                return null;
            }
            return ter;
        }
        Node ArithmeticOP()
        {
            Node arithmeticOP = new Node("ArithmeticOP");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Plus)
            {
                arithmeticOP.Children.Add(match(Token_Class.Plus));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Minuse)
            {
                arithmeticOP.Children.Add(match(Token_Class.Minuse));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Multiply)
            {
                arithmeticOP.Children.Add(match(Token_Class.Multiply));
            }
            else
            {
                arithmeticOP.Children.Add(match(Token_Class.Division));
            }
            return arithmeticOP;

        }
        Node Eq()
        {
            Node eq = new Node("Eq");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Number
                || TokenStream[InputPointer].token_type == Token_Class.Identifier))
            {
                eq.Children.Add(Term());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LeftBraces)
            {
                eq.Children.Add(match(Token_Class.LeftBraces));
                eq.Children.Add(Equation());
                eq.Children.Add(match(Token_Class.RightBraces));
            }
            return eq;
        }
        Node Equa()
        {
            Node equa = new Node("Equa");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Plus
                || TokenStream[InputPointer].token_type == Token_Class.Minuse
                || TokenStream[InputPointer].token_type == Token_Class.Multiply
                || TokenStream[InputPointer].token_type == Token_Class.Division))
            {
                equa.Children.Add(ArithmeticOP());
                equa.Children.Add(Eq());
                equa.Children.Add(Equa());

            }
            else
            {
                return null;
            }

            return equa;
        }
        Node Equation()
        {
            Node equation = new Node("Equation");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Number
                || TokenStream[InputPointer].token_type == Token_Class.Identifier))
            {
                equation.Children.Add(Term());
                equation.Children.Add(Equa());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LeftBraces)
            {
                equation.Children.Add(match(Token_Class.LeftBraces));
                equation.Children.Add(Equation());
                equation.Children.Add(match(Token_Class.RightBraces));
                equation.Children.Add(Equa());
            }
            return equation;
        }
        Node Expression()
        {
            Node expression = new Node("Expression");

            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Number
                || TokenStream[InputPointer].token_type == Token_Class.Identifier))
            {

                expression.Children.Add(Term());
                expression.Children.Add(Exp());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LeftBraces)
            {
                expression.Children.Add(match(Token_Class.LeftBraces));
                expression.Children.Add(Equation());
                expression.Children.Add(match(Token_Class.RightBraces));
                expression.Children.Add(Equa());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                expression.Children.Add(match(Token_Class.String));
            }

            return expression;
        }

        Node Exp()
        {
            Node exp = new Node("Exp");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Plus
                || TokenStream[InputPointer].token_type == Token_Class.Minuse
                || TokenStream[InputPointer].token_type == Token_Class.Multiply
                || TokenStream[InputPointer].token_type == Token_Class.Division))
            {
                exp.Children.Add(Equa());
            }
            else
            {
                return null;
            }
            return exp;
        }
        Node AssignmentStatement()
        {
            Node assignmentStatement = new Node("AssignmentStatement");
            assignmentStatement.Children.Add(match(Token_Class.Identifier));
            assignmentStatement.Children.Add(match(Token_Class.Assign));
            assignmentStatement.Children.Add(Expression());

            return assignmentStatement;
        }
        Node FullAssignmentStatement()
        {
            Node fullassignmentStatement = new Node("FullAssignmentStatement");
            fullassignmentStatement.Children.Add(AssignmentStatement());
            fullassignmentStatement.Children.Add(match(Token_Class.SemiColon));

            return fullassignmentStatement;
        }
        Node Datatype()
        {
            Node datatype = new Node("Datatype");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DatatypeInt)
            {
                datatype.Children.Add(match(Token_Class.DatatypeInt));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DatatypeFloat)
            {
                datatype.Children.Add(match(Token_Class.DatatypeFloat));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DatatypeString)
            {
                datatype.Children.Add(match(Token_Class.DatatypeString));
            }
            return datatype;

        }
        Node AssignState()
        {
            Node assignState = new Node("AsssignState");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Assign))
            {
                assignState.Children.Add(match(Token_Class.Assign));
                assignState.Children.Add(Expression());
                return assignState;
            }
            else
            {
                return null;
            }

        }
        Node Declaration()
        {
            Node declaration = new Node("Declaration");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Comma))
            {
                declaration.Children.Add(match(Token_Class.Comma));
                declaration.Children.Add(match(Token_Class.Identifier));
                declaration.Children.Add(AssignState());
                declaration.Children.Add(Declaration());
                return declaration;
            }
            else
            {
                return null;
            }

        }
        Node Declare()
        {
            Node declare = new Node("Declare");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Assign)
            {
                declare.Children.Add(AssignState());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {

                declare.Children.Add(Declaration());
            }
            else
            {
                return null;
            }
            return declare;
        }
        Node DeclarationStatement()
        {
            Node declarationStatement = new Node("DeclarationStatement");
            declarationStatement.Children.Add(Datatype());
            declarationStatement.Children.Add(match(Token_Class.Identifier));
            declarationStatement.Children.Add(Declare());
            declarationStatement.Children.Add(match(Token_Class.SemiColon));

            return declarationStatement;
        }
        Node Writestate()
        {
            Node writeState = new Node("WriteState");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.String
                || TokenStream[InputPointer].token_type == Token_Class.Number
                || TokenStream[InputPointer].token_type == Token_Class.Identifier))
            {
                writeState.Children.Add(Expression());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EndLine)
            {
                writeState.Children.Add(match(Token_Class.EndLine));
            }
            return writeState;
        }
        Node WriteStatement()
        {
            Node writeStatement = new Node("Write State");
            writeStatement.Children.Add(match(Token_Class.Write));
            writeStatement.Children.Add(Writestate());
            writeStatement.Children.Add(match(Token_Class.SemiColon));

            return writeStatement;
        }
        Node ReadStatement()
        {
            Node readStatement = new Node("ReadStatement");
            readStatement.Children.Add(match(Token_Class.Read));
            readStatement.Children.Add(match(Token_Class.Identifier));
            readStatement.Children.Add(match(Token_Class.SemiColon));

            return readStatement;
        }
        Node ReturnStatement()
        {
            Node returnStatement = new Node("ReturnStatement");
            returnStatement.Children.Add(match(Token_Class.Return));
            returnStatement.Children.Add(Expression());
            returnStatement.Children.Add(match(Token_Class.SemiColon));

            return returnStatement;
        }
        Node BooleanOp()
        {
            Node booelanOp = new Node("BooleanOP");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                booelanOp.Children.Add(match(Token_Class.AndOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                booelanOp.Children.Add(match(Token_Class.OrOp));
            }
            return booelanOp;
        }
        Node ConditionOp()
        {
            Node conditionOp = new Node("ConditionOp");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LessThan)
            {
                conditionOp.Children.Add(match(Token_Class.LessThan));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.GreaterThan)
            {
                conditionOp.Children.Add(match(Token_Class.GreaterThan));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                conditionOp.Children.Add(match(Token_Class.EqualOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.NotEqual)
            {
                conditionOp.Children.Add(match(Token_Class.NotEqual));
            }
            return conditionOp;
        }
        Node Condition()
        {
            Node condition = new Node("Condition");

            condition.Children.Add(match(Token_Class.Identifier));
            condition.Children.Add(ConditionOp());
            condition.Children.Add(Term());

            return condition;
        }

        Node CondState()
        {
            Node condState = new Node("CondState");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.AndOp
                || TokenStream[InputPointer].token_type == Token_Class.OrOp))
            {
                condState.Children.Add(BooleanOp());
                condState.Children.Add(Condition());
                condState.Children.Add(CondState());
                return condState;
            }
            else
            {
                return null;
            }
        }
        Node ConditionStatement()
        {
            Node conditionStatement = new Node("ConditionStatement");
            conditionStatement.Children.Add(Condition());
            conditionStatement.Children.Add(CondState());

            return conditionStatement;
        }
        Node IfStatement()
        {
            Node ifStatement = new Node("IfStatement");
            ifStatement.Children.Add(match(Token_Class.If));
            ifStatement.Children.Add(ConditionStatement());
            ifStatement.Children.Add(match(Token_Class.Then));
            ifStatement.Children.Add(Statements());
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                ifStatement.Children.Add(ReturnStatement());
            }
            ifStatement.Children.Add(EndState());

            return ifStatement;
        }
        Node EndState()
        {
            Node endState = new Node("EndState");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.ElseIf)
            {
                endState.Children.Add(ElseIfStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                endState.Children.Add(ElseStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.End)
            {
                endState.Children.Add(match(Token_Class.End));
            }
            return endState;
        }
        Node State()
        {
            Node state = new Node("State");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Read
                || TokenStream[InputPointer].token_type == Token_Class.Write
                || TokenStream[InputPointer].token_type == Token_Class.Identifier
                || (TokenStream[InputPointer].token_type == Token_Class.DatatypeFloat
                || TokenStream[InputPointer].token_type == Token_Class.DatatypeInt
                || TokenStream[InputPointer].token_type == Token_Class.DatatypeString)
                || TokenStream[InputPointer].token_type == Token_Class.Repeat
                || TokenStream[InputPointer].token_type == Token_Class.If
                || TokenStream[InputPointer].token_type == Token_Class.Comment))
            {
                state.Children.Add(Statement());
                state.Children.Add(State());

            }
            else
            {
                return null;
            }
            return state;
        }
        Node Statements()
        {
            Node statements = new Node("Statements");
            statements.Children.Add(Statement());
            statements.Children.Add(State());

            return statements;
        }
        Node Statement()
        {
            Node statement = new Node("Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                statement.Children.Add(ReadStatement());

            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                statement.Children.Add(WriteStatement());

            }
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.DatatypeFloat
                || TokenStream[InputPointer].token_type == Token_Class.DatatypeInt
                || TokenStream[InputPointer].token_type == Token_Class.DatatypeString))
            {
                statement.Children.Add(DeclarationStatement());

            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.LeftBraces)
                {
                    statement.Children.Add(FunctionCall());
                    statement.Children.Add(match(Token_Class.SemiColon));

                }
                else
                {
                    statement.Children.Add(FullAssignmentStatement());
                }
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                statement.Children.Add(RepeatStatement());

            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.If)
            {
                statement.Children.Add(IfStatement());

            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comment)
            {
                statement.Children.Add(match(Token_Class.Comment));

            }
            else
            {
                return null;
            }
            return statement;
        }
        Node ElseIfStatement()
        {
            Node elseIfStatement = new Node("ElseIfStatement");
            elseIfStatement.Children.Add(match(Token_Class.ElseIf));
            elseIfStatement.Children.Add(ConditionStatement());
            elseIfStatement.Children.Add(match(Token_Class.Then));
            elseIfStatement.Children.Add(Statements());
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                elseIfStatement.Children.Add(ReturnStatement());
            }
            elseIfStatement.Children.Add(EndState());

            return elseIfStatement;
        }
        Node ElseStatement()
        {
            Node elseStatement = new Node("ElseStatement");
            elseStatement.Children.Add(match(Token_Class.Else));
            elseStatement.Children.Add(Statements());
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                elseStatement.Children.Add(ReturnStatement());
            }
            elseStatement.Children.Add(match(Token_Class.End));

            return elseStatement;
        }
        Node RepeatStatement()
        {
            Node repeatStatement = new Node("RepeatStatement");
            repeatStatement.Children.Add(match(Token_Class.Repeat));
            repeatStatement.Children.Add(Statements());
            repeatStatement.Children.Add(match(Token_Class.Until));
            repeatStatement.Children.Add(ConditionStatement());

            return repeatStatement;
        }
        Node FunctionName()
        {
            Node functionName = new Node("FunctionName");
            functionName.Children.Add(match(Token_Class.Identifier));

            return functionName;
        }
        Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.Identifier));

            return parameter;
        }
        Node FunctionDeclaration()
        {
            Node functionDeclaration = new Node("FunctionDeclaration");
            functionDeclaration.Children.Add(Datatype());
            functionDeclaration.Children.Add(FunctionName());
            functionDeclaration.Children.Add(match(Token_Class.LeftBraces));
            functionDeclaration.Children.Add(ParamList());
            functionDeclaration.Children.Add(match(Token_Class.RightBraces));

            return functionDeclaration;
        }
        Node ParamList()
        {
            Node paramList = new Node("ParamList");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.DatatypeInt
                || TokenStream[InputPointer].token_type == Token_Class.DatatypeFloat
                || TokenStream[InputPointer].token_type == Token_Class.DatatypeString) && TokenStream[InputPointer + 1].token_type == Token_Class.Identifier)
            {
                paramList.Children.Add(Parameter());
                paramList.Children.Add(Par());
            }
            else
            {
                return null;
            }
            return paramList;
        }
        Node Par()
        {
            Node par = new Node("Par");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma &&
                (TokenStream[InputPointer + 1].token_type == Token_Class.DatatypeInt
                || TokenStream[InputPointer + 1].token_type == Token_Class.DatatypeFloat
                || TokenStream[InputPointer + 1].token_type == Token_Class.DatatypeString))
            {
                par.Children.Add(match(Token_Class.Comma));
                par.Children.Add(Parameter());
                par.Children.Add(Par());

            }
            else
            {
                return null;
            }
            return par;
        }
        Node FunctionBody()
        {
            Node functionBody = new Node("FunctionBody");
            functionBody.Children.Add(match(Token_Class.LeftParentheses));
            functionBody.Children.Add(Statements());
            functionBody.Children.Add(ReturnStatement());
            functionBody.Children.Add(match(Token_Class.RightParentheses));

            return functionBody;
        }
        Node FunctionStatement()
        {
            Node functionStatement = new Node("FunctionStatement");
            functionStatement.Children.Add(FunctionDeclaration());
            functionStatement.Children.Add(FunctionBody());

            return functionStatement;
        }
        Node MainFunction()
        {
            Node mainFunction = new Node("MainFunction");
            mainFunction.Children.Add(Datatype());
            mainFunction.Children.Add(match(Token_Class.main));
            mainFunction.Children.Add(match(Token_Class.LeftBraces));
            mainFunction.Children.Add(match(Token_Class.RightBraces));
            mainFunction.Children.Add(FunctionBody());

            return mainFunction;
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString());
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
