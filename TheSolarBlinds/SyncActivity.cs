
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;

namespace TheSolarBlinds{
	[Activity (Label = "Sync")]			
	public class SyncActivity : Activity{

		// Variables
		private ListView sync_list_view;
		DBRepository dbr;
		EditText sync_set_id, sync_set_nickname, sync_set_device_id;
		Button sync_btn, sync_delete_btn, sync_set_btn, sync_save_btn;
		TextView sync_set_message;
		ToggleButton sync_read_write_btn;
		NfcAdapter nfcAdapter;
		BluetoothAdapter mBluetoothAdapter;
		BluetoothDevice bt_peripheral;
		BluetoothGatt bt_peripheral_connectedGATT;
//		BluetoothLeService mBluetoothLeService;
		public static BluetoothGatt mBluetoothGatt;
		public static BluetoothGattCallback mBluetoothGattCallBack;
		Handler mHandler;
		static readonly int REQUEST_ENABLE_BT = 1;
		// Stops scanning after 10 seconds.
		static readonly long SCAN_PERIOD = 10000;

		protected override void OnCreate (Bundle bundle){
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.SyncLayout); // Link the layout

			// Check for NFC adapter on android phone
			nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
			if (nfcAdapter != null && nfcAdapter.IsEnabled)
				Toast.MakeText (this, "NFC is available!", ToastLength.Short).Show ();
			else
				Toast.MakeText (this, "NFC is not available!", ToastLength.Short).Show ();

			Console.WriteLine ("Button Check 1");
			// Initializes a Bluetooth adapter.  For API level 18 and above, get a reference to
			// BluetoothAdapter through BluetoothManager.
//			BluetoothManager bluetoothManager = (BluetoothManager) GetSystemService (Context.BluetoothService);
//			mBluetoothAdapter = bluetoothManager.Adapter;

			// Checks if Bluetooth is supported on the device.
			if (mBluetoothAdapter != null && mBluetoothAdapter.IsEnabled) {
				Toast.MakeText (this, "Bluetooth is available", ToastLength.Short).Show();
			} else
				Toast.MakeText (this, "Bluetooth is not supported", ToastLength.Short).Show();

			Console.WriteLine ("Button Check 2");
			// Use this check to determine whether BLE is supported on the device.  Then you can
			// selectively disable BLE-related features.
			if (!PackageManager.HasSystemFeature (Android.Content.PM.PackageManager.FeatureBluetoothLe)) {
				Toast.MakeText (this, "Bluetooth LE not supported", ToastLength.Short).Show ();
			}

			Console.WriteLine ("Button Check 3");
			// Create a database to manage the solar blinds
			dbr = new DBRepository ("IdNickname");

			// Get resoucres for the layout of the device
			sync_btn = FindViewById<Button> (Resource.Id.sync_btn);
			sync_delete_btn = FindViewById<Button> (Resource.Id.sync_delete_btn);
			sync_set_btn = FindViewById<Button> (Resource.Id.sync_set_btn);
			sync_save_btn = FindViewById<Button> (Resource.Id.sync_save_btn);
			sync_set_id = FindViewById<EditText> (Resource.Id.sync_set_id);
			sync_set_nickname = FindViewById <EditText> (Resource.Id.sync_set_nickname);
			sync_set_device_id = FindViewById<EditText> (Resource.Id.sync_set_device_id);
			sync_set_message = FindViewById<TextView> (Resource.Id.sync_set_message);
			sync_read_write_btn = FindViewById<ToggleButton> (Resource.Id.sync_read_write_btn);
			sync_list_view = FindViewById<ListView> (Resource.Id.sync_list_view);

			// Initalize the list view
			getCursorView ();

			// Button Clicks 
			sync_btn.Click += syncBtnClick;
			sync_delete_btn.Click += syncDeleteBtnClick;
			sync_set_btn.Click += syncSetBtnClick;
			sync_save_btn.Click += syncSaveBtnClick;
			sync_list_view.ItemClick += syncListViewItemClick;
			sync_read_write_btn.Click += syncReadWriteBtnClick;
		}

		void syncReadWriteBtnClick (object sender, EventArgs e)
		{
			sync_set_message.Text = " ";
		}

		// The user can press the save button ot update a device name
		void syncSaveBtnClick (object sender, EventArgs e)
		{
			int iId = -1;
			int.TryParse (sync_set_id.Text, out iId);
			dbr.updateRecord (iId, sync_set_nickname.Text);
			sync_set_message.Text = dbr.message;
			sync_set_id.Text = sync_set_nickname.Text = sync_set_device_id.Text = "";
			getCursorView ();
		}

