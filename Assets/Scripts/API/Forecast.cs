using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Forecast : MonoBehaviour
{
    [SerializeField] private LocationScriptableObject location;
    [SerializeField] private GameObject geocodingContent;
    [SerializeField] private Transform forecastContent;

    /* OnClick event */
    public void GetForecast()
    {
        int i;

        StartCoroutine(FetchOpenMeteoForecastData((data) =>
        {
            for (i = 0; i < data.time.Count; ++i)
                Debug.Log($"Day {data.time[i].Day} at {data.time[i].Hour}H - {data.temperature[i]}°C and precipitation probability of {data.precipitationProbability[i]}%");

            geocodingContent.SetActive(false);
            forecastContent.gameObject.SetActive(true);
        }));
    }

    private IEnumerator FetchOpenMeteoForecastData(Action<OmfHourly> callback)
    {
        string uri = $"https://api.open-meteo.com/v1/forecast?latitude={location.latitude}&longitude={location.longitude}&forecast_days=5&hourly=temperature_2m,precipitation_probability";
        string jsonText;
        OpenMeteoForecast forecast;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Forecast error: The request failed to fetch from the API.");
                yield break;
            }

            jsonText = webRequest.downloadHandler.text;
            forecast = new OpenMeteoForecast(jsonText);

            if (forecast.error == true)
            {
                Debug.LogWarning($"Forecast error: The request went through but the API couldn't find any data at these coordinates ({location.latitude},{location.longitude}).");
                yield break;
            }

            callback(forecast.hourly);
        }
    }
}
