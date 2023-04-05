using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forecast : MonoBehaviour
{
    // https://api.open-meteo.com/v1/forecast?latitude=43.72&longitude=7.30&forecast_days=5&hourly=temperature_2m,precipitation_probability
    // 5 days or 120 hours
    // Time is in GMT+0 and in iso8601 format
    // Temperature is celcius and prob in %
}
