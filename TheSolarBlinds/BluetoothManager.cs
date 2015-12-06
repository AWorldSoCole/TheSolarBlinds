using System;
using Android.Bluetooth;
using Java.IO;
using Java.Util;

namespace TheSolarBlinds
{
	public class BluetoothManager
	{
		// Unique ID which help us connect to any device 
		private const string UUID_UNIVERSAL_PROFILE = "a88667e4-f4bf-46cd-9563-4f83c645e517";

		// Represent bluetooth data coming from UART
		private BluetoothDevice result;

		// Get input/output stream of this communication
		private BluetoothSocket mSocket;

		// Convert byte[] to readable strings
		private BufferedReader reader;

		private System.IO.Stream mStream;
		private InputStreamReader mReader;


		public BluetoothManager () {
			reader = null;
		}

		private UUID getUUIDFromString() {
			return UUID.FromString (UUID_UNIVERSAL_PROFILE);
		}

		private void close (IDisposable aConnectedObject) {
			if (aConnectedObject == null) {
				return;
			}
			try {
				aConnectedObject.Dispose();
			} catch (Exception e) {
				throw e;
			}
			aConnectedObject = null;
		}

		public string getDataFromDevice() {
			return reader.ReadLine ();
		}

		private void openDeviceConnnection(BluetoothDevice btDevice) {
			try {
				// Getting socket from specific device
				mSocket = btDevice.CreateRfcommSocketToServiceRecord(getUUIDFromString());

				// Blocking operation
				mSocket.ConnectAsync();

				// Input stream
				mStream = mSocket.InputStream;

				// Output stream
				// mSocket.OutputStream
				mReader = new InputStreamReader (mStream);
				reader = new BufferedReader(mReader);
			} catch (IOException e) {
				// Close all 
				close (mSocket);
				close (mStream);
				close (mReader);
				throw e;
			}
		}

		public void getAllPairedDevices() {
			BluetoothAdapter btAdapter = BluetoothAdapter.DefaultAdapter;
			var devices = btAdapter.BondedDevices;

			if (devices != null && devices.Count > 0) {

				// Search throughout all devices
				foreach (BluetoothDevice mDevice in devices) {
					if (mDevice.Address == "C4:BE:84:E9:02:04") {
						openDeviceConnnection (mDevice);
					}
				}
			}
		}
	}
}

