using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;

public class WeatherManager : MonoBehaviour
{
    public EnvironmentManager eManager;

    private const string jsonApi = "http://api.openweathermap.org/data/2.5/weather?q=Orlando,us&mode=json&appid=5f176f7b14a493d1172bb12fe79ee37b";

    [Header("Current City")]
    [SerializeField] string cityName;
    [SerializeField] int weatherCode;
    [SerializeField] string weather;
    [SerializeField] string weatherDescription;
    [SerializeField] float temperature;
    private WeatherData wData;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI currentCity;
    [SerializeField] TextMeshProUGUI currentWeather;
    [SerializeField] TextMeshProUGUI currentTimeOfDay;

    private void Start()
    {
        StartCoroutine(GetWeatherJson(ReadCallback, jsonApi));
    }

    private void ReadCallback(string message){
        print(message);

        // Read our json data.
        ReadWeatherData(message);
    }

    // Parse our data into a new WeatherData.
    private void ReadWeatherData(string json){
        // Create a JObject to parse our json into for easy extraction.
        JObject parsedJson = JObject.Parse(json);

        // Grab our desired information.
        cityName = parsedJson["name"].ToString();
        weatherCode = parsedJson["weather"][0]["id"].ToObject<int>();
        weather = parsedJson["weather"][0]["main"].ToString();
        weatherDescription = parsedJson["weather"][0]["description"].ToString();
        temperature = parsedJson["main"]["temp"].ToObject<float>() - 273.15f;       // Convert to celsius.

        // Calculate the time of day.
        string timeOfDay = "Day";
        long currentTime = parsedJson["dt"].ToObject<long>();
        long sunriseTime = parsedJson["sys"]["sunrise"].ToObject<long>();
        long sunsetTime = parsedJson["sys"]["sunset"].ToObject<long>();

        if(currentTime >= sunriseTime && currentTime < sunsetTime)
        {
            timeOfDay = "Day";
        }
        else
        {
            timeOfDay = "Night";
        }

        // Create a new WeatherData to hold our new information.
        WeatherData newCity = new WeatherData(cityName, weatherCode, weather, weatherDescription, temperature, timeOfDay);

        // Save and display our WeatherData.
        wData = newCity;
        print(cityName + "  " + weatherCode + "  " + weather + "  " + weatherDescription + "  " + temperature + "  " + timeOfDay);
    
        // Update our UI.
        currentCity.text = cityName.ToUpper();
        currentWeather.text = weather.ToUpper();
        currentTimeOfDay.text = timeOfDay.ToUpper();

        // Change our environment.
        eManager.DetermineEnvironment(weatherCode, timeOfDay);
    }

#region API Call
    public void GetWeatherData(string cityURL)
    {
        StartCoroutine(GetWeatherJson(ReadCallback, cityURL));
    }

    private IEnumerator CallAPI(string url, Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"network problem: {request.error}");
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"response error: {request.responseCode}");
            }
            else
            {
                callback(request.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetWeatherJson(Action<string> callback, string cityURL)
    {
        return CallAPI(cityURL, callback);
    }
#endregion

}

// Stores our desired weather data!
[System.Serializable]
public class WeatherData
{
    public string cityName;
    public int weatherCode;
    public string weather;
    public string weatherDescription;
    public float temperature;
    public string timeOfDay;

    public WeatherData(string cityName, int weatherCode, string weather, string weatherDescription, float temperature, string timeOfDay)
    {
        this.cityName = cityName;
        this.weatherCode = weatherCode;
        this.weather = weather;
        this.weatherDescription = weatherDescription;
        this.temperature = temperature;
        this.timeOfDay = timeOfDay;
    }
}