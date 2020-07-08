using Android.Content;
using AndroidX.Room;
using Java.Lang;

namespace RoomExample
{
    [Database(Version = 1, Entities = new Class[] { Class.FromType(typeof(Note)) }, ExportSchema = false)]
    public abstract class NoteDatabase : RoomDatabase
    {
        private static readonly object LOCK = new object();
        public static readonly string DATABASE_NAME = "notes";
        private static NoteDatabase sInstance;

        public static NoteDatabase GetInstance(Context context)
        {
            if (sInstance == null)
            {
                lock (LOCK)
                {
                    sInstance = (NoteDatabase)Room.DatabaseBuilder(context.ApplicationContext,
                            Class.FromType(typeof(NoteDatabase)),
                            DATABASE_NAME)
                        .Build();
                }
            }
            return sInstance;
        }

        public abstract INoteDao NoteDao();
    }
}