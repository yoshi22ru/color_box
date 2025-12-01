using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private TMP_Text fullEquationText; // すべての数式と答えを表示

    public Material[] CubeMaterials { get; set; }

    public void UpdateUI(List<int?> inputNumbers, string currentEquationString, int? correctNum)
    {
        string displayEquation = currentEquationString;

        for (int i = 0; i < inputNumbers.Count; i++)
        {
            if (inputNumbers[i].HasValue)
            {
                // --- 修正箇所: 数字に変わった後も色を適用 ---
                if (CubeMaterials != null && i < CubeMaterials.Length)
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(CubeMaterials[i].color);
                    displayEquation = displayEquation.Replace(((char)('A' + i)).ToString(), $"<color=#{hexColor}>{inputNumbers[i].Value.ToString()}</color>");
                }
                else
                {
                    displayEquation = displayEquation.Replace(((char)('A' + i)).ToString(), inputNumbers[i].Value.ToString());
                }
            }
            else
            {
                if (CubeMaterials != null && i < CubeMaterials.Length)
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(CubeMaterials[i].color);
                    displayEquation = displayEquation.Replace(((char)('A' + i)).ToString(), $"<color=#{hexColor}>" + ((char)('A' + i)).ToString() + "</color>");
                }
                else
                {
                    displayEquation = displayEquation.Replace(((char)('A' + i)).ToString(), ((char)('A' + i)).ToString());
                }
            }
        }
        
        string resultString = correctNum.HasValue ? correctNum.Value.ToString() : "";
        fullEquationText.text = $"{displayEquation} = {resultString}";
    }

    public void InitializeUI()
    {
        fullEquationText.text = "";
    }

    public int GetNumCount()
    {
        return 3;
    }
}