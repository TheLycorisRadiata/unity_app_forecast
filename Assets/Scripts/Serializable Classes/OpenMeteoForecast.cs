using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[Serializable]
public class OpenMeteoForecast
{
    public bool error;
    public OmfHourly hourly;

    public OpenMeteoForecast()
    {
        this.error = false;
        this.hourly = new OmfHourly();
    }

    public OpenMeteoForecast(string jsonText)
    {
        OpenMeteoForecast obj = JsonConvert.DeserializeObject<OpenMeteoForecast>(jsonText);
        this.error = obj?.error ?? false;

        if (obj?.hourly?.time != null)
        {
            /* Convert from UTC to local time */
            for (int i = 0; i < obj.hourly.time.Count; ++i)
                obj.hourly.time[i] = obj.hourly.time[i].ToLocalTime();

            /* Keep hours: 0, 6, 12, 18 --> Create a list of anonymous type */
            var indexesToRemove = obj.hourly.time.Select((item, index) => new { Item = item, Index = index })
                                                 .Where(e => e.Item.Hour != 0 && e.Item.Hour != 6 && e.Item.Hour != 12 && e.Item.Hour != 18)
                                                 .Select(e => e.Index)
                                                 .ToList();

            foreach (int index in indexesToRemove.OrderByDescending(i => i))
            {
                obj.hourly.time.RemoveAt(index);
                obj.hourly.temperature.RemoveAt(index);
                obj.hourly.precipitationProbability.RemoveAt(index);
            }
        }

        this.hourly = obj.hourly;
    }
}

public class OmfHourly
{
    /* Default: Lists of 120 elements, because the forecast is on 120 hours or 5 days */
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
