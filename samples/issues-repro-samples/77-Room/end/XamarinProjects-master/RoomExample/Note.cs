using AndroidX.Room;

namespace RoomExample
{
    [Entity(TableName ="notes")]
    public class Note
    {
        [PrimaryKey(AutoGenerate = true)]
        [ColumnInfo(Name = "id")]
        public long Id { get; set; }
        [ColumnInfo(Name = "text")]
        public string Text { get; set; }
        [ColumnInfo(Name = "timestamp")]
        public string Timestamp { get; set; }
    }
}