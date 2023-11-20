using Microsoft.Maui.Controls;

namespace Dissectr.Views;

public partial class LabelledEntry : ContentView
{
	public static BindableProperty LabelTextProperty = BindableProperty.Create(
		nameof(LabelText), typeof(string), typeof(LabelledEntry));

	public string LabelText
	{
		get => (string)GetValue(LabelTextProperty);
		set => SetValue(LabelTextProperty, value);
	}

	public static BindableProperty TextProperty = BindableProperty.Create(
		nameof(Text), typeof(string), typeof(LabelledEntry), defaultBindingMode: BindingMode.TwoWay);

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public LabelledEntry()
	{
		InitializeComponent();
	}
}
