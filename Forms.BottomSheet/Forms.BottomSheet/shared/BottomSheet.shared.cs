using System;
using Xamarin.Forms;

namespace Forms.BottomSheet
{
    public class BottomSheet : ContentView
    {
        #region Binable Properties
        public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
            nameof(IsOpen),
            typeof(bool),
            typeof(BottomSheet),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: false);
        public static readonly BindableProperty BottomSheetStateProperty = BindableProperty.Create(
            nameof(BottomSheetState),
            typeof(BottomSheetState),
            typeof(BottomSheet),
            defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty IsModalProperty = BindableProperty.Create(
            nameof(IsModal),
            typeof(bool),
            typeof(BottomSheet),
            defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(BottomSheet),
            defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty CancelableProperty = BindableProperty.Create(
            nameof(Cancelable),
            typeof(bool),
            typeof(BottomSheet),
            defaultBindingMode: BindingMode.TwoWay);
        #endregion

        #region Properties
        public DataTemplate? BottomSheetContentDataTemplate { get; set; }
        public BottomSheetState BottomSheetState { get => (BottomSheetState)GetValue(BottomSheetStateProperty); set => SetValue(BottomSheetStateProperty, value); }
        public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
        public bool IsModal { get => (bool)GetValue(IsModalProperty); set => SetValue(IsModalProperty, value); }
        public bool Cancelable { get => (bool)GetValue(CancelableProperty); set => SetValue(CancelableProperty, value); }
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        #endregion

        #region Constructor
        public BottomSheet()
        {
            VerticalOptions = LayoutOptions.FillAndExpand;
        }
        #endregion
    }
}
