
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

namespace TheSolarBlinds
{
	[Activity (Label = "WeatherActivity")]			
	public class WeatherActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set the screen layout
			SetContentView (Resource.Layout.WeatherLayout); // Link the layout

			// Attach variables to a specific layout object
			EditText weather_zipcode_text = FindViewById<EditText>(Resource.Id.weather_zipcode_text);
			Button weather_get_update_btn = FindViewById<Button>(Resource.Id.weather_get_update_btn);
			Button weather_forecast_btn = FindViewById<Button>(Resource.Id.weather_forecast_btn);
			Button weather_view_battery_btn = FindViewById<Button>(Resource.Id.weather_view_battery_btn);
			string weather_city_id = "0"; 

			// Get current weather json using a zipcode on button press
			weather_get_update_btn.Click += async (sender, e) => {   // Weather button will request Json from OpenWeatherMao
				string weather_zipcode_url = "http://api.openweathermap.org/data/2.5/weather?zip=" + 
					weather_zipcode_text.Text + 
					",us&appid=2de143494c0b295cca9337e1e96b00e0";   // Update APIkey
				JsonValue json = await fetchWeatherAsync (weather_zipcode_url);   // Fetch the weather information asynchronously.
				parseAndDisplay(json);   // Parse the results, then update the screen.
				JsonValue idResults = json["id"];   // Store json city id to be passed to the forecast screen.
				weather_city_id = idResults.ToString();
			};

			weather_forecast_btn.Click += delegate {   // Forecast button will request json from OpenWeatherMap using the stored city id
//				var forecastIntent = new Intent (this, typeof(ForecastActivity));
//				forecastIntent.PutExtra ("CityId", weather_city_id);
//				//				Console.WriteLine(weather_city_id);
//				StartActivity(forecastIntent);
			};
		}

		// Gets weather data from the passed zipcode url.
		// url: The zipcode url, weather_zipcode_url
		private async Task<JsonValue> fetchWeatherAsync (string url) {
			// Create an HTTP web request using the URL
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
		// json: The json returned from fetchWeatherAsync(weather_zipcode_url)
		private void parseAndDisplay (JsonValue json) {
			TextView weather_city_text = FindViewById<TextView>(Resource.Id.weather_current_city_text);
			TextView weather_temperature_text = FindViewById<TextView>(Resource.Id.weather_current_temperature_text);
			TextView weather_conditions_text = FindViewById<TextView>(Resource.Id.weather_current_condition_text);
			ImageView weather_image = FindViewById<ImageView>(Resource.Id.weather_current_weather_image);

			// Extract the array of name/value results for the field name "(field_name)". 
			JsonValue weather_results = json["weather"];
			JsonValue main_results = json["main"];
			JsonValue id_results = json["id"];
			JsonValue name_results = json["name"];

			weather_city_text.Text = name_results;   // Extract the "stationName" and write it to the location TextBox
			//			int id;
			//			id = id_results;

			double weather_temperature_kelvin = main_results["temp"];   // The temperature is expressed in Kevlin
			double weather_temperature_celsius = weather_temperature_kelvin - 273.15;   // Convert it to Celsius
			double weather_temperature_fahrenheit = ((9.0 / 5.0) * weather_temperature_celsius) + 32;   // Convert it to Fahrenheit

			weather_temperature_text.Text = String.Format("{0:F1}° F / {1:F1}° C", weather_temperature_fahrenheit, weather_temperature_celsius);   // Write the temperature (one decimal place) to the temperature TextBox

			// Get the "clouds" and "weatherConditions" strings and combine them.
			string weather_json_conditon = weather_results[0]["main"];
			string weather_json_description = weather_results[0]["description"];
			weather_conditions_text.Text = weather_json_conditon + ": " + weather_json_description;   // Write the result to the conditions TextBox

			// Get the conditions image and display it (optimize)
			//			string weather_image_code = weather_results[0]["icon"];   // Optimized using the saved images
			string weather_image_code = "condition_" + weather_results [0] ["icon"];
			string weather_image_url = "http://openweathermap.org/img/w/" + weather_image_code + ".png";
			//Console.WriteLine (imageURL);
			//			var weather_image_bitmap = GetImageBitmapFromUrl(weather_image_url);
			//			weather_image.SetImageBitmap(weather_image_bitmap);

			switch (weather_image_code) 
			{
			case "condition_01d":
				weather_image.SetImageResource (Resource.Drawable.condition_01d);
				break;
			case "condition_01n":
				weather_image.SetImageResource (Resource.Drawable.condition_01n);
				break;
			case "condition_02d":
				weather_image.SetImageResource (Resource.Drawable.condition_02d);
				break;
			case "condition_02n":
				weather_image.SetImageResource (Resource.Drawable.condition_02n);
				break;
			case "condition_03d":
				weather_image.SetImageResource (Resource.Drawable.condition_03d);
				break;
			case "condition_03n":
				weather_image.SetImageResource (Resource.Drawable.condition_03n);
				break;
			case "condition_04d":
				weather_image.SetImageResource (Resource.Drawable.condition_04d);
				break;
			case "condition_04n":
				weather_image.SetImageResource (Resource.Drawable.condition_04n);
				break;
			case "condition_09d":
				weather_image.SetImageResource (Resource.Drawable.condition_09d);
				break;
			case "condition_09n":
				weather_image.SetImageResource (Resource.Drawable.condition_09n);
				break;
			case "condition_10d":
				weather_image.SetImageResource (Resource.Drawable.condition_10d);
				break;
			case "condition_10n":
				weather_image.SetImageResource (Resource.Drawable.condition_10n);
				break;
			case "condition_11d":
				weather_image.SetImageResource (Resource.Drawable.condition_11d);
				break;
			case "condition_11n":
				weather_image.SetImageResource (Resource.Drawable.condition_11n);
				break;
			case "condition_13d":
				weather_image.SetImageResource (Resource.Drawable.condition_13d);
				break;
			case "condition_13n":
				weather_image.SetImageResource (Resource.Drawable.condition_13n);
				break;
			case "condition_50d":
				weather_image.SetImageResource (Resource.Drawable.condition_50d);
				break;
			case "condition_50n":
				weather_image.SetImageResource (Resource.Drawable.condition_50n);
				break;
			default:
				break;
			}
		}

		//		private Bitmap GetImageBitmapFromUrl(string url) {
		//			Bitmap imageBitmap = null;
		//
		//			using (var webClient = new WebClient()) {
		//				var imageBytes = webClient.DownloadData(url);
		//				if (imageBytes != null && imageBytes.Length > 0) {
		//					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
		//				}
		//			}
		//			return imageBitmap;
		//		}
	}
}