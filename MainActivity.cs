using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace TheSolarBlinds
{
	[Activity (Label = "TheSolarBlinds", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get the button from the layout resource
			ImageButton menu_motor_btn = FindViewById<ImageButton> (Resource.Id.menu_motor_btn);
			ImageButton menu_sync_btn = FindViewById<ImageButton> (Resource.Id.menu_sync_btn);
			ImageButton menu_weather_btn = FindViewById<ImageButton> (Resource.Id.menu_weather_btn);

			// Metrics to format the button sizes
			var metrics = Resources.DisplayMetrics;
			var size = (metrics.WidthPixels > metrics.HeightPixels) ? metrics.HeightPixels : metrics.WidthPixels;

			// Format the buttons
			menu_motor_btn.LayoutParameters.Width = (size/2)-10;
			menu_motor_btn.LayoutParameters.Height = (size/4)-10;
			menu_sync_btn.LayoutParameters.Width = (size/2)-10;
			menu_sync_btn.LayoutParameters.Height = (size/4)-10;
			menu_weather_btn.LayoutParameters.Width = (size/2)-10;
			menu_weather_btn.LayoutParameters.Height = (size/4)-10;

			menu_motor_btn.Click += delegate { //Motor button will launch Motor activity
				StartActivity(typeof(MotorActivity));
			};

			menu_sync_btn.Click += delegate { //Sync button will launch Sync activity
				StartActivity(typeof(SyncActivity));
			};

			menu_weather_btn.Click += delegate { //Weather button will launch Weather activity
				StartActivity(typeof(WeatherActivity));
			};
		}
	}
}