		// The user can use the set button to resync the phone with the solar blinds
		void syncSetBtnClick (object sender, EventArgs e)
		{
			Console.WriteLine ("Button pushed");
		}

		// The user can press the delete button to remove a 
		void syncDeleteBtnClick (object sender, EventArgs e)
		{
			int iId = -1;
			int.TryParse (sync_set_id.Text, out iId);
			dbr.removeRecord (iId);
			sync_set_message.Text = dbr.message;
			sync_set_id.Text = sync_set_nickname.Text = sync_set_device_id.Text = "";
			getCursorView ();
		}

		// The actions that happen when a user clicks on the listview 
		void syncListViewItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			TextView sync_id = e.View.FindViewById<TextView> (Resource.Id.sync_id);
			TextView sync_nickname = e.View.FindViewById <TextView> (Resource.Id.sync_nickname);
			TextView sync_device_id = e.View.FindViewById<TextView> (Resource.Id.sync_device_id);

			sync_set_id.Text = sync_id.Text;
			sync_set_nickname.Text = sync_nickname.Text;
			sync_set_device_id.Text = sync_device_id.Text;
		}


		void syncBtnClick (object sender, EventArgs e)
		{
			int count = 1;
			string data = null;
			BluetoothManager manager = new BluetoothManager ();
			manager.getAllPairedDevices ();

			Thread thread = new Thread(() =>
				{
					while (true) {
						data = manager.getDataFromDevice();
					}
				});
			thread.IsBackground = true;
			thread.Start ();

			Console.WriteLine ("Sync Button pushed");
			Toast.MakeText (this, "SyncButton Pressed!", ToastLength.Short).Show ();
			sync_read_write_btn = FindViewById<ToggleButton> (Resource.Id.sync_read_write_btn);
			sync_set_message = FindViewById<TextView> (Resource.Id.sync_set_message);
			Console.WriteLine("Testing the Bluetooth!!!");

			// When read is selected begin reading and parsing the NFC tag
			if (sync_read_write_btn.Checked) {
				Console.WriteLine ("read/write button is checked");
				//				IParcelable[] parcelables = intent.GetParcelableArrayExtra (NfcAdapter.ExtraNdefMessages);

				//				if (parcelables != null && parcelables.Length > 0) {
				Console.WriteLine ("Tag has data");
				//					readTextFromMessages ((NdefMessage)parcelables[0]);
				//					NdefMessage ndefmessage = (NdefMessage) parcelables[0];

				// MAC Address from NFC tag 
				//					sync_set_message.Text = Encoding.UTF8.GetString(ndefmessage.GetRecords()[0].GetPayload());
				sync_set_message.Text = "C4:BE:84:E9:02:04";
				Console.WriteLine ("MAC Address: " + sync_set_message.Text + " Valid: " + BluetoothAdapter.CheckBluetoothAddress (sync_set_message.Text));

				// Get the Bluetooth device
//				bt_peripheral = mBluetoothAdapter.GetRemoteDevice ("C4:BE:84:E9:02:04");
//				Console.WriteLine ("Device Name: " + bt_peripheral.Name + " " + "Device Address: " + bt_peripheral.Address);
//				var listOfDevices = mBluetoothAdapter.BondedDevices;
//				Console.WriteLine (listOfDevices);
//				bluetoothSocket = bt_peripheral.CreateRfcommSocketToServiceRecord(BluetoothService.SerialPort);
			}
		}

		protected override void OnNewIntent(Intent intent) {
			// NFC Tag has been scanned
			Toast.MakeText (this, "NFC intent receieved!", ToastLength.Short).Show ();
			sync_read_write_btn = FindViewById<ToggleButton> (Resource.Id.sync_read_write_btn);
			sync_set_message = FindViewById<TextView> (Resource.Id.sync_set_message);
			Console.WriteLine("Testing the NFC!!!");

			// When read is selected begin reading and parsing the NFC tag
			if (sync_read_write_btn.Checked) {
				Console.WriteLine("read/write button is checked");
//				IParcelable[] parcelables = intent.GetParcelableArrayExtra (NfcAdapter.ExtraNdefMessages);

//				if (parcelables != null && parcelables.Length > 0) {
					Console.WriteLine("Tag has data");
					//					readTextFromMessages ((NdefMessage)parcelables[0]);
//					NdefMessage ndefmessage = (NdefMessage) parcelables[0];

					// MAC Address from NFC tag 
//					sync_set_message.Text = Encoding.UTF8.GetString(ndefmessage.GetRecords()[0].GetPayload());
					sync_set_message.Text = "C4:BE:84:E9:02:04";
					Console.WriteLine("MAC Address: " + sync_set_message.Text + "Valid: " + BluetoothAdapter.CheckBluetoothAddress (sync_set_message.Text));

					// Get the Bluetooth device
					bt_peripheral = mBluetoothAdapter.GetRemoteDevice (sync_set_message.Text);
					Console.WriteLine("Device Name: " + bt_peripheral.Name + " " + "Device Address: " + bt_peripheral.Address);
//				Console.WriteLine(mBluetoothAdapter.BondedDevices);
				var paired_devices = mBluetoothAdapter.BondedDevices;

//				if (DevicePolicyService != null && paired_devices > 0) 
				{	
					// Search throughout all devices
					foreach (BluetoothDevice mDevice in paired_devices) {
//						mDevice.Name.Split
					}
				
				}

					// Different attempts to connect to BLE Device
					//					mBluetoothLeService.Connect (Encoding.UTF8.GetString(ndefmessage.GetRecords()[0].GetPayload()));
					//					bt_peripheral_connectedGATT = bt_peripheral.ConnectGatt (this, true, mBluetoothGattCallBack);
					//					BluetoothSocket socket;
					//					socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
					//					await _socket.ConnectAsync();

//				} else {
//					Toast.MakeText (this, "No ndef messages found", ToastLength.Short).Show ();
//					Console.WriteLine("read/write button check 3");
//				}

				// Else wrtie to the NFC tag the hard coded string
			} else {
//				var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
//				NdefMessage ndefmessage = CreateNdefMessage ("C4:BE:84:E9:02:04");
//				Console.WriteLine("read/write button check 4");

//				writeNdefMessage (tag, ndefmessage);

//				if (tag == null) {
//					Toast.MakeText (this, "NFC tag has no data", ToastLength.Short).Show ();
//				}
			}
			base.OnNewIntent (intent);
		}

		private void readTextFromMessages(NdefMessage ndefmessage) {
			NdefRecord[] ndefrecords = ndefmessage.GetRecords ();

			if (ndefrecords != null && ndefrecords.Length > 0) {
				//				NdefRecord ndefrecord = ndefrecords [0];

				//				string tag_context = GetTextFromNdefRecord (ndefrecord);
				//				string tag_context = Encoding.UTF8.GetString(ndefrecord.GetPayload);

				//				sync_set_message.Text = tag_context;
			} else {
				Toast.MakeText (this, "No NDEF records found!", ToastLength.Short).Show ();
			}
		}

		protected override void OnResume() {
			// NFC tag has been discovered
//			var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
//			var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
//			intent.AddFlags(ActivityFlags.SingleTop);
//			var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);
//			var filters = new[] { tagDetected };
//			nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);

			// Ensures Bluetooth is enabled on the device.  If Bluetooth is not currently enabled,
			// fire an intent to display a dialog asking the user to grant permission to enable it.
//			if (!mBluetoothAdapter.IsEnabled) {
				if ( 1 == 4) { // Refer to the line above for the if statement
					Intent enableBtIntent = new Intent (BluetoothAdapter.ActionRequestEnable);
					StartActivityForResult (enableBtIntent, REQUEST_ENABLE_BT);
				}
//			}
			base.OnResume ();
		}

		// Not sure if needed
		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			// User chose not to enable Bluetooth.
			if (requestCode == REQUEST_ENABLE_BT && resultCode == Result.Canceled) {
				Finish ();
				return;
			}
			base.OnActivityResult (requestCode, resultCode, data);
		}

		protected override void OnPause() {
//			nfcAdapter.DisableForegroundDispatch(this);
			base.OnPause ();
		}

		// Attempt to format NFC tag if the tag is not formatted
		private void formatTag(Tag tag, NdefMessage ndefMessage) {
			try {
				NdefFormatable ndefFormatable = NdefFormatable.Get(tag);

				if (ndefFormatable == null){
					Toast.MakeText(this, "Tag is not ndef formatable", ToastLength.Short).Show();
					return;
				}
				ndefFormatable.Connect();
				ndefFormatable.Format(ndefMessage);
				ndefFormatable.Close();
				Toast.MakeText(this, "Tag written.", ToastLength.Short).Show();
			} catch (Exception ex) {
				var error = ex.Message;
				Console.WriteLine (error);
			}
		}

		//	Write message to NFC Tag
		private void writeNdefMessage(Tag tag, NdefMessage ndefmessage) {
			try {
				if (tag == null) {
					Toast.MakeText(this, "Tag object cannot be null", ToastLength.Short).Show();
					return;
				}

				Ndef ndef = Ndef.Get(tag);

				if (ndef == null) {
					// Format tag with the ndef format and writes the message
					formatTag(tag, ndefmessage);
				} else {
					ndef.Connect();

					if (!ndef.IsWritable) {
						Toast.MakeText(this, "Tag is not writeable", ToastLength.Short).Show();
						ndef.Close();
						return;
					}

					// NFC tags can only store a small amount of data, this depends on the type of tag its.
					var size = ndefmessage.ToByteArray().Length;
					if (ndef.MaxSize < size)
					{
						Toast.MakeText(this, "Tag doesn't have enough space.", ToastLength.Short).Show();
					}

					ndef.WriteNdefMessage(ndefmessage);
					ndef.Close();
					Toast.MakeText(this, "Tag written.", ToastLength.Short).Show();
				}

			} catch (Exception ex) {
				Console.WriteLine ("writeNdefMessage", ex.Message);
			}
		}

		public NdefMessage CreateNdefMessage (string text)
		{
			NdefMessage msg = new NdefMessage (
				new NdefRecord[]{ CreateMimeRecord (
					"application/com.group11.solarblinds",
					Encoding.UTF8.GetBytes (text)) });
			return msg;
		}

		public NdefRecord CreateMimeRecord (String mimeType, byte [] payload)
		{
			byte [] mimeBytes = Encoding.UTF8.GetBytes (mimeType);
			NdefRecord mimeRecord = new NdefRecord (
				NdefRecord.TnfMimeMedia, mimeBytes, new byte [0], payload);
			return mimeRecord;
		}

		// Populate the list view fromt eh database
		protected void getCursorView() {
			Console.WriteLine("Get Cursor");
			Android.Database.ICursor icursor_temp = dbr.getRecordCursor ();
			if (icursor_temp != null) {
				icursor_temp.MoveToFirst ();
				Console.WriteLine("Moved first");
				sync_list_view = FindViewById<ListView> (Resource.Id.sync_list_view);
				Console.WriteLine("Set sync_list_view");
				string[] from = new string[] { "_id", "Nickname", "Device_id" };
				int[] to = new int[] {
					Resource.Id.sync_id,
					Resource.Id.sync_nickname,
					Resource.Id.sync_device_id
				};
				// Create a simple CursorAdapter to fill the listview object
				SimpleCursorAdapter cursor_adapter_temp = new SimpleCursorAdapter(this, Resource.Layout.SyncListView, icursor_temp, from, to);
				sync_list_view.Adapter = cursor_adapter_temp;
			} else {
				Console.WriteLine("Something failed...");
			}
		}


		// Attempts to handle BluetoothGattCallBack
		public void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status,
			ProfileState newState)
		{
			String intentAction; 
			switch (newState)
			{
			case ProfileState.Connected:
//				intentAction = BluetoothLeService.ACTION_GATT_CONNECTED;
//				BluetoothLeService.mConnectionState = State.Connected;
				Console.WriteLine("Connected peripheral: " + gatt.Device.Name);
				break;
			case ProfileState.Disconnected:
//				intentAction = BluetoothLeService.ACTION_GATT_DISCONNECTED;
//				BluetoothLeService.mConnectionState = State.Disconnected;
				Console.WriteLine("Disconnected peripheral: " + gatt.Device.Name);
				break;
			case ProfileState.Connecting:
				Console.WriteLine("Connecting peripheral: " + gatt.Device.Name);
				break;
			case ProfileState.Disconnecting:
				Console.WriteLine("Disconnecting peripheral: " + gatt.Device.Name);
				break;
			}
		}

		public void OnServicesDiscovered (BluetoothGatt gatt, GattStatus status)
		{
			if (status == GattStatus.Success) {
				Console.WriteLine("GATT Services discovered: " + gatt.Device.Name);
				//				service.BroadcastUpdate (BluetoothLeService.ACTION_GATT_SERVICES_DISCOVERED);
			} else {
				Console.WriteLine("GATT Services discovered: Fail" + gatt.Device.Name);
				//				Log.Warn (BluetoothLeService.TAG, "onServicesDiscovered received: " + status);
			}
		}

		public void OnCharacteristicRead (BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
		{
			if (status == GattStatus.Success) {
				Console.WriteLine("GATT Characteristic read" + gatt.Device.Name);
				//				service.BroadcastUpdate (BluetoothLeService.ACTION_DATA_AVAILABLE, characteristic);
			}
		}

		public void OnCharacteristicChanged (BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
		{
			Console.WriteLine("GATT Characteristic changed" + gatt.Device.Name);
			//			service.BroadcastUpdate (BluetoothLeService.ACTION_DATA_AVAILABLE, characteristic);
		}
	}
}