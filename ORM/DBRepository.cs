using System;
using System.Data;
using System.IO;
//using SQLite;
using Android.Database.Sqlite;

namespace TheSolarBlinds
{	
	public class DBRepository
	{

		private SQLiteDatabase sqld_temp;
		private string sqld_query;
		private string sqld_message;
		private bool db_isavailable;

		public DBRepository (string sqld_name)
		{
			sqld_message = "";
			db_isavailable = false;
			createDB (sqld_name);
		}

		public string message {
			get{ return sqld_message;}
			set{ sqld_message = value;}
		}

		// Code to create the database
		public void createDB(string sqldb_name) {
			try {
				sqld_message = "";
				string db_path = Path.Combine (Environment.GetFolderPath 
					(Environment.SpecialFolder.Personal), sqldb_name);
				bool db_exists = File.Exists(db_path);
				if (!db_exists) {
					sqld_temp = SQLiteDatabase.OpenOrCreateDatabase(db_path, null);
					sqld_query = "Create Table IF Not EXISTS " + 
						"IdNickname " +
						"(_id INTEGER PRIMARY KEY AUTOINCREMENT, Nickname VARCHAR, Device_id VARCHAR)";
					sqld_temp.ExecSQL (sqld_query);
					sqld_message = "New database is created.";
					Console.WriteLine("New database has been created");
				}else {
					sqld_temp = SQLiteDatabase.OpenDatabase(db_path, null, DatabaseOpenFlags.OpenReadwrite);
					sqld_message = "Database is opened";
					Console.WriteLine("Database has been opened");
				}
				db_isavailable = true;
				Console.WriteLine("Database has been created or opened");
			} catch (Exception ex) {
				sqld_message = ex.Message;
			}
			//			var output = "";
			//			output += "Creating Database if it doesn't exist.";
			//			string dbPath = Path.Combine (Environment.GetFolderPath 
			//				(Environment.SpecialFolder.Personal), "solarblinds_sync.db3");
			//			var db = new SQLiteConnection(dbPath);
			//			output += "\nDatabase Created...";
			//			return output;

		}

		//		public string createTable() {
		//			try {
		//				string db_path = Path.Combine (Environment.GetFolderPath 
		//					(Environment.SpecialFolder.Personal), "solarblinds_sync.db3");
		//				var db = new SQLiteConnection(db_path);
		//				db.CreateTable<IdNicknames>();
		//				string result = "Table created successfully...";
		//				return result;
		//			} catch (Exception ex) {
		//				return "Error : " + ex.Message;
		//			}
		//		}

		//		public string getAllRecords () {
		//			try {
		//				string db_path = Path.Combine (Environment.GetFolderPath 
		//					(Environment.SpecialFolder.Personal), "solarblinds_sync.db3");
		//				var db = new SQLiteConnection(db_path);
		//
		//				string output = "";
		//				output += "Retrieving the data using ORM...";
		//				var table = db.Table<IdNicknames>();
		//				foreach( var item in table) {
		//					output += "\n" + item.Id + "---" + item.nickname;
		//				}
		//
		//				return output;
		//
		//			} catch (Exception ex) {
		//				return "Error : " + ex.Message;
		//			}
		//		}

		//		// Code to retrieve specific record using ORM
		//		public string getNicknameById(int id) {
		//			try {
		//				string db_path = Path.Combine (Environment.GetFolderPath 
		//					(Environment.SpecialFolder.Personal), "solarblinds_sync.db3");
		//				var db = new SQLiteConnection(db_path);
		//
		//				var item = db.Get<IdNicknames>(id);
		//				return item.nickname;
		//
		//			} catch (Exception ex) {
		//				return "Error : " + ex.Message;
		//			}
		//		}

		public void addRecord(string nickname, string device_id) {
			try {
				sqld_query = "INSERT INTO " + 
					"IdNickname " +
					"(Nickname,Device_id)" +
					"VALUES('" + nickname + "','" + device_id + "');";
				sqld_temp.ExecSQL(sqld_query);
				sqld_message = "Record is saved.";
				Console.WriteLine("Record has been added!");
			} catch (SQLiteException ex) {
				sqld_message = ex.Message;
			}
		}

		// Code to remove the record using ORM
		public void removeRecord(int id) {
			try {
				sqld_query = "DELETE FROM IdNickname " +
					"WHERE _id='" + id + "';";
				sqld_temp.ExecSQL(sqld_query);
				sqld_message = "Record is deleted: " + id;
			} catch (Exception ex) {
				sqld_message = ex.Message;
			}
		}

		// Code to update the record using ORM
		public void updateRecord(int id, string nickname) {
			try {
				sqld_query = "UPDATE IdNickname " +
					"SET Nickname='" + nickname + "' " + 
					"WHERE _id='" + id + "';";
				sqld_temp.ExecSQL(sqld_query);
				sqld_message = "Record is updated: " + id;
			} catch (Exception ex) {
				sqld_message = ex.Message;
			}
		}

		public Android.Database.ICursor getRecordCursor() {
			Android.Database.ICursor icursor_temp = null;
			try {
				sqld_query = "SELECT * FROM IdNickname;";
				icursor_temp = sqld_temp.RawQuery(sqld_query, null);
				if (!(icursor_temp != null)) {
					sqld_message = "Record not found.";
				}
			} catch (Exception ex) {
				sqld_message = ex.Message;
			}
			return icursor_temp;
		}
	}
}