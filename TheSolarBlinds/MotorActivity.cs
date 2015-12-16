
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

			var motor_state = 0;

			// Create your application here
			ImageButton blinds_up_btn = FindViewById<ImageButton> (Resource.Id.blinds_up_btn);
			ImageButton blinds_down_btn = FindViewById<ImageButton> (Resource.Id.blinds_down_btn);
			ImageButton blinds_stop_btn = FindViewById<ImageButton> (Resource.Id.blinds_stop_btn);

			// Metrics to format the button sizes
			var metrics = Resources.DisplayMetrics;
			var size_height = (metrics.HeightPixels / 4) + 20;
			var size_width = metrics.WidthPixels - 10;

			// Format the buttons
			blinds_up_btn.LayoutParameters.Width = size_width;
			blinds_up_btn.LayoutParameters.Height = size_height;
			blinds_down_btn.LayoutParameters.Width = size_width;
			blinds_down_btn.LayoutParameters.Height = size_height;
			blinds_stop_btn.LayoutParameters.Width = size_width;
			blinds_stop_btn.LayoutParameters.Height = size_height;

			blinds_up_btn.Click += delegate { //   Roll blinds up
				if (motor_state == 2 || motor_state == 1) {
					GattClientObserver.Instance.MotorBtnStop();
					Toast.MakeText(this, "Motor needs to stop before going up.", ToastLength.Short).Show();
					motor_state = 0;
				} else {
					GattClientObserver.Instance.MotorBtnUp();
					motor_state = 1;
				}
			};

			blinds_down_btn.Click += delegate { //   Roll blinds down
				if (motor_state == 1 || motor_state == 2) {
					GattClientObserver.Instance.MotorBtnStop();
					Toast.MakeText(this, "Motor needs to stop before going down.", ToastLength.Short).Show();
					motor_state = 0;
				} else {
					GattClientObserver.Instance.MotorBtnDown();
					motor_state = 2;
				}
			};

			blinds_stop_btn.Click += delegate { //   Stop blinds
				GattClientObserver.Instance.MotorBtnStop();
				motor_state = 0;
			};
		}
	}
}

