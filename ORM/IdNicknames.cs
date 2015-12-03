using System;
using System.Data;
using System.IO;
using SQLite;

namespace TheSolarBlinds
{
	public class IdNicknames
	{
		public IdNicknames ()
		{
		}

		[PrimaryKey, AutoIncrement,Column("_Id")]
		public int Id { get; set;}

		[MaxLength(50)]
		public string nickname { get; set;}

		[MaxLength(50)]
		public string device_id { get; set;}
	}
}