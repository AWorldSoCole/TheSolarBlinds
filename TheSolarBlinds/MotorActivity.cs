﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;

namespace TheSolarBlinds
{
	[Activity (Label = "MotorActivity")]			
	public class MotorActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.MotorLayout); // Link the layout

			// Create your application here
			ImageButton blinds_up_btn = FindViewById<ImageButton> (Resource.Id.blinds_up_btn);
			ImageButton blinds_down_btn = FindViewById<ImageButton> (Resource.Id.blinds_down_btn);
			ImageButton blinds_stop_btn = FindViewById<ImageButton> (Resource.Id.blinds_stop_btn);

			// Metrics to format the button sizes
			var metrics = Resources.DisplayMetrics;
			var size = (metrics.WidthPixels > metrics.HeightPixels) ? metrics.HeightPixels : metrics.WidthPixels;
			var size_height = (metrics.HeightPixels / 4) + 20;
			var size_width = metrics.WidthPixels - 10;

			// Format the buttons
//			blinds_up_btn.LayoutParameters.Width = size-10;
//			blinds_up_btn.LayoutParameters.Height = (size-10)/3;
//			blinds_down_btn.LayoutParameters.Width = size-10;
//			blinds_down_btn.LayoutParameters.Height = (size-10)/3;
//			blinds_stop_btn.LayoutParameters.Width = size-10;
//			blinds_stop_btn.LayoutParameters.Height = (size-10)/3;
			blinds_up_btn.LayoutParameters.Width = size_width;
//			blinds_up_btn.LayoutParameters.Height = size_height;
			blinds_down_btn.LayoutParameters.Width = size_width;
//			blinds_down_btn.LayoutParameters.Height = size_height;
			blinds_stop_btn.LayoutParameters.Width = size_width;
//			blinds_stop_btn.LayoutParameters.Height = size_height;

			blinds_up_btn.Click += delegate { //Roll blinds up
				GattClientObserver.Instance.MotorBtnUp();
			};

			blinds_down_btn.Click += delegate { //Roll blinds down
				GattClientObserver.Instance.MotorBtnDown();
			};

			blinds_stop_btn.Click += delegate { //Stop blinds
				GattClientObserver.Instance.MotorBtnStop();
			};
		}
	}
}

