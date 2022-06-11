using Android.Content;
using Xamarin.Forms.Platform.Android;

namespace Forms.BottomSheet
{
	public class ModalBottomSheet : IBottomSheet
	{
        #region Members
        private readonly AndroidX.Fragment.App.FragmentManager? sheetFragmentManager;
        private ModalBottomSheetFragment? modalBottomSheetDialogFragment;
        #endregion

        #region Properties
        public BottomSheet BottomSheet { get; private set; }
        #endregion

        #region Constructors
        public ModalBottomSheet(BottomSheet bottomSheet, Context context) : base()
        {
            BottomSheet = bottomSheet;
            sheetFragmentManager = context.GetFragmentManager();
        }
        #endregion

        #region IBottomSheet
        public void UpdateBottomSheet(string property)
        {
            modalBottomSheetDialogFragment?.UpdateBottomSheet(property);
        }

        public void ShowBottomSheet()
        {
            modalBottomSheetDialogFragment = new ModalBottomSheetFragment(BottomSheet)
            {
                Cancelable = BottomSheet.Cancelable
            };
            modalBottomSheetDialogFragment.Show(sheetFragmentManager, ModalBottomSheetFragment.TAG);
        }

        public void DismissBottomSheet()
        {
            modalBottomSheetDialogFragment?.Dismiss();
        }
        #endregion
    }
}

