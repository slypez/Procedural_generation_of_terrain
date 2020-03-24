using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataAnalyzer
{
    public enum valueRepresentation { ExecutionTime, Traversability };
    public enum numberForm { Percent, Decimal };
    private static float averageValue;
    private static float medianValue;
    private static float maxValue;
    private static float minValue;
    private static float standardDeviationValue;
    private static int sampleRate;

    private static float SOME_OTHER_STATISTIC_HERE_NORMALFÖRDELNING_KANSKE;

    private static void LogData(valueRepresentation v, numberForm form, int floatPrecision)
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
            unit = "units";
        }
        Debug.LogFormat("This is a test for {0}:" + "\n"
        + "Samplerate is : " + "<color=green>" + sampleRate.ToString() + "</color>", name);
        Debug.LogFormat("Average {0}: " + "<color=green>" + averageValue.ToString("F" + floatPrecision) + "</color>" + "{1}" + "\n" 
        + "Median {0}: " + "<color=green>" + medianValue.ToString("F"+floatPrecision) + "</color>" + "{1}", name, unit);
        Debug.LogFormat("Maximum {0}: " + "<color=green>" + maxValue.ToString("F" + floatPrecision) + "</color>" + "{1}" + "\n"
        + "Minimum {0}: " + "<color=green>" + minValue.ToString("F" + floatPrecision) + "</color>" + "{1}", name, unit);
        Debug.LogFormat("Standard deviation {0}: " + "<color=green>" + standardDeviationValue.ToString("F" + floatPrecision) + "</color>" + "{1}" + "\n"
        + "<color=red>*Test concluded*</color>", name, unit);
    }

    public static void AnalyzeAndShowData(List<float> values, valueRepresentation v, numberForm form, int floatPrecision)
    {
        // Create helpful statistics here
        sampleRate = values.Count;
        averageValue = CalculateAverageValue(values);
        medianValue = CalculateMedianValue(values);
        standardDeviationValue = CalculateStandardDeviationValue(values);
        minValue = Mathf.Min(values.ToArray());
        maxValue = Mathf.Max(values.ToArray());

        LogData(v, form, floatPrecision);
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
