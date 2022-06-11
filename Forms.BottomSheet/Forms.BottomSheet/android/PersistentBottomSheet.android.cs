using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.BottomSheet;
using Xamarin.Forms.Platform.Android;

namespace Forms.BottomSheet
{
	public class PersistentBottomSheet : IBottomSheet
	{
        #region Members
        private BottomSheetBehavior? bottomSheetBehavior;
        private LinearLayout? bottomSheetLayout;
        private Xamarin.Forms.View? bottomSheetContent;
        private readonly BottomSheet bottomSheet;
        private readonly Context context;
        private readonly CoordinatorLayout parent;
        #endregion

        #region Properties
        public BottomSheetBehavior Behavior { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion

        #region Constructors
        public PersistentBottomSheet(BottomSheet bottomSheet, Context context, CoordinatorLayout parent)
		{
            this.bottomSheet = bottomSheet;
            this.context = context;
            this.parent = parent;
		}
        #endregion

        #region IBottomSheet
        public void DismissBottomSheet()
        {
            _ = bottomSheetBehavior ?? throw new ArgumentException($"{nameof(bottomSheetBehavior)} can not be null.");

            bottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
        }

        public void ShowBottomSheet()
        {
            if (bottomSheetLayout == null)
            {
                CreateBottomSheet();
            }

            SetBottomSheetState();
        }

        public void UpdateBottomSheet(string property)
        {
            if (property == nameof(BottomSheet.Title))
            {
                CreateTitle();
            }

            else if (property == nameof(BottomSheet.BottomSheetState)
                && bottomSheet.IsOpen == true
                && bottomSheetBehavior != null)
            {
                SetBottomSheetState();
            }
        }
        #endregion

        #region Methods
        public void CreateBottomSheet()
        {
            CreateBottomSheetLayout();
        }

        private void CreateBottomSheetLayout()
        {
            bottomSheetLayout = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
                {
                    Behavior = DefaultBottomSheetBehavior()
                },
            };

            if (bottomSheet.BottomSheetContentDataTemplate?.CreateContent() is Xamarin.Forms.View content)
            {
                bottomSheetContent = content;
                bottomSheetContent.BindingContext = bottomSheet.BindingContext;

                if (string.IsNullOrWhiteSpace(bottomSheet.Title) == false)
                {
                    CreateTitle();
                }

                SetBottomSheetBackground();

                bottomSheetLayout.Elevation = 16;
                bottomSheetLayout.AddView(new ContainerView(context, bottomSheetContent));
                parent.AddView(bottomSheetLayout);
            }
        }

        private BottomSheetBehavior DefaultBottomSheetBehavior()
        {
            bottomSheetBehavior = new BottomSheetBehavior()
            {
                PeekHeight = 0,
                Draggable = bottomSheet.BottomSheetState != BottomSheetState.Medium,
                SkipCollapsed = true,
                FitToContents = false
            };

            bottomSheetBehavior.AddBottomSheetCallback(new BottomSheetBehaviorCallback(bottomSheet));

            return bottomSheetBehavior;
        }

        private void CreateTitle()
        {
            var title = new TextView(context)
            {
                Text = bottomSheet.Title,
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
                Gravity = GravityFlags.Center
            };

            bottomSheetLayout?.AddView(title, 0);
        }

        private void SetBottomSheetBackground()
        {
            _ = bottomSheetContent ?? throw new ArgumentException($"{nameof(bottomSheetContent)} can not be null.");

            GradientDrawable shape = new GradientDrawable();

            var backgroundColor = bottomSheetContent.BackgroundColor.ToAndroid();
            if (backgroundColor.A == 0)
            {
                backgroundColor.A = 255;
            }

            shape.SetColor(backgroundColor);
            bottomSheetLayout?.SetBackground(shape);
        }

        public void SetBottomSheetState()
        {
            _ = bottomSheetBehavior ?? throw new ArgumentException($"{nameof(bottomSheetBehavior)} can not be null.");

            bottomSheetBehavior.State = bottomSheet.BottomSheetState switch
            {
                BottomSheetState.Unknown => BottomSheetBehavior.StateHalfExpanded,
                BottomSheetState.Large => BottomSheetBehavior.StateExpanded,
                BottomSheetState.Medium => BottomSheetBehavior.StateHalfExpanded,
                BottomSheetState.MediumAndLarge => BottomSheetBehavior.StateHalfExpanded,
                _ => BottomSheetBehavior.StateCollapsed
            };
        }
        #endregion
    }
}

