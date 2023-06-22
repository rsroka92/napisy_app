using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace napisy_app
{
    class Program
    {
        private static TimeSpan _timeDiff = new TimeSpan(0, 0, 0, 5, 880);
        static void Main(string[] args)
        {
            string textFile = @"C:\Users\rsrok\Desktop\napisy\napisy do filmu.srt";
            if (!File.Exists(textFile))
            {
                Console.WriteLine("Nie ma takiego pliku");
                return;
            }
            string text = File.ReadAllText(textFile);

            int newListIndex = 0;
            var textsList = text.Split("\n\n").ToList();
            var newTextsList = new List<string>();

            for (int i = 0; i < textsList.Count; i++)
            {
                string[] textTimeArray = textsList[i].Split("\n");
                string[] eachTextTime = textTimeArray[1].Split(" --> ");

                var textStart = ConvertToTimestampWithDiffTime(eachTextTime[0]);
                var textEnd = ConvertToTimestampWithDiffTime(eachTextTime[1]);
                if (textStart.Split(",")[1].Equals("0"))
                {
                    newListIndex++;
                    newTextsList.Add(BuildNewTextWithIndex(textTimeArray, textStart, textEnd, newListIndex));
                    textsList.RemoveAt(i);
                    i--;
                }
                else
                {
                    textsList[i] = UpdateTextListIndex(textTimeArray, textStart, textEnd, newListIndex);
                }
            }
            CreateNewFileAndSaveData(newTextsList, @"C:\Users\rsrok\Desktop\napisy\napisy do filmu1.srt");
            CreateNewFileAndSaveData(textsList, @"C:\Users\rsrok\Desktop\napisy\napisy do filmu2.srt");
            //CreateNewFileAndSaveData(textsList, @"C:\Users\rsrok\Desktop\napisy\napisy do filmu.srt");
        }

        private static void CreateNewFileAndSaveData(List<string> data, string fileName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.AppendLine(item);
            }
            File.WriteAllText(fileName, sb.ToString());
        }

        private static string UpdateTextListIndex(string[] textArray, string textStart, string textEnd, int index)
        {
            var dialog = "";

            textEnd = HandleEndText(textEnd);

            for (int i = 2; i < textArray.Length; i++)
            {
                dialog += textArray[i] + "\n";
            }
            return $"{Convert.ToInt16(textArray[0]) - index}\n{textStart} --> {textEnd}\n{dialog}";
        }

        private static string BuildNewTextWithIndex(string[] textArray, string textStart, string textEnd, int index)
        {
            var dialog = "";

            textEnd = HandleEndText(textEnd);

            for (int i = 2; i < textArray.Length; i++)
            {
                dialog += textArray[i] + "\n";
            }
            return $"{index}\n{textStart}00 --> {textEnd}\n{dialog}"; ;
        }

        private static string HandleEndText(string textEnd)
        {
            var tEnd = textEnd.Split(",")[1].Length;
            if (tEnd == 3)
                return textEnd;
            if (tEnd == 1)
                return textEnd += "00";
            if (tEnd == 2)
                return textEnd += "0";

            return textEnd.Substring(0, 3);
        }

        private static string ConvertToTimestampWithDiffTime(string time)
        {
            var splittedTime = time.Split(":");
            var sec = Convert.ToInt16(splittedTime[2].Split(",")[0]);
            var milisec = Convert.ToInt16(splittedTime[2].Split(",")[1]);
            var timestamp = new TimeSpan(0, Convert.ToInt16(splittedTime[0]), Convert.ToInt16(splittedTime[1]), sec, milisec);
            timestamp = timestamp.Add(_timeDiff);
            return $"{(timestamp.Hours < 10 ? "0" + timestamp.Hours.ToString() : timestamp.Hours.ToString())}:{(timestamp.Minutes < 10 ? "0" + timestamp.Minutes.ToString() : timestamp.Minutes.ToString())}" +
                $":{(timestamp.Seconds < 10 ? "0" + timestamp.Seconds.ToString() : timestamp.Seconds.ToString())},{timestamp.Milliseconds}";
        }
    }
}
