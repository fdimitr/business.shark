using BusinessSharkClient.Logic.Models;
using Syncfusion.Maui.Popup;

namespace BusinessSharkClient.View.DivisionDetail;

public partial class DivisionDetailEditPopupView : SfPopup
{
    public static readonly BindableProperty HeaderProperty =
        BindableProperty.Create(nameof(Header), typeof(string), typeof(DivisionDetailEditPopupView));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly BindableProperty SawmillNameProperty =
        BindableProperty.Create(nameof(SawmillName), typeof(string), typeof(DivisionDetailEditPopupView));

    public string SawmillName
    {
        get => (string)GetValue(SawmillNameProperty);
        set => SetValue(SawmillNameProperty, value);
    }

    public static readonly BindableProperty SizesProperty =
        BindableProperty.Create(nameof(Sizes), typeof(List<DivisionSizeModel>), typeof(DivisionDetailEditPopupView));

    public List<DivisionSizeModel> Sizes
    {
        get => (List<DivisionSizeModel>)GetValue(SizesProperty);
        set => SetValue(SizesProperty, value);
    }

    public DivisionDetailEditPopupView()
    {
        InitializeComponent();
    }
}