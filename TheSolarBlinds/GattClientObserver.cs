using System;
using Android.Bluetooth;
using Android.Content;
using Android.Widget;
using Java.Util;
using System.Linq;

namespace TheSolarBlinds
{
	public class GattClientObserver : BluetoothGattCallback
	{
		private static GattClientObserver _instance;
		private static BluetoothGattCharacteristic _gatt_motor_characteristic;
		private static BluetoothGatt _devicegatt;
		private static BluetoothDevice _device;
		private static ProfileState _state;

		public BluetoothGattCharacteristic gatt_motor_characteristic 
		{
			get { return _gatt_motor_characteristic; }
			set { _gatt_motor_characteristic = value; }
		}

		public BluetoothGatt devicegatt 
		{
			get { return _devicegatt; }
			set { _devicegatt = value; }
		}

		public BluetoothDevice device
		{
			get { return _device; }
			set { _device = value; }
		}

		public ProfileState state
		{
			get { return _state; }
			set { _state = value; }
		}

		public static GattClientObserver Instance
		{
			get { return _instance ?? (_instance = new GattClientObserver ()); }
		}

		public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status) {
			Console.WriteLine(status != GattStatus.Success ? "Failed to discover device services" : "Successfully discovered device services");
			if (status != GattStatus.Success) {
				return;
			}
			GattClientObserver.Instance.devicegatt = gatt;
			foreach(var service in gatt.Services) {
				Console.WriteLine("Service with adress: " + service.Uuid);
				foreach(var characteristic in service.Characteristics) {
					Console.WriteLine("Has characteristic with adress: " + characteristic.Uuid);
					IsEncryptedAndWritable (characteristic);
					// Device Name
//					if (characteristic.Uuid.ToString() == "00002a00-0000-1000-8000-00805f9b34fb") {
//						Console.WriteLine("Charateristic Data is: " + Read(characteristic, gatt));
//					}
					// Heart Rate Measurement 
//					if (characteristic.Uuid.ToString() == "00002a39-0000-1000-8000-00805f9b34fb") {
//						Console.WriteLine("Charateristic Data is: " + Read(characteristic, gatt));
//						GattClientObserver.Instance.gatt_motor_characteristic = characteristic;
//					}
//					if (characteristic.Uuid.ToString() == "f000ee01-0451-4000-b000-000000000000") {
//						Console.WriteLine("Charateristic Data is: " + Read(characteristic, gatt));
//					}
					// SolarBlinds Motor Characteristic
					if (characteristic.Uuid.ToString() == "0000fff1-0000-1000-8000-00805f9b34fb") {
						GattClientObserver.Instance.gatt_motor_characteristic = characteristic;
						Console.WriteLine("Charateristic Data is: " + Read(characteristic, gatt));
					}
//					// SolarBlinds Motor Characteristic Phone App
//					if (characteristic.Uuid.ToString() == "0000fff1-00451-4000-B000-000000000000") {
//						GattClientObserver.Instance.gatt_motor_characteristic = characteristic;
//						Console.WriteLine("Charateristic Data is: " + Read(characteristic, gatt));
//					}
					// SolarBlinds Temperature Characteristic
					if (characteristic.Uuid.ToString() == "0000fff5-0000-1000-8000-00805f9b34fb") {
						Console.WriteLine ("Subscribing to this charateristic");
						SubscribeCharacteristic (characteristic, gatt);
					}
//					Console.WriteLine("Charateristic Data is: " + Read(characteristic, gatt));
				}
			}
		}

		public bool IsEncryptedAndWritable(BluetoothGattCharacteristic characteristic) {
			var isWritable = characteristic.Properties.HasFlag(GattProperty.Write);
			var isEncrypted = characteristic.Permissions.HasFlag(GattPermission.WriteEncrypted);
			return isWritable && isEncrypted;
		}

