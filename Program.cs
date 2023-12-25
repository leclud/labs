using System;
using System.Collections.Generic;

namespace labwork
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите математическое выражение: ");
            string expression = Console.ReadLine().Replace(" ", "");

            var rpn = RPN(expression);

            Console.WriteLine($"Выражение в ОПЗ: {string.Join(" ", rpn)}");

            Console.WriteLine($"Результат: {EvaluateRPN(rpn)}");
        }

        public static List<object> RPN(string input)
        {
            Dictionary<object, int> priority = new Dictionary<object, int>
            {
                {'+', 0},
                {'-', 0},
                {'*', 1},
                {'/', 1},
                {'(', -1},
                {')', 2},
            };
            List<object> rpn = new List<object>();
            Stack<object> stack = new Stack<object>();
            string nums = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (priority.ContainsKey(input[i]))
                {
                    if (!string.IsNullOrEmpty(nums))
                    {
                        rpn.Add(nums);
                        nums = "";
                    }

                    if (input[i] == ')')
                    {
                        while ((char)stack.Peek() != '(')
                        {
                            rpn.Add(stack.Pop());
                        }
                        stack.Pop();
                    }
                    else if (stack.Count == 0 || input[i] == '(' || priority[input[i]] > priority[stack.Peek()])
                    {
                        stack.Push(input[i]);
                    }
                    else if (priority[input[i]] <= priority[stack.Peek()])
                    {
                        while (stack.Count > 0 && (char)stack.Peek() != '(')
                        {
                            rpn.Add(stack.Pop());
                        }
                        stack.Push(input[i]);
                    }
                }
                else
                {
                    nums += input[i];
                }
            }
            if (!string.IsNullOrEmpty(nums))
            {
                rpn.Add(nums);
            }
            while (stack.Count > 0)
            {
                rpn.Add(stack.Pop());
            }
            return rpn;
        }

        public static double EvaluateRPN(List<object> rpn)
        {
            Stack<double> stack = new Stack<double>();
            for (int i = 0; i < rpn.Count; i++)
            {
                if (rpn[i] is string)
                {
                    double num = Convert.ToDouble(rpn[i]);
                    stack.Push(num);
                }
                else
                {
                    var rightOperand = stack.Pop();
                    var leftOperand = stack.Pop();
                    stack.Push(Operator((char)rpn[i], leftOperand, rightOperand));
                }
            }
            return stack.Pop();
        }

        public static double Operator(char @operator, double leftOperand, double rightOperand)
        {
            switch (@operator)
            {
                case '+':
                    return leftOperand + rightOperand;
                case '-':
                    return leftOperand - rightOperand;
                case '*':
                    return leftOperand * rightOperand;
                case '/':
                    return leftOperand / rightOperand;
                default: return 0;
            }
        }
    }
}