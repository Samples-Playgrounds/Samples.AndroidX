using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Immutable;
using AndroidX.RecyclerView.Widget;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Adapters;
using Toggl.Droid.ViewHolders;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class SelectDefaultWorkspaceFragment : ReactiveDialogFragment<SelectDefaultWorkspaceViewModel>
    {
        public SelectDefaultWorkspaceFragment() { }

        public SelectDefaultWorkspaceFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SelectDefaultWorkspaceFragment, null);

            InitializeViews(view);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var adapter = new SimpleAdapter<SelectableWorkspaceViewModel>(
                Resource.Layout.SelectDefaultWorkspaceFragmentCell,
                SelectDefaultWorkspaceViewHolder.Create
            );
            adapter.Items = ViewModel.Workspaces ?? ImmutableList<SelectableWorkspaceViewModel>.Empty;
            adapter.ItemTapObservable
                .Subscribe(ViewModel.SelectWorkspace.Inputs)
                .DisposedBy(DisposeBag);

            recyclerView.SetAdapter(adapter);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DisposeBag.Dispose();
        }
    }
}
