using System;
using System.ComponentModel;
using Android.Content;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(Forms.BottomSheet.BottomSheet), typeof(Forms.BottomSheet.BottomSheetRenderer))]
namespace Forms.BottomSheet
{
    public class BottomSheetRenderer : CoordinatorLayout, IVisualElementRenderer
    {
        #region Members
        private int? defaultLabelFor;
        private VisualElementTracker? tracker;
        private bool disposed;
        #endregion

        #region Properties
        public BottomSheet? Element { get; private set; }
        public IBottomSheet? BottomSheetController { get; private set; }
        Xamarin.Forms.VisualElement? IVisualElementRenderer.Element => Element;
        VisualElementTracker? IVisualElementRenderer.Tracker => tracker;
        ViewGroup? IVisualElementRenderer.ViewGroup => null;
        View? IVisualElementRenderer.View => this;
        #endregion

        #region Constructor
        public BottomSheetRenderer(Context context) : base(context)
        {
        }
        #endregion

        #region Events
        public event EventHandler<VisualElementChangedEventArgs>? ElementChanged;
        public event EventHandler<PropertyChangedEventArgs>? ElementPropertyChanged;

        void OnElementChanged(ElementChangedEventArgs<BottomSheet?>? e)
        {
            if (e?.OldElement != null)
            {
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;
            }
            if (e?.NewElement != null)
            {
                e.NewElement.PropertyChanged += OnElementPropertyChanged;
                ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
                this.EnsureId();
            }
        }

        void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BottomSheet.IsOpen))
            {
                if (Element?.IsOpen == true)
                {

                    BottomSheetController?.ShowBottomSheet();
                }
                else 
                {
                    BottomSheetController?.DismissBottomSheet();
                }
            }
            else if (e.PropertyName == nameof(BottomSheet.IsModal))
            {
                bool resetSheet = Element?.IsOpen == true;

                if (resetSheet)
                {
                    BottomSheetController?.DismissBottomSheet();
                }

                CreateButtomSheetDialog();

                if (resetSheet)
                {
                    BottomSheetController?.ShowBottomSheet();
                }
            }
            else
            {
                BottomSheetController?.UpdateBottomSheet(e.PropertyName);
            }

            ElementPropertyChanged?.Invoke(this, e);
        }
        #endregion

        #region Methods
        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            if (disposing)
            {
                if (tracker != null)
                {
                    tracker.Dispose();
                    tracker = null;
                }

                if (Element != null)
                {
                    Element.PropertyChanged -= OnElementPropertyChanged;

                    if (Platform.GetRenderer(Element) == this)
                    {
                        Platform.SetRenderer(Element, null);
                    }
                }
            }

            base.Dispose(disposing);
        }

        private void CreateButtomSheetDialog()
        {
            _ = Context ?? throw new ArgumentException($"{nameof(Context)} can not be null.");

            BottomSheetController = Element?.IsModal switch
            {
                false => new PersistentBottomSheet(Element, Context, this),
                true => new ModalBottomSheet(Element, Context),
                _ => throw new NotImplementedException(),
            };
        }

        private void CreateBottomSheet()
        {
            LayoutParameters = new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            AddView(new ContainerView(Context, Element?.Content));

            CreateButtomSheetDialog();
            if (Element?.IsOpen == true)
            {
                BottomSheetController?.ShowBottomSheet();
            }
        }
        #endregion

        #region IVisualElementRenderer
        Xamarin.Forms.SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            Measure(widthConstraint, heightConstraint);

            return new Xamarin.Forms.SizeRequest(new Xamarin.Forms.Size(MeasuredWidth, MeasuredHeight), default);
        }

        void IVisualElementRenderer.UpdateLayout() => tracker?.UpdateLayout();

        void IVisualElementRenderer.SetElement(Xamarin.Forms.VisualElement element)
        {
            if (element is not BottomSheet bottomSheet)
            {
                throw new ArgumentException($"{nameof(element)} must be of type {nameof(BottomSheet)}");
            }

            var oldElement = Element;
            Element = bottomSheet;

            CreateBottomSheet();

            if (tracker == null)
            {
                tracker = new VisualElementTracker(this);
            }

            OnElementChanged(new ElementChangedEventArgs<BottomSheet?>(oldElement, Element));
        }
        
        void IVisualElementRenderer.SetLabelFor(int? id)
        {
            if (defaultLabelFor == null)
            {
                defaultLabelFor = LabelFor;
            }
            LabelFor = (int)(id ?? defaultLabelFor);
        }
        #endregion
    }
}
