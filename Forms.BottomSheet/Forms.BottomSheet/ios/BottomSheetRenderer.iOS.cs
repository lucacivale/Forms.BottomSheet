using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Forms.BottomSheet.BottomSheet), typeof(Forms.BottomSheet.BottomSheetRenderer))]
namespace Forms.BottomSheet
{
    public class BottomSheetRenderer : UIViewController, IVisualElementRenderer
    {
        #region Members
        bool isDisposed;
        public IVisualElementRenderer? Control { get; private set; }
        public BottomSheet? Element { get; private set; }
        VisualElement? IVisualElementRenderer.Element => Element;
        public UIView? NativeView => View;
        public UIViewController? ViewController { get; private set; }
        public event EventHandler<VisualElementChangedEventArgs>? ElementChanged;
        public event EventHandler<PropertyChangedEventArgs>? ElementPropertyChanged;
        private ContentPage? bottomSheetContentPage;
        private UIViewController? bottomSheetViewController;
        private ButtomSheetUIViewController? bottomSheetNavigationController;
        private readonly UIViewController? rootController;
        #endregion

        #region Constructor
        public BottomSheetRenderer()
        {
            rootController = UIApplication.SharedApplication.Delegate?.GetWindow()?.RootViewController ?? throw new InvalidOperationException($"{nameof(View)} cannot be null");
        }
        #endregion


        #region IVisualElementRenderer
        public void SetElementSize(Size size)
            => Control?.SetElementSize(size);

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            UIView rootView = rootController?.View ?? throw new InvalidOperationException($"{nameof(View)} cannot be null");

            SetElementSize(new Size(rootView.Bounds.Width, rootView.Bounds.Height));
        }

