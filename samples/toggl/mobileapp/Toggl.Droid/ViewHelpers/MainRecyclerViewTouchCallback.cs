using Android.Graphics;
using Android.Runtime;
using System;
using AndroidX.RecyclerView.Widget;
using Toggl.Droid.Adapters;
using Toggl.Droid.ViewHolders;

namespace Toggl.Droid.ViewHelpers
{
    public class MainRecyclerViewTouchCallback : ItemTouchHelper.SimpleCallback
    {
        private MainRecyclerAdapter adapter;
        public bool AreSwipeActionsEnabled { get; set; }

        public MainRecyclerViewTouchCallback(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MainRecyclerViewTouchCallback(MainRecyclerAdapter adapter) : base(0, ItemTouchHelper.Left | ItemTouchHelper.Right)
        {
            this.adapter = adapter;
        }
        public override bool IsItemViewSwipeEnabled => AreSwipeActionsEnabled;

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
            => false;

        public override int GetSwipeDirs(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            if (viewHolder is MainLogCellViewHolder mainLogCellViewHolder && mainLogCellViewHolder.CanSync)
            {
                return !mainLogCellViewHolder.Item.ViewModel.CanContinue ? ItemTouchHelper.Left : base.GetSwipeDirs(recyclerView, viewHolder);
            }

            return ItemTouchHelper.ActionStateIdle;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            var swipedPosition = viewHolder.AdapterPosition;

            if (direction == ItemTouchHelper.Right)
            {
                adapter.ContinueTimeEntryBySwiping(swipedPosition);
            }
            else
            {
                adapter.DeleteTimeEntry(swipedPosition);
            }
        }

        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            if (viewHolder is MainLogCellViewHolder logViewHolder)
            {
                if (dX > 0)
                {
                    logViewHolder.ShowSwipeToContinueBackground();
                }
                else if (dX < 0)
                {
                    logViewHolder.ShowSwipeToDeleteBackground();
                }
                else
                {
                    logViewHolder.HideSwipeBackgrounds();
                }

                DefaultUIUtil.OnDraw(c, recyclerView, logViewHolder.MainLogContentView, dX, dY, actionState, isCurrentlyActive);
            }
            else
            {
                base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
            }
        }

        public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            if (viewHolder is MainLogCellViewHolder logViewHolder)
            {
                DefaultUIUtil.OnSelected(logViewHolder.MainLogContentView);
            }
            else
            {
                base.OnSelectedChanged(viewHolder, actionState);
            }
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            if (viewHolder is MainLogCellViewHolder logViewHolder)
            {
                DefaultUIUtil.ClearView(logViewHolder.MainLogContentView);
            }
            else
            {
                base.ClearView(recyclerView, viewHolder);
            }
        }

        public override float GetSwipeVelocityThreshold(float defaultValue)
        {
            return defaultValue * 0.1f;
        }

        public override float GetSwipeEscapeVelocity(float defaultValue)
        {
            return defaultValue * 2.0f;
        }

        public override float GetSwipeThreshold(RecyclerView.ViewHolder viewHolder)
        {
            return 0.75f;
        }
    }
}
