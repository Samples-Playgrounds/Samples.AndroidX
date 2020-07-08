using System.Collections.Generic;

using AndroidX.Room;

namespace RoomExample
{
    [Dao]
    public interface INoteDao
    {
        [Insert(OnConflict = OnConflictStrategy.Replace)]
        void Save(Note note);

        [Query(Value = "SELECT * FROM notes WHERE id = :noteId LIMIT 1")]
        Note Load(int noteId);

        [Query(Value = "SELECT * FROM notes ORDER BY id DESC")]
        List<Note> Load();

        [Update(OnConflict = OnConflictStrategy.Replace)]
        void Update(Note note);

        [Delete]
        void Delete(Note note);
    }
}