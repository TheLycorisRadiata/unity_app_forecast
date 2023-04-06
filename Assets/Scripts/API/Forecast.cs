using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Forecast : MonoBehaviour
{
    [SerializeField] private LocationScriptableObject location;
    [SerializeField] private Geocoding geocoding;
    [SerializeField] private GameObject spinner, forecastExplanation, forecastContent;
    private CultureInfo frFrench;
    private DateTimeFormatInfo frenchInfo;
    private float previousLatitude, previousLongitude;

    void Start()
    {
        frFrench = new CultureInfo("fr-FR");
        frenchInfo = frFrench.DateTimeFormat;
        previousLatitude = location.latitude;
        previousLongitude = location.longitude;
    }

    /* OnClick event */
    public void GetForecast()
    {
        int i, j;
        TextMeshProUGUI dayElement;
        int nbrDay = 0, nbrYear = 0;
        string strMonth = "";
        float[] temperatures;
        int[] precipitationProbabilities;

        /* If coordinates are unchanged, don't fetch from API */
        if ((int)(Math.Round(location.latitude, 2) * 100) == (int)(Math.Round(previousLatitude, 2) * 100) 
            && (int)(Math.Round(location.longitude, 2) * 100) == (int)(Math.Round(previousLongitude, 2) * 100))
        {
            geocoding.HideGeocoding();
            DisplayForecast();
            return;
        }
        else
        {
            previousLatitude = location.latitude;
            previousLongitude = location.longitude;
        }

        geocoding.HideGeocoding();
        HideForecast();
        spinner.SetActive(true);

        StartCoroutine(FetchOpenMeteoForecastData((days) =>
        {
            if (days == null || days.Length != 5 || days[0].time.Count != 4)
            {
                spinner.SetActive(false);
                return;
            }

            for (i = 0; i < days.Length; ++i)
            {
                dayElement = forecastContent.transform.Find($"Day ({i})").GetChild(0).GetComponent<TextMeshProUGUI>();
                nbrYear = 0;
                temperatures = new float[4];
                precipitationProbabilities = new int[4];

                for (j = 0; j < days[i].time.Count; ++j)
                {
                    if (nbrYear == 0)
                    {
                        nbrDay = days[i].time[j].Day;
                        strMonth = frenchInfo.MonthNames[days[i].time[j].Month - 1];
                        nbrYear = days[i].time[j].Year;
                    }

                    temperatures[j] = days[i].temperature[j];
                    precipitationProbabilities[j] = days[i].precipitationProbability[j];
                }

                dayElement.text = $"<u>{nbrDay} {strMonth} {nbrYear}</u>\n\n" + 
                $"6H : {temperatures[0]}°C / {precipitationProbabilities[0]}%\n" + 
                $"Midi : {temperatures[1]}°C / {precipitationProbabilities[1]}%\n" + 
                $"18H : {temperatures[2]}°C / {precipitationProbabilities[2]}%\n" + 
                $"Minuit : {temperatures[3]}°C / {precipitationProbabilities[3]}%";
            }

            spinner.SetActive(false);
            DisplayForecast();
        }));
    }

    public void HideForecast()
    {
        spinner.SetActive(false);
        forecastExplanation.SetActive(false);
        forecastContent.SetActive(false);
    }

    public void DisplayForecast()
    {
        forecastExplanation.SetActive(true);
        forecastContent.SetActive(true);
    }

    private IEnumerator FetchOpenMeteoForecastData(Action<OmfHourly[]> callback)
    {
        string uri = $"https://api.open-meteo.com/v1/forecast?latitude={location.latitude}&longitude={location.longitude}&hourly=temperature_2m,precipitation_probability";
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

            callback(forecast?.days);
        }
    }
}
