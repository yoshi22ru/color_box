using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExpressionEvaluator
{
    public int Evaluate(string equation, List<int> numbers)
    {
        return Evaluate(equation, numbers.Select(n => (int?)n).ToList());
    }

    public int Evaluate(string equation, List<int?> numbers)
    {
        if (numbers == null || !numbers.All(n => n.HasValue))
        {
            return 99999;
        }

        // 数式を逆ポーランド記法（RPN）に変換して計算
        try
        {
            return CalculateRPN(InfixToRPN(equation, numbers));
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Expression evaluation failed: " + equation + "\nError: " + ex.Message);
            return 99999;
        }
    }

    // 中置記法から逆ポーランド記法へ変換
    private Queue<string> InfixToRPN(string equation, List<int?> numbers)
    {
        var outputQueue = new Queue<string>();
        var operatorStack = new Stack<string>();
        var tokens = new List<string>();

        // 数式をトークンに分割
        string expression = equation;
        for (int i = 0; i < numbers.Count; i++)
        {
            expression = expression.Replace(((char)('A' + i)).ToString(), numbers[i].Value.ToString());
        }

        // --- 修正箇所: ×を*に変換 ---
        expression = expression.Replace("×", "*");

        string currentNumber = "";
        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];
            if (char.IsDigit(c) || c == '.')
            {
                currentNumber += c;
            }
            else
            {
                if (currentNumber != "")
                {
                    tokens.Add(currentNumber);
                    currentNumber = "";
                }
                if (c != ' ') tokens.Add(c.ToString());
            }
        }
        if (currentNumber != "") tokens.Add(currentNumber);

        // アルゴリズムの実行
        foreach (var token in tokens)
        {
            if (IsNumber(token))
            {
                outputQueue.Enqueue(token);
            }
            else if (IsOperator(token))
            {
                while (operatorStack.Count > 0 && IsOperator(operatorStack.Peek()) && GetPrecedence(operatorStack.Peek()) >= GetPrecedence(token))
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                operatorStack.Push(token);
            }
            else if (token == "(")
            {
                operatorStack.Push(token);
            }
            else if (token == ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                {
                    operatorStack.Pop();
                }
            }
        }
        while (operatorStack.Count > 0)
        {
            outputQueue.Enqueue(operatorStack.Pop());
        }
        return outputQueue;
    }

    // RPNの計算
    private int CalculateRPN(Queue<string> rpnQueue)
    {
        var stack = new Stack<int>();
        while (rpnQueue.Count > 0)
        {
            var token = rpnQueue.Dequeue();
            if (IsNumber(token))
            {
                stack.Push(int.Parse(token));
            }
            else if (IsOperator(token))
            {
                int operand2 = stack.Pop();
                int operand1 = stack.Pop();
                switch (token)
                {
                    case "+": stack.Push(operand1 + operand2); break;
                    case "-": stack.Push(operand1 - operand2); break;
                    case "*": stack.Push(operand1 * operand2); break;
                    case "/": stack.Push(operand1 / operand2); break;
                }
            }
        }
        return stack.Pop();
    }

    private bool IsNumber(string s) => int.TryParse(s, out _);
    private bool IsOperator(string s) => s == "+" || s == "-" || s == "*" || s == "/";
    private int GetPrecedence(string op)
    {
        if (op == "+" || op == "-") return 1;
        if (op == "*" || op == "/") return 2;
        return 0;
    }
}