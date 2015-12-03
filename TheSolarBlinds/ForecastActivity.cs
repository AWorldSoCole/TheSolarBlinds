
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Json;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Newtonsoft.Json;
using System.Threading;

namespace TheSolarBlinds {
	[Activity (Label = "ForecastActivity")]			
	public class ForecastActivity : Activity {
		private List<Date> forecastDates;
		private ListView forecast_list_view;

		protected override void OnCreate (Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView (Resource.Layout.ForecastLayout); // Link the layout

			string forecasst_city_id = Intent.GetStringExtra ("CityId") ;
			Button forecast_get_update_btn = FindViewById<Button>(Resource.Id.getForecastButton);
			forecast_list_view = FindViewById<ListView> (Resource.Id.forecast_list_view);

			forecast_get_update_btn.Click += async (sender, e) => {   // Update Forecast button will request Json from OpenWeatherMao
				string forecast_city_id_url = "http://api.openweathermap.org/data/2.5/forecast?id=" + 
					forecasst_city_id + 
					",us&appid=2de143494c0b295cca9337e1e96b00e0";   // Update API key
				JsonValue json = await FetchWeatherAsync (forecast_city_id_url);   // Fetch the weather information asynchronously.
				ParseAndDisplay(json);   // Parse the results, then update the screen.
			};
		}

		// Gets weather data from the passed zipcode url.
		// url: The city url, forecast_city_id_url
		private async Task<JsonValue> FetchWeatherAsync (string url) {
			// Create an HTTP web request using the URL:
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
			request.ContentType = "application/json";
			request.Method = "GET";
			// Send the request to the server and wait for the response:
			using (WebResponse response = await request.GetResponseAsync ()) {
				// Get a stream representation of the HTTP web response:
				using (Stream stream = response.GetResponseStream ()) {
					// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run (() => JsonObject.Load (stream));
					Console.Out.WriteLine("Response: {0}", jsonDoc.ToString ());
					return jsonDoc;
				}
			}
		}

		// Parse the weather data, then write temperature, humidity, conditions, and location to the screen.
		// json: The json returned from fetchWeatherAsync(forecast_city_id_url)
		private void ParseAndDisplay (JsonValue json) {

			forecastDates = new List<Date> ();
			int dateCount = 0;
			JsonValue weather_lists = json["list"][dateCount];
			JsonValue weather_results = weather_lists["weather"];
			JsonValue weather_main = weather_lists["main"];

			while (dateCount != 37) {   // While loop to add forecast to the listview
				// Parse the date/time 
				char[] delimiterChars = { ' ', ':', '-' };
				string forecast_date_text_array = weather_lists["dt_txt"];
				string[] forecast_date_text_parse = forecast_date_text_array.Split(delimiterChars);
				string forecast_month_day_text = forecast_date_text_parse [1] + "/" + forecast_date_text_parse [2];
				string forecast_time_text = forecast_date_text_parse [3] + ":" + forecast_date_text_parse [4];
				string main_text = weather_results[0]["main"];
				string forecast_description_text = weather_results[0]["description"];
				string forecast_icon_text = "condition_" + weather_results[0]["icon"];
				//				string forecast_image_url = "http://openweathermap.org/img/w/" + forecast_icon_text + ".png";
				string forecast_temperature_text;
				double weather_temperature_kelvin = weather_main["temp"];   // The temperature is expressed in Kevlin
				double weather_temperature_celsius = weather_temperature_kelvin - 273.15;   // Convert it to Celsius:
				double weather_temperature_fahrenheit = ((9.0 / 5.0) * weather_temperature_celsius) + 32;   // Convert it to Fahrenheit:
				forecast_temperature_text = String.Format("{0:F1}° F / {1:F1}° C", weather_temperature_fahrenheit, weather_temperature_celsius);   // Write the temperature (one decimal place) to the temperature TextBox

				forecastDates.Add (new Date () {   // Add the forecast data to a new date object
					monthDay = forecast_month_day_text,
					time = forecast_time_text,
					main = main_text,
					icon = forecast_icon_text,
					temp = forecast_temperature_text
				});

				DateAdapter adapter = new DateAdapter (this, forecastDates);   // Use a adapter to fill the listview
				forecast_list_view.Adapter = adapter;

				dateCount++;
				if (dateCount == 37)   // Increment the date count once it hits 37 break
					break;
				weather_lists = json["list"][dateCount];   // Update the lists and results of the json
				weather_results = weather_lists["weather"];
				weather_main = weather_lists["main"];
			}
		}
	}
}