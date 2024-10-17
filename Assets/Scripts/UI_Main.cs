using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    [SerializeField] InputField Input_Address;
    [SerializeField] Button Btn_Read;
    [SerializeField] Text Text_Test;

    private List<string> contentList = new List<string>();


    private void OnEnable()
    {
        Btn_Read.onClick.AddListener(ReadFile);
    }

    private void OnDisable()
    {
        Btn_Read.onClick.RemoveListener(ReadFile);
    }


    void ReadFile()
    {
        string filePath = Input_Address.text;

        if (File.Exists(filePath))
        {
            try
            {
                // ������ �а� �� ���� �����ϰ� �� ���� ����Ʈ�� ����
                contentList = File.ReadAllLines(filePath)
                                  .Where(line => !string.IsNullOrWhiteSpace(line))
                                  .ToList();


                string newContent = MakeSRT(contentList);
                // ����Ʈ�� ������ Text ������Ʈ�� ǥ��
                Text_Test.text = newContent;

                SaveFile(Input_Address.text, newContent);

            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error reading file: {e.Message}");
                Text_Test.text = "Error reading file. Please check the console for details.";
            }
        }
        else
        {
            Text_Test.text = "File not found. Please check the path and try again.";
        }
    }

    //�ð� ���� 1�ʷ� ����.
    private string MakeSRT(List<string> contentList)
    {
        string newContent = string.Empty;
        int sec = 0;
        int min = 0;
        int hour = 0;
        for(int i = 0; i < contentList.Count; i++)
        {
            string index = (i+1).ToString();
            string timeLine = $"{hour:D2}:{min:D2}:{sec:D2},000 --> {hour:D2}:{min:D2}:{sec+1:D2},000";
            string content = contentList[i];

            newContent += index + "\n";
            newContent += timeLine + "\n";
            newContent += content + "\n";
            newContent += "\n";

            sec++;
            if(sec == 59) { min++; sec = 0; }
            if(min == 60) { hour++; min = 0; }
            if(hour > 99) { Debug.LogError("?�̷����� ���µ�"); break; }
        }
        return newContent;
    }

    private void SaveFile(string originalFilePath, string content)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
        string newFileName = fileNameWithoutExtension + "_����.srt";
        string directoryPath = Path.GetDirectoryName(originalFilePath);
        string newFilePath = Path.Combine(directoryPath, newFileName);
        
        File.WriteAllText(newFilePath, content);
    }
}