        public async override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (Element?.IsOpen == true)
            {
                CreateBottomSheet();
                _ = bottomSheetNavigationController ?? throw new ArgumentException($"{nameof(bottomSheetNavigationController)} can not be null.");
                _ = ViewController ?? throw new ArgumentException($"{nameof(ViewController)} can not be null.");

                await ViewController.PresentViewControllerAsync(bottomSheetNavigationController, true);
            }
        }

        public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint) =>
            NativeView.GetSizeRequest(widthConstraint, heightConstraint);

        void IVisualElementRenderer.SetElement(VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!(element is BottomSheet))
                throw new ArgumentNullException("Element is not of type " + typeof(BottomSheet), nameof(element));

            var oldElement = Element;
            Element = element as BottomSheet;

            CreateControl();

            if (oldElement != null)
                oldElement.PropertyChanged -= OnElementPropertyChanged;

            element.PropertyChanged += OnElementPropertyChanged;

            OnElementChanged(new ElementChangedEventArgs<BottomSheet?>(oldElement, Element));
        }
        #endregion

        #region Events
        protected virtual void OnElementChanged(ElementChangedEventArgs<BottomSheet?> e)
        {
            if (e.NewElement != null
                && !isDisposed)
            {
                SetView();
            }

            ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
        }

        protected async virtual void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BottomSheet.IsOpen))
            {
                if (Element?.IsOpen == true)
                {
                    if (bottomSheetContentPage == null)
                    {
                        CreateBottomSheet();
                    }
                    else
                    {
                        ConfigureSheetPresentationController();
                    }

                    _ = bottomSheetNavigationController ?? throw new ArgumentException($"{nameof(bottomSheetNavigationController)} can not be null.");
                    _ = ViewController ?? throw new ArgumentException($"{nameof(ViewController)} can not be null.");

                    await ViewController.PresentViewControllerAsync(bottomSheetNavigationController, true);
                }
                else if (bottomSheetNavigationController?.IsDismissed == false)
                {
                    await bottomSheetNavigationController.DismissViewControllerAsync(true);
                }
            }

            else if (e.PropertyName == nameof(BottomSheet.Title)
                && bottomSheetNavigationController != null)
            {
                bottomSheetNavigationController.NavigationBar.TopItem.Title = Element?.Title;
            }

            else if (e.PropertyName == nameof(BottomSheet.BottomSheetState)
                || e.PropertyName == nameof(BottomSheet.IsModal))
            {
                ConfigureSheetPresentationController();
            }

            ElementPropertyChanged?.Invoke(this, e);
        }
        #endregion
        #region Methods
        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            isDisposed = true;
            if (disposing)
            {
                if (Element != null)
                {
                    Element.PropertyChanged -= OnElementPropertyChanged;
                    Element = null;
                }
                bottomSheetViewController?.Dispose();
                bottomSheetNavigationController?.Dispose();
            }

            base.Dispose(disposing);
        }


        void CreateControl()
        {
            _ = Element ?? throw new InvalidOperationException($"{nameof(Element)} cannot be null");

            var view = Element.Content;
            var contentPage = new ContentPage { Content = view };
            
            Control = Platform.CreateRenderer(contentPage);
            ViewController = Control.ViewController;
            Platform.SetRenderer(contentPage, Control);
            contentPage.Parent = Xamarin.Forms.Application.Current.MainPage;
            contentPage.SetBinding(BindableObject.BindingContextProperty, new Binding { Source = Element, Path = BindableObject.BindingContextProperty.PropertyName });
        }

        void SetView()
        {
            _ = View ?? throw new InvalidOperationException($"{nameof(View)} cannot be null");
            _ = Control ?? throw new InvalidOperationException($"{nameof(Control)} cannot be null");

            View.AddSubview(Control.ViewController.View ?? throw new NullReferenceException());
            AddChildViewController(Control.ViewController);
        }

        private ContentPage CreateBottomSheetContentPage()
        {
            return new ContentPage()
            {
                Content = Element?.BottomSheetContentDataTemplate?.CreateContent() as View,
                BindingContext = Element?.BindingContext
            };
        }

        private void CreateBottomSheet()
        {
            bottomSheetContentPage = CreateBottomSheetContentPage();
            bottomSheetViewController = bottomSheetContentPage.CreateViewController();
            bottomSheetNavigationController = ConfigureDefaultNavigationController();
            ConfigureSheetPresentationController();
        }

        #region Configuration
        private UISheetPresentationControllerDetent[] ConfigureBottomSheetStates(BottomSheetState? state)
        {
            return state switch
            {
                BottomSheetState.Medium => new UISheetPresentationControllerDetent[] { UISheetPresentationControllerDetent.CreateMediumDetent() },
                BottomSheetState.Large => new UISheetPresentationControllerDetent[] { UISheetPresentationControllerDetent.CreateLargeDetent() },
                BottomSheetState.MediumAndLarge => new UISheetPresentationControllerDetent[]
                {
                    UISheetPresentationControllerDetent.CreateMediumDetent(),
                    UISheetPresentationControllerDetent.CreateLargeDetent()
                },
                _ => new UISheetPresentationControllerDetent[] { UISheetPresentationControllerDetent.CreateMediumDetent() }
            };
        }

        private ButtomSheetUIViewController ConfigureDefaultNavigationController()
        {
            _ = bottomSheetViewController ?? throw new ArgumentException($"{nameof(bottomSheetViewController)} can not be null.");

            ButtomSheetUIViewController navigationController = new ButtomSheetUIViewController(Element, bottomSheetViewController);
            UINavigationBarAppearance standardAppearance = new UINavigationBarAppearance();
            standardAppearance.ConfigureWithOpaqueBackground();
            standardAppearance.ShadowColor = UIColor.Clear;

            UINavigationBar navBar = navigationController.NavigationBar;
            navBar.TopItem.Title = Element?.Title;
            navBar.StandardAppearance = standardAppearance;
            navBar.ScrollEdgeAppearance = standardAppearance;
            navBar.CompactAppearance = standardAppearance;
            navBar.Translucent = false;

            return navigationController;
        }

        private void ConfigureSheetPresentationController()
        {
            _ = Element ?? throw new ArgumentException($"{nameof(Element)} can not be null.");

            if (bottomSheetNavigationController?.SheetPresentationController != null)
            {
                bottomSheetNavigationController.SheetPresentationController.Detents = ConfigureBottomSheetStates(Element.BottomSheetState);
                bottomSheetNavigationController.SheetPresentationController.PrefersGrabberVisible = true; 
                bottomSheetNavigationController.SheetPresentationController.PrefersScrollingExpandsWhenScrolledToEdge = true;
                bottomSheetNavigationController.ModalInPresentation = Element.Cancelable == false;
                bottomSheetNavigationController.SheetPresentationController.LargestUndimmedDetentIdentifier = Element.IsModal == false ? UISheetPresentationControllerDetentIdentifier.Medium : UISheetPresentationControllerDetentIdentifier.Unknown;
            }
        }
        #endregion
        #endregion
    }
}
