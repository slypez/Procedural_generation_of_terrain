using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataAnalyzer
{
    public enum valueRepresentation { ExecutionTime, Traversability };
    public enum unitOfTime { None, Milliseconds, Seconds, Minutes, Hours }
    public enum numberForm { Percent, Decimal };
    public enum LogContent { All, AverageValue, None }

    private static float averageValue;
    private static float medianValue;
    private static float maxValue;
    private static float minValue;
    private static float standardDeviationValue;
    private static int sampleRate;

    public static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    private static float SOME_OTHER_STATISTIC_HERE_NORMALFÖRDELNING_KANSKE;

    private static void LogData(valueRepresentation v, numberForm form, int floatPrecision, unitOfTime unitOfTime = unitOfTime.None, LogContent logContent = LogContent.All, bool WriteValuesToTextFile = false)
    {
        string name = "";
        string unit = "";

        if(v == valueRepresentation.ExecutionTime)
        {
            name = "execution time";
        }
        else if(v == valueRepresentation.Traversability)
        {
            name = "traversability";
        }

        if(form == numberForm.Percent)
        {
            float percent = 100f;
            averageValue *= percent;
            medianValue *= percent;
            maxValue *= percent;
            minValue *= percent;
            standardDeviationValue *= percent;
            unit = "%";
        }
        else if(form == numberForm.Decimal)
        {
            switch (unitOfTime)
            {
                case unitOfTime.None:
                    unit = "units";
                    break;
                case unitOfTime.Milliseconds:
                    unit = "ms";
                    break;
                case unitOfTime.Seconds:
                    unit = "s";
                    break;
                case unitOfTime.Minutes:
                    unit = "m";
                    break;
                case unitOfTime.Hours:
                    unit = "h";
                    break;
            }
        }

        string value_1 = "";
        string value_2 = "";
        string value_3 = "";
        string value_4 = "";

        if (logContent == LogContent.All) // Not best way to handle content: Better would be to check if medianvalue even has value, and if it has then add it or something.
        {
            value_1 = sampleRate.ToString();
            value_2 = medianValue.ToString("F" + floatPrecision);
            value_3 = minValue.ToString("F" + floatPrecision);
            value_4 = standardDeviationValue.ToString("F" + floatPrecision);

            Debug.LogFormat("<color=green>*Test started*</color>" + "\n"
            + "This is a test for {0}. Samplerate is : " + "<color=purple>" + value_1 + "</color>", name);
            Debug.LogFormat("Average {0}: " + "<color=green>" + averageValue.ToString("F" + floatPrecision) + "</color>" + "{1}" + "\n"
            + "Median {0}: " + "<color=green>" + value_2 + "</color>" + "{1}", name, unit);
            Debug.LogFormat("Maximum {0}: " + "<color=green>" + maxValue.ToString("F" + floatPrecision) + "</color>" + "{1}" + "\n"
            + "Minimum {0}: " + "<color=green>" + value_3 + "</color>" + "{1}", name, unit);
            Debug.LogFormat("Standard deviation {0}: " + "<color=green>" + value_4 + "</color>" + "{1}" + "\n"
            + "<color=red>*Test concluded*</color>", name, unit);
        }
        else if(logContent == LogContent.AverageValue)
        {
            value_1 = averageValue.ToString("F" + floatPrecision);
            Debug.LogFormat("<color=green>" + value_1 + "</color>");
        }
        else if(logContent == LogContent.None)
        {
            WriteValuesToTextFile = false;
            Debug.LogFormat("<color=red>" + "You are not logging anything here, test is still executing though." + "</color>");
        }

        if (WriteValuesToTextFile)
        {
            if (value_1.Length > 0)
            {
                HandleTextFile.WriteTextContent(value_1);
            }
            if (value_2.Length > 0)
            {
                HandleTextFile.WriteTextContent(value_2);
            }
            if (value_3.Length > 0)
            {
                HandleTextFile.WriteTextContent(value_3);
            }
            if (value_4.Length > 0)
            {
                HandleTextFile.WriteTextContent(value_4);
            }
        }
    }

    public static void AnalyzeAndShowData(List<float> values, valueRepresentation v, numberForm form, int floatPrecision, unitOfTime unitOfTime = unitOfTime.None, LogContent logContent = LogContent.All, bool WriteValuesToTextFile = false)
    {
        // Create helpful statistics here
        if (logContent == LogContent.All) // Not best way to handle content: Better would be to check if medianvalue even has value, and if it has then add it or something.
        {
            sampleRate = values.Count;
            averageValue = CalculateAverageValue(values);
            medianValue = CalculateMedianValue(values);
            standardDeviationValue = CalculateStandardDeviationValue(values);
            minValue = Mathf.Min(values.ToArray());
            maxValue = Mathf.Max(values.ToArray());
        }
        else if (logContent == LogContent.AverageValue)
        {
            sampleRate = values.Count;
            averageValue = CalculateAverageValue(values);
        }
        else if (logContent == LogContent.None)
        {
            // Nada
        }


        LogData(v, form, floatPrecision, unitOfTime, logContent, WriteValuesToTextFile);
    }

    private static float CalculateStandardDeviationValue(List<float> values)
    {
        float sum = 0f;
        float avg = CalculateAverageValue(values);

        foreach (float value in values)
        {
            sum += Mathf.Pow((value - avg), 2);
        }

        return Mathf.Sqrt((sum / sampleRate));
    }

    private static float CalculateAverageValue(List<float> values)
    {
        float sum = 0f;
        foreach (float value in values)
        {
            sum += value;
        }
        return sum / sampleRate;
    }

    private static float CalculateMedianValue(List<float> values)
    {
        if(sampleRate == 1)
        {
            return values[0];
        }

        float median = 0f;
        values.Sort();

        if(sampleRate % 2f == 0) // Even length
        {
            median = (values[(sampleRate / 2) - 1] + values[sampleRate / 2]) / 2f;
        }
        else // Uneven length
        {
            median = values[Mathf.CeilToInt(sampleRate / 2)];
        }
        return median;
    }
}
