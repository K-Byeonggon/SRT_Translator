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
                // 파일을 읽고 빈 줄을 제외하고 각 줄을 리스트에 저장
                contentList = File.ReadAllLines(filePath)
                                  .Where(line => !string.IsNullOrWhiteSpace(line))
                                  .ToList();


                string newContent = MakeSRT(contentList);
                // 리스트의 내용을 Text 컴포넌트에 표시
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

    //시간 간격 1초로 저장.
    private string MakeSRT(List<string> contentList)
    {
        string newContent = string.Empty;
        
        TimeSpan time = TimeSpan.Zero;
        TimeSpan interval = TimeSpan.FromSeconds(subtitleInterval);
        TimeSpan length = TimeSpan.FromSeconds(subtitleLength);

        for(int i = 0; i < contentList.Count; i++)
        {
            //자막 간격 계산
            time = time.Add(interval);
            string startTime = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2},{time.Milliseconds:D3}";

            //자막 길이 계산
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
        string newFileName = fileNameWithoutExtension + "_수정.srt";
        string directoryPath = Path.GetDirectoryName(originalFilePath);
        string newFilePath = Path.Combine(directoryPath, newFileName);
        
        File.WriteAllText(newFilePath, content);
    }
}
