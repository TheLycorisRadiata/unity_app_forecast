using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Forecast : MonoBehaviour
{
    [SerializeField] private LocationScriptableObject _location;
    [SerializeField] private Geocoding _geocoding;
    [SerializeField] private GameObject _spinner, _forecastExplanation, _forecastContent;
    private DateTimeFormatInfo _englishInfo;
    private float _previousLatitude, _previousLongitude;

    private void Start()
    {
        _englishInfo = new CultureInfo("en-US").DateTimeFormat;
        _previousLatitude = _location.Latitude;
        _previousLongitude = _location.Longitude;
    }

    /* OnClick event */
    public void GetForecast()
    {
        int i, j;
        TextMeshProUGUI dayElement;
        int nbrDay = 0, nbrYear = 0;
        string strDayEnding = "";
        string strMonth = "";
        float[] temperatures;
        int[] precipitationProbabilities;

        /* If coordinates are unchanged, don't fetch from API */
        if ((int)(Math.Round(_location.Latitude, 2) * 100) == (int)(Math.Round(_previousLatitude, 2) * 100) 
            && (int)(Math.Round(_location.Longitude, 2) * 100) == (int)(Math.Round(_previousLongitude, 2) * 100))
        {
            _geocoding.HideGeocoding();
            DisplayForecast();
            return;
        }
        else
        {
            _previousLatitude = _location.Latitude;
            _previousLongitude = _location.Longitude;
        }

        _geocoding.HideGeocoding();
        HideForecast();
        _spinner.SetActive(true);

        StartCoroutine(FetchOpenMeteoForecastData((days) =>
        {
            if (days == null || days.Length != 5 || days[0].time.Count != 4)
            {
                _spinner.SetActive(false);
                return;
            }

            for (i = 0; i < days.Length; ++i)
            {
                dayElement = _forecastContent.transform.Find($"Day ({i})").GetChild(0).GetComponent<TextMeshProUGUI>();
                nbrYear = 0;
                temperatures = new float[4];
                precipitationProbabilities = new int[4];

                for (j = 0; j < days[i].time.Count; ++j)
                {
                    if (nbrYear == 0)
                    {
                        nbrDay = days[i].time[j].Day;
                        strDayEnding = nbrDay == 1 || nbrDay == 21 || nbrDay == 31 ? "st" 
                                     : nbrDay == 2 || nbrDay == 22 ? "nd" 
                                     : nbrDay == 3 || nbrDay == 23 ? "rd" 
                                     : "th";
                        strMonth = _englishInfo.MonthNames[days[i].time[j].Month - 1];
                        nbrYear = days[i].time[j].Year;
                    }

                    temperatures[j] = days[i].temperature[j];
                    precipitationProbabilities[j] = days[i].precipitationProbability[j];
                }

                dayElement.text = $"<u>{strMonth} {nbrDay}{strDayEnding} {nbrYear}</u>\n\n" + 
                $"6 AM: {temperatures[0]}°C / {precipitationProbabilities[0]}%\n" + 
                $"Noon: {temperatures[1]}°C / {precipitationProbabilities[1]}%\n" + 
                $"6 PM: {temperatures[2]}°C / {precipitationProbabilities[2]}%\n" + 
                $"Midnight: {temperatures[3]}°C / {precipitationProbabilities[3]}%";
            }

            _spinner.SetActive(false);
            DisplayForecast();
        }));
    }

    public void HideForecast()
    {
        _spinner.SetActive(false);
        _forecastExplanation.SetActive(false);
        _forecastContent.SetActive(false);
    }

    public void DisplayForecast()
    {
        _forecastExplanation.SetActive(true);
        _forecastContent.SetActive(true);
    }

    private IEnumerator FetchOpenMeteoForecastData(Action<OmfHourly[]> callback)
    {
        string uri = $"https://api.open-meteo.com/v1/forecast?latitude={_location.Latitude}&longitude={_location.Longitude}&hourly=temperature_2m,precipitation_probability";
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
                Debug.LogWarning($"Forecast error: The request went through but the API couldn't find any data at these coordinates ({_location.Latitude},{_location.Longitude}).");
                yield break;
            }

            callback(forecast?.days);
        }
    }
}