		public bool Read(BluetoothGattCharacteristic readCharacteristic, BluetoothGatt gatt) {
			return gatt.ReadCharacteristic(readCharacteristic);
		}
			
		public void WriteValueInternal(byte[] buffer, BluetoothGatt gatt, BluetoothGattCharacteristic characteristic) { 
			//Set value that will be written 
			characteristic.SetValue(buffer); 
			//Set writing type 
			characteristic.WriteType = GattWriteType.Default; 
			gatt.WriteCharacteristic(characteristic); 
		}

		public void SubscribeCharacteristic(BluetoothGattCharacteristic characteristic, BluetoothGatt gatt) {
			gatt.SetCharacteristicNotification(characteristic, true);
//			var descriptor = characteristic.GetDescriptor(UUID.FromString("0000fff4-0000-1000-8000-00805f9b34fb"));
//			descriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray());
//			gatt.WriteDescriptor(descriptor);
		}

		public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
		{
			switch (newState)
			{
			case ProfileState.Connected:
				Console.WriteLine ("Connected peripheral: " + gatt.Device.Name);
//				Toast.MakeText (Android.Content.Context, "Bluetooth Connected", ToastLength.Short).Show ();
//				SyncActivity.sync_set_message.Text = "Device is connected";
				GattClientObserver.Instance.state = newState;
				gatt.DiscoverServices ();
				break;
			case ProfileState.Disconnected:
				Console.WriteLine ("Disconnected peripheral: " + gatt.Device.Name);
				GattClientObserver.Instance.state = newState;
//				SyncActivity.sync_set_message.Text = "Device is connected";
//				gatt.Disconnect ();
				//After connection state changed to disconnected close gatt
				gatt.Close();
				break;
			case ProfileState.Connecting:
				Console.WriteLine("Connecting peripheral: " + gatt.Device.Name);
				GattClientObserver.Instance.state = newState;
				break;
			case ProfileState.Disconnecting:
				Console.WriteLine("Disconnecting peripheral: " + gatt.Device.Name);
				GattClientObserver.Instance.state = newState;
				break;
			}
		}

		public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
		{
			
		}

		public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status) {
			if (status != GattStatus.Success) {
				Console.WriteLine("Failed to read characteristic: " + characteristic.Uuid);
				return;
			} 
			//Read value from device 
			byte[] readValue = characteristic.GetValue();
			var str = System.Text.Encoding.UTF8.GetString (readValue, 0, readValue.Length);
//			byte[] test = { 0x61, 0x62, 0x63, 0x64 };
//			foreach(byte place in test) {
//				Console.WriteLine(place);
//			}
			foreach(byte place in readValue) {
				Console.WriteLine(place);
			}
			Console.WriteLine ("Data: " + str);
			//Process received value...
		}

		public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status) {
			if (status == GattStatus.Success) {
				Console.WriteLine("Writing was successfull");
				Read (characteristic, gatt);
			} else {
				var errorCode = status; 
				Console.WriteLine("Writing was unsuccessfull: Error Code " +  errorCode.ToString());
				//Process error... 
			} 
		}

		public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic) {
			var newValue = characteristic.GetValue(); 
			//Process value...
			Console.WriteLine("Characteristic has changed");
		}

		public void MotorBtnUp ()
		{
			byte[] motorUp = { 0x01 };
			WriteValueInternal (motorUp, GattClientObserver.Instance.devicegatt, GattClientObserver.Instance.gatt_motor_characteristic);
		}

		public void MotorBtnDown ()
		{
			byte[] motorUp = { 0x02 };
			WriteValueInternal (motorUp, GattClientObserver.Instance.devicegatt, GattClientObserver.Instance.gatt_motor_characteristic);
		}

		public void MotorBtnStop ()
		{
			byte[] motorUp = { 0x00 };
			WriteValueInternal (motorUp, GattClientObserver.Instance.devicegatt, GattClientObserver.Instance.gatt_motor_characteristic);
		}
	}
}

