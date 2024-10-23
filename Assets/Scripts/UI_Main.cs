using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    [SerializeField] InputField Input_Address;
    [SerializeField] InputField Input_Length;
    [SerializeField] InputField Input_Interval;
    [SerializeField] Button Btn_Read;
    [SerializeField] Text Text_Test;


    private float subtitleLength;
    private float subtitleInterval;

    private List<string> contentList = new List<string>();


    private void OnEnable()
    {
        Btn_Read.onClick.AddListener(ReadFile);
    }

    private void OnDisable()
    {
        Btn_Read.onClick.RemoveListener(ReadFile);
    }


    private float textParser(string text, float defaultReturn = 3f)
    {
        return float.TryParse(text, out float parsedText) ? parsedText : defaultReturn;
    }


    void ReadFile()
    {
        string filePath = Input_Address.text;
        subtitleLength = textParser(Input_Length.text, 3f);
        subtitleInterval = textParser(Input_Interval.text, 0f);

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
        
        TimeSpan time = TimeSpan.Zero;
        TimeSpan interval = TimeSpan.FromSeconds(subtitleInterval);
        TimeSpan length = TimeSpan.FromSeconds(subtitleLength);

        for(int i = 0; i < contentList.Count; i++)
        {
            //�ڸ� ���� ���
            time = time.Add(interval);
            string startTime = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2},{time.Milliseconds:D3}";

            //�ڸ� ���� ���
            time = time.Add(length);
            string endTime = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2},{time.Milliseconds:D3}";

            string index = (i + 1).ToString();

            string timeLine = $"{startTime} --> {endTime}";
            
            string content = contentList[i];

            newContent += index + "\n";
            newContent += timeLine + "\n";
            newContent += content + "\n";
            newContent += "\n";
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
