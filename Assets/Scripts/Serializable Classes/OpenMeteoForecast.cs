using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[Serializable]
public class OpenMeteoForecast
{
    public bool error;
    public OmfHourly hourly;
    public OmfHourly[] days;

    public OpenMeteoForecast()
    {
        this.error = false;
        this.hourly = new OmfHourly();
        this.days = new OmfHourly[5];
    }

    public OpenMeteoForecast(string jsonText)
    {
        int i, j;
        OpenMeteoForecast obj = JsonConvert.DeserializeObject<OpenMeteoForecast>(jsonText);
        this.error = obj?.error ?? false;

        if (obj?.hourly?.time == null || obj.hourly.time.Count == 0)
        {
            this.hourly = new OmfHourly();
            this.days = new OmfHourly[5];
            return;
        }

        /* Convert from UTC to local time */
        for (i = 0; i < obj.hourly.time.Count; ++i)
            obj.hourly.time[i] = obj.hourly.time[i].ToLocalTime();

        /* Keep hours: 6, 12, 18, 0 --> Create a list of anonymous type */
        var indexesToRemove = obj.hourly.time.Select((item, index) => new { Item = item, Index = index })
                                             .Where(e => e.Item.Hour != 6 && e.Item.Hour != 12 && e.Item.Hour != 18 && e.Item.Hour != 0)
                                             .Select(e => e.Index)
                                             .ToList();

        foreach (int index in indexesToRemove.OrderByDescending(i => i))
        {
            obj.hourly.time.RemoveAt(index);
            obj.hourly.temperature.RemoveAt(index);
            obj.hourly.precipitationProbability.RemoveAt(index);
        }

        /* 5 days and 4 hours per day */
        obj.days = new OmfHourly[5];
        for (i = 0; i < obj.days.Length; ++i)
        {
            obj.days[i] = new OmfHourly();
            for (j = i * 4; j < (i + 1) * 4; ++j)
            {
                obj.days[i].time.Add(obj.hourly.time[j]);
                obj.days[i].temperature.Add(obj.hourly.temperature[j]);
                obj.days[i].precipitationProbability.Add(obj.hourly.precipitationProbability[j]);
            }
        }

        this.days = obj.days;
    }
}

public class OmfHourly
{
    public List<DateTime> time; /* Default: UTC and strings in ISO8601 format */
    [JsonProperty("temperature_2m")] public List<float> temperature; /* °C */
    [JsonProperty("precipitation_probability")] public List<int> precipitationProbability; /* % */

    public OmfHourly()
    {
        this.time = new List<DateTime>();
        this.temperature = new List<float>();
        this.precipitationProbability = new List<int>();
    }
}
