
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

namespace TheSolarBlinds
{
	class DateAdapter : BaseAdapter<Date>
	{
		private List<Date> forecastdates;
		private Context mContext;

		public DateAdapter (Context context, List<Date> dates)
		{
			forecastdates = dates;
			mContext = context;
		}
		public override int Count
		{
			get{return forecastdates.Count;}
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override Date this[int position] {
			get {
				return forecastdates[position];
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View row = convertView;

			if (row == null)
			{
				row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ForecastListView, null, false);
			}

			TextView forecaste_date_text = row.FindViewById<TextView> (Resource.Id.forecaste_date_text);
			forecaste_date_text.Text = forecastdates [position].monthDay;

			TextView forecast_time_text = row.FindViewById<TextView> (Resource.Id.forecast_time_text);
			forecast_time_text.Text = forecastdates [position].time;

			var forecast_image_code = forecastdates [position].icon;
			ImageView forecast_image = row.FindViewById<ImageView> (Resource.Id.forecast_image);

			switch (forecast_image_code) 
			{
			case "condition_01d":
				forecast_image.SetImageResource (Resource.Drawable.condition_01d);
				break;
			case "condition_01n":
				forecast_image.SetImageResource (Resource.Drawable.condition_01n);
				break;
			case "condition_02d":
				forecast_image.SetImageResource (Resource.Drawable.condition_02d);
				break;
			case "condition_02n":
				forecast_image.SetImageResource (Resource.Drawable.condition_02n);
				break;
			case "condition_03d":
				forecast_image.SetImageResource (Resource.Drawable.condition_03d);
				break;
			case "condition_03n":
				forecast_image.SetImageResource (Resource.Drawable.condition_03n);
				break;
			case "condition_04d":
				forecast_image.SetImageResource (Resource.Drawable.condition_04d);
				break;
			case "condition_04n":
				forecast_image.SetImageResource (Resource.Drawable.condition_04n);
				break;
			case "condition_09d":
				forecast_image.SetImageResource (Resource.Drawable.condition_09d);
				break;
			case "condition_09n":
				forecast_image.SetImageResource (Resource.Drawable.condition_09n);
				break;
			case "condition_10d":
				forecast_image.SetImageResource (Resource.Drawable.condition_10d);
				break;
			case "condition_10n":
				forecast_image.SetImageResource (Resource.Drawable.condition_10n);
				break;
			case "condition_11d":
				forecast_image.SetImageResource (Resource.Drawable.condition_11d);
				break;
			case "condition_11n":
				forecast_image.SetImageResource (Resource.Drawable.condition_11n);
				break;
			case "condition_13d":
				forecast_image.SetImageResource (Resource.Drawable.condition_13d);
				break;
			case "condition_13n":
				forecast_image.SetImageResource (Resource.Drawable.condition_13n);
				break;
			case "condition_50d":
				forecast_image.SetImageResource (Resource.Drawable.condition_50d);
				break;
			case "condition_50n":
				forecast_image.SetImageResource (Resource.Drawable.condition_50n);
				break;
			default:
				break;
			}

			TextView forecast_temperature_text = row.FindViewById<TextView> (Resource.Id.forecast_temperature_text);
			forecast_temperature_text.Text = forecastdates [position].temp;

			TextView forecast_condition_text = row.FindViewById<TextView> (Resource.Id.forecast_condition_text);
			forecast_condition_text.Text = forecastdates [position].main;

			return row;
		}
	}
}