// EquationGenerator.cs
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Linq;

public class EquationGenerator
{
    public string Generate(int numCount, out List<int> correctNumbers, out int correctResult)
    {
        correctNumbers = new List<int>();
        correctResult = 0;
        string equationString = "";

        string[] simpleTemplates = { "A + B", "A - B", "A × B", "A / B" };
        string[] complexTemplates = new string[]
        {
            "A + B × C",
            "A × B + C",
            "(A + B) × C",
            "A - B × C",
            "A × (B - C)",
            "A + B + C",
            "A × B × C",
            "A - B - C"
        };

        if (numCount == 2)
        {
            equationString = simpleTemplates[Random.Range(0, simpleTemplates.Length)];
        }
        else if (numCount == 3)
        {
            equationString = complexTemplates[Random.Range(0, complexTemplates.Length)];
        }
        else
        {
            equationString = simpleTemplates[Random.Range(0, simpleTemplates.Length)];
        }

        int maxAttempts = 100;
        ExpressionEvaluator evaluator = new ExpressionEvaluator();
        do
        {
            correctNumbers.Clear();
            for (int i = 0; i < numCount; i++)
            {
                correctNumbers.Add(Random.Range(1, 10));
            }

            if (equationString.Contains("/"))
            {
                if (numCount == 2)
                {
                    int temp = Random.Range(1, 10);
                    correctNumbers[0] = correctNumbers[1] * temp;
                }
            }
            
            correctResult = evaluator.Evaluate(equationString, correctNumbers.Select(n => (int?)n).ToList());

            maxAttempts--;
        } while (maxAttempts > 0 && (correctResult < -50 || correctResult > 100));
        
        return equationString;
    }
}