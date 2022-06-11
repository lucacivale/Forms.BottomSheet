using Android.Views;
using Google.Android.Material.BottomSheet;
using Xamarin.Forms.Platform.Android;

namespace Forms.BottomSheet
{
	public class BottomSheetBehaviorCallback : BottomSheetBehavior.BottomSheetCallback
    {
        private readonly BottomSheet bottomSheet;

        public BottomSheetBehaviorCallback(BottomSheet bottomSheet)
		{
            this.bottomSheet = bottomSheet;
		}

        public override void OnSlide(View bottomSheet, float newState)
        {
        }

        public override void OnStateChanged(View sheet, int state)
        {
            switch (state)
            { 
                case BottomSheetBehavior.StateHalfExpanded:
                    if (bottomSheet.BottomSheetState == BottomSheetState.Large)
                    {
                        BottomSheetBehavior.From(sheet).State = BottomSheetBehavior.StateExpanded;
                    }
                    break;
                case BottomSheetBehavior.StateExpanded:
                    if (bottomSheet.BottomSheetState == BottomSheetState.Medium)
                    {
                        BottomSheetBehavior.From(sheet).State = BottomSheetBehavior.StateHalfExpanded;
                    }
                    break;
                case BottomSheetBehavior.StateCollapsed:
                case BottomSheetBehavior.StateHidden:
                    if (bottomSheet.Cancelable == false)
                    {
                        ((BottomSheetRenderer)Platform.GetRenderer(bottomSheet))?.BottomSheetController?.ShowBottomSheet();
                    }
                    else
                    {
                        bottomSheet.IsOpen = false;
                    }
                    break;
            }
        }
    }
}

