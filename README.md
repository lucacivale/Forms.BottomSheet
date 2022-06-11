# Forms.BottomSheet

Native BottomSheets in Xamarin.Forms!

## Getting Started

No initialization neccessary! Install the package and you are good to go! 

### XAML usage

In order to make use of the sheet within XAML you can use this namespace:

'xmlns:bottomSheet="clr-namespace:Forms.BottomSheet;assembly=Forms.BottomSheet"'

### Bottom Sheet Control

The `BottomSheet` is basiclly a `ContentView`, so can set the content of your view directly.
```
<StackLayout>
    <bottomSheet:BottomSheet>
        <Label Text="Main Content"/>
    </bottomSheet:BottomSheet>
</StackLayout>
```
The sheet content is a `DataTemplate` which will be inflated on the first opening of the sheet.
```
<bottomSheet:BottomSheet.BottomSheetContentDataTemplate>
    <DataTemplate>
        <Label Text="I'm a bottom sheet"/>
    </DataTemplate>
</bottomSheet:BottomSheet.BottomSheetContentDataTemplate>
```
`IsOpen` will open or close the sheet. If the user closes the sheet by swiping down the property will be set accordingly.

`IsModal` indicates if the sheet should be presented modal. If the sheet is modal no interaction with the main content is possible.

`Cancelable`indicates if the sheet is cancelable.

`Title`creates a title at the top of the sheet.

`BottomSheetState` sets the states of the bottom sheet.
 - #### Unknown => The sheet can be any state.
 - #### Medium  => The sheet size will be 50% of the screen size and the sheet isn't expandable.
 - #### Lage => The sheet be full screen and not collapsable.
 - #### MediumAndLarge => initially opens in `Medium` state but can be expanded to `Large` state.

## Example
```
<StackLayout>
    <bottomSheet:BottomSheet IsOpen="True" BottomSheetState="MediumAndLarge" Title="I'm a bottom sheet!">
        <StackLayout>
            <Label Text="Main Content" HorizontalOptions="Center"/>
            <Switch/>
        </StackLayout>
        <bottomSheet:BottomSheet.BottomSheetContentDataTemplate>
            <DataTemplate>
                <CollectionView ItemsSource="{Binding Items}" SelectionMode="Single" SelectedItem="{Binding Item}" SelectionChangedCommand="{Binding SelectionChanged}"/>
            </DataTemplate>
        </bottomSheet:BottomSheet.BottomSheetContentDataTemplate>
    </bottomSheet:BottomSheet>
</StackLayout>
```
![Example](https://user-images.githubusercontent.com/56133182/173190335-fc9ef962-f380-40c1-985e-9dd8d3d108a3.gif)

