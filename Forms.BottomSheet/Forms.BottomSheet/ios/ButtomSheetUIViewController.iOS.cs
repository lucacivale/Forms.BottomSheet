using UIKit;

namespace Forms.BottomSheet
{
	public class ButtomSheetUIViewController : UINavigationController
	{
		private readonly BottomSheet? bottomSheet;
		public bool IsDismissed { get; private set; }
		
		public ButtomSheetUIViewController(BottomSheet? bottomSheet, UIViewController viewController) : base(viewController)
		{
			this.bottomSheet = bottomSheet;
		}

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            IsDismissed = false;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            IsDismissed = true;
            if (bottomSheet != null)
            {
                bottomSheet.IsOpen = false;
            }
        }
    }
}

