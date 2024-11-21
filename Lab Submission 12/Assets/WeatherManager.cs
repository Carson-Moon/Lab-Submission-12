using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class WeatherManager : MonoBehaviour
{

    private const string xmlApi = "http://api.openweathermap.org/data/2.5/weather?q=Orlando,us&mode=json&appid=5f176f7b14a493d1172bb12fe79ee37b";

    [Header("Current City")]
    [SerializeField] string cityName;
    [SerializeField] string weather;
    [SerializeField] string weatherDescription;
    [SerializeField] float temperature;
    private WeatherData wData;

    private void Start()
    {
        StartCoroutine(GetWeatherXML(ReadCallback));
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
        weather = parsedJson["weather"][0]["main"].ToString();
        weatherDescription = parsedJson["weather"][0]["description"].ToString();
        temperature = parsedJson["main"]["temp"].ToObject<float>() - 273.15f;       // Convert to celsius.

        // Create a new WeatherData to hold our new information.
        WeatherData newCity = new WeatherData(cityName, weather, weatherDescription, temperature);

        // Save and display our WeatherData.
        wData = newCity;
        print(cityName + "  " + weather + "  " + weatherDescription + "  " + temperature);
    }

#region API Call
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

    public IEnumerator GetWeatherXML(Action<string> callback)
    {
        return CallAPI(xmlApi, callback);
    }
#endregion

}

// Stores our desired weather data!
[System.Serializable]
public class WeatherData
{
    public string cityName;
    public string weather;
    public string weatherDescription;
    public float temperature;

    public WeatherData(string cityName, string weather, string weatherDescription, float temperature)
    {
        this.cityName = cityName;
        this.weather = weather;
        this.weatherDescription = weatherDescription;
        this.temperature = temperature;
    }
}