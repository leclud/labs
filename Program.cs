using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

abstract class Token
{

}
class Number : Token
{
    public float Value { get; }

    public Number(float value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

class Operation : Token
{
    public char Symbol;
    public int Priorety;
    public Operation(char symbol)
    {
        Symbol = symbol;
        Priorety = GetPriorety(symbol);
    }

    private int GetPriorety(char symbol)
    {
        Dictionary<object, int> PriorityOperation = new Dictionary<object, int>
            {
                {'+', 1},
                {'-', 1},
                {'*', 2},
                {'/', 2},
                {'(', 0},
                {')', 5},
            };
        return PriorityOperation[symbol];
    }

    public override string ToString()
    {
        return Symbol.ToString();
    }
}

class Parenthesis : Token
{
    public char Symbol;
    public bool IsClosing;
    public Parenthesis(char symbol)
    {
        if (symbol != '(' && symbol != ')')
        {
            throw new ArgumentException("This is not valid ...");
        }

        IsClosing = symbol == ')';
        Symbol = symbol;
    }
    public override string ToString()
    {
        return Symbol.ToString();
    }
}

namespace LabWork
{
    class Programm
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Введите математическое выражение: ");
                string expression = Console.ReadLine();
                expression = expression.Replace(" ", string.Empty);

                List<Token> tokens = GetToken(expression);
                List<Token> rpn = PRN(tokens);

                Console.Write("Список чисел: ");
                foreach (Token token in rpn)
                {
                    if (token is Number)
                    {
                        Console.Write(token);
                        Console.Write(" ");
                    }
                }
                Console.WriteLine("");

                Console.Write("Список операций: ");
                foreach (Token token in rpn)
                {
                    if (token is Operation)
                    {
                        Console.Write(token);
                        Console.Write(" ");
                    }

                }
                Console.WriteLine("");

                Console.Write("Выражение в ОПЗ:");
                foreach (Token token in rpn)
                {
                    if (token is Number)
                    {
                        Number number = (Number)token;
                        Console.Write(number.Value);
                        Console.Write(" ");
                    }
                    else if (token is Operation)
                    {
                        Operation operation = (Operation)token;
                        Console.Write(operation.Symbol);
                        Console.Write(" ");
                    }
                    else if (token is Parenthesis)
                    {
                        Parenthesis parenthesis = (Parenthesis)token;
                        Console.Write(parenthesis.Symbol);
                        Console.Write(" ");
                    }
                }
                Console.WriteLine("");

                float result = EvaluateRPN(rpn);
                Console.WriteLine("Результат: " + result);
                Console.WriteLine("Для продолжения нажмите любую кнопку...");
                Console.ReadKey();
            }
        }

        public static List<Token> GetToken(string expression)
        {
            List<Token> tokens = new List<Token>();
            string num = string.Empty;
            for (int i = 0; i < expression.Length; i++)
            {
                if (char.IsDigit(expression[i]) || expression[i] == ',' || expression[i] == '.')
                {
                    num += expression[i];
                }
                else if (expression[i] == '+' || expression[i] == '-' || expression[i] == '*' || expression[i] == '/')
                {
                    if (num != string.Empty)
                    {
                        tokens.Add(new Number(float.Parse(num)));
                        num = string.Empty;
                    }
                    tokens.Add(new Operation(expression[i]));
                }
                else if (expression[i] == '(' || expression[i] == ')')
                {
                    if (num != string.Empty)
                    {
                        tokens.Add(new Number(float.Parse(num)));
                        num = string.Empty;
                    }
                    tokens.Add(new Parenthesis(expression[i]));
                }
            }
            if (num != string.Empty)
            {
                tokens.Add(new Number(float.Parse(num)));
            }
            return tokens;
        }



        public static List<Token> PRN(List<Token> tokens)
        {
            List<Token> prn = new List<Token>();
            Stack<Token> stack = new Stack<Token>();
            foreach (Token token in tokens)
            {
                if (stack.Count == 0 && !(token is Number))
                {
                    stack.Push(token);
                    continue;
                }
                if (token is Operation)
                {
                    if (stack.Peek() is Parenthesis)
                    {
                        stack.Push(token);
                        continue;
                    }

                    Operation oper = (Operation)token;
                    Operation oper2 = (Operation)stack.Peek();
                    if (oper.Priorety > oper2.Priorety)
                    {
                        stack.Push(token);
                    }
                    else if (oper.Priorety <= oper2.Priorety)
                    {
                        while (stack.Count > 0 && !(token is Parenthesis))
                        {
                            prn.Add(stack.Pop());
                        }
                        stack.Push(token);
                    }
                }
                else if (token is Parenthesis)
                {
                    if (((Parenthesis)token).IsClosing)
                    {
                        while (!(stack.Peek() is Parenthesis))
                        {
                            prn.Add(stack.Pop());
                        }
                        stack.Pop();
                    }
                    else
                    {
                        stack.Push(token);
                    }
                }
                else if (token is Number)
                {
                    prn.Add(token);
                }
            }
            while (stack.Count > 0)
            {
                prn.Add(stack.Pop());
            }
            return prn;
        }

        private static float EvaluateRPN(List<Token> tokens)
        {
            Stack<float> stack = new Stack<float>();

            foreach (Token token in tokens)
            {
                if (token is Number)
                {
                    Number number = (Number)token;
                    stack.Push(number.Value);
                }
                else if (token is Operation)
                {
                    Operation op = (Operation)token;
                    if (stack.Count < 1)
                    {
                        throw new InvalidOperationException("Invalid expression");
                    }
                    float b = stack.Pop();
                    float a = stack.Pop();

                    switch (op.Symbol)
                    {
                        case '+':
                            stack.Push(a + b);
                            break;
                        case '-':
                            stack.Push(a - b);
                            break;
                        case '*':
                            stack.Push(a * b);
                            break;
                        case '/':
                            stack.Push(a / b);
                            break;
                    }
                }
            }

            if (stack.Count != 1)
            {
                throw new InvalidOperationException("Invalid expression");
            }

            return stack.Pop();
        }
    }
}