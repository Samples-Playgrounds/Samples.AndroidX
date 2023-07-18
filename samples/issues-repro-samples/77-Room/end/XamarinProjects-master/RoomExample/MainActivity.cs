using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;

namespace RoomExample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity,
        IItemClickListener
    {
        private EditText text;
        private Button button;
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager layoutManager;
        private NoteAdapter adapter;
        private readonly List<Note> notes = new List<Note>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            text = FindViewById<EditText>(Resource.Id.text);
            button = FindViewById<Button>(Resource.Id.button);
            button.Click +=(sender, e) => Click_Listener();
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerview);

            layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);
            adapter = new NoteAdapter(this, this);
            recyclerView.SetAdapter(adapter);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Click_Listener()
        {
            if(!string.IsNullOrWhiteSpace(text.Text))
            {
                notes.Add(new Note {
                    Text = text.Text,
                    Timestamp = DateTime.Now.ToString()
                });
                // Empty entry
                text.Text = string.Empty;
                // Update RecyclerView
                adapter.ReplaceNotes(notes);
            }
        }

        public void OnItemClicked(View view, int position)
        {
            Note note = adapter.GetNote(position);

            Intent intent = new Intent(this, typeof(NoteDetailsActivity));
            intent.PutExtra("noteId", note.Id);
            StartActivity(intent);
        }
    }
}