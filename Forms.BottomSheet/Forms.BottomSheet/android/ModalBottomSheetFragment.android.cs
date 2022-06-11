using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Google.Android.Material.BottomSheet;
using Xamarin.Forms.Platform.Android;

namespace Forms.BottomSheet
{
	public class ModalBottomSheetFragment : BottomSheetDialogFragment
    {
        #region Members
        private Android.Widget.TextView? title;
        private ContainerView? container;
        #endregion

        #region Properties
        public BottomSheetBehavior? Behavior { get; private set; }
        public BottomSheet BottomSheet { get; private set; }
        public static readonly string TAG = "Forms.BottomSheet.ModalBottomSheetFragment";
        #endregion

        #region Constructors
        public ModalBottomSheetFragment(BottomSheet bottomSheet)
        {
            BottomSheet = bottomSheet;
        }
        #endregion

        #region Bottom Sheet Dialog Fragment
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return CreateBottomSheetView();
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            BottomSheet.IsOpen = false;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            BottomSheetDialog bottomSheetDialog = (BottomSheetDialog)base.OnCreateDialog(savedInstanceState);

            Behavior = bottomSheetDialog.Behavior;

            DefaultBottomSheetBehavior();
            SetBottomSheetState();

            return bottomSheetDialog;
        }
        #endregion

        #region Configuration
        private void DefaultBottomSheetBehavior()
        {
            _ = Behavior ?? throw new ArgumentException($"{nameof(Behavior)} can not be null.");

            Behavior.PeekHeight = 0;
            Behavior.Draggable = BottomSheet.BottomSheetState != BottomSheetState.Medium;
            Behavior.SkipCollapsed = true;
            Behavior.FitToContents = false;

            Behavior.AddBottomSheetCallback(new BottomSheetBehaviorCallback(BottomSheet));
        }

        internal void UpdateBottomSheet(string property)
        {
            if (property == nameof(BottomSheet.Title)
                && title != null
                && Dialog.FindViewById(title.Id) is Android.Widget.TextView titleView)
            {
                titleView.Text = BottomSheet.Title;
            }

            else if (property == nameof(BottomSheet.BottomSheetState)
                && BottomSheet.IsOpen == true)
            {
                SetBottomSheetState();
            }

            else if (property == nameof(BottomSheet.Cancelable))
            {
                Cancelable = BottomSheet.Cancelable;
            }
        }

        private void CreateTitle()
        {
            title = new Android.Widget.TextView(Context)
            {
                Text = BottomSheet.Title,
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
                Gravity = GravityFlags.Center
            };
            title.EnsureId();
        }

        private void SetBottomSheetState()
        {
            _ = Behavior ?? throw new ArgumentException($"{nameof(Behavior)} can not be null.");

            Behavior.State = BottomSheet.BottomSheetState switch
            {
                BottomSheetState.Unknown => BottomSheetBehavior.StateHalfExpanded,
                BottomSheetState.Large => BottomSheetBehavior.StateExpanded,
                BottomSheetState.Medium => BottomSheetBehavior.StateHalfExpanded,
                BottomSheetState.MediumAndLarge => BottomSheetBehavior.StateHalfExpanded,
                _ => BottomSheetBehavior.StateCollapsed
            };
        }
        #endregion

        public View CreateBottomSheetView()
        {
            _ = BottomSheet.BottomSheetContentDataTemplate ?? throw new ArgumentException($"{nameof(BottomSheet.BottomSheetContentDataTemplate)} can not be null.");

            if (container == null)
            {
                var bottomSheetContent = (Xamarin.Forms.View)BottomSheet.BottomSheetContentDataTemplate.CreateContent();
                bottomSheetContent.BindingContext = BottomSheet?.BindingContext;

                container = new ContainerView(Context, bottomSheetContent);
            }

            var linearLayout = new Android.Widget.LinearLayout(Context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
                Orientation = Android.Widget.Orientation.Vertical
            };

            if (string.IsNullOrWhiteSpace(BottomSheet?.Title) == false)
            {
                CreateTitle();
                linearLayout.AddView(title);
            }

            linearLayout.AddView(container);

            return linearLayout;
        }
    }
}

