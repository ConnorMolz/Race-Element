﻿using RaceElement.Data.Games;
using RaceElement.HUD.Overlay.Configuration;
using RaceElement.Util.SystemExtensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static RaceElement.HUD.Overlay.Configuration.OverlayConfiguration;

namespace RaceElement.Controls.HUD.Controls.ValueControls;

internal sealed class FloatValueControl : IValueControl<float>
{
    private readonly Grid _grid;
    private readonly Label _label;
    private readonly Slider _slider;

    public FrameworkElement Control => _grid;
    public float Value { get; set; }
    private readonly ConfigField _field;

    public FloatValueControl(FloatRangeAttribute floatRange, ConfigField configField)
    {
        _field = configField;

        _grid = new Grid()
        {
            Width = ControlConstants.ControlWidth,
            Margin = new Thickness(0, 0, 7, 0),
            Background = new SolidColorBrush(Color.FromArgb(140, 2, 2, 2)),
            Cursor = Cursors.Hand
        };
        _grid.PreviewMouseLeftButtonUp += (s, e) => Save();
        _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
        _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });

        _label = new Label()
        {
            HorizontalContentAlignment = HorizontalAlignment.Right,
            FontWeight = FontWeights.Bold,
            FontSize = 13,
        };
        _grid.Children.Add(_label);
        Grid.SetColumn(_label, 0);

        _slider = new Slider()
        {
            Minimum = floatRange.GetMin(GameManager.CurrentGame),
            Maximum = floatRange.GetMax(GameManager.CurrentGame),
            TickFrequency = floatRange.Increment,
            IsSnapToTickEnabled = true,
            Width = 220
        };
        _slider.ValueChanged += (s, e) =>
        {
            _field.Value = _slider.Value.ToString($"F{floatRange.Decimals}");
            UpdateLabel(floatRange.Decimals);
        };

        float value = float.Parse(configField.Value.ToString());
        value.Clip(floatRange.GetMin(GameManager.CurrentGame), floatRange.GetMax(GameManager.CurrentGame));
        _slider.Value = value;

        _grid.Children.Add(_slider);
        _slider.HorizontalAlignment = HorizontalAlignment.Right;
        _slider.VerticalAlignment = VerticalAlignment.Center;
        Grid.SetColumn(_slider, 1);

        Control.MouseWheel += (sender, args) =>
        {
            int delta = args.Delta;
            _slider.Value += delta.Clip(-1, 1) * floatRange.Increment;
            args.Handled = true;
            Save();
        };

        UpdateLabel(floatRange.Decimals);
    }

    private void UpdateLabel(int decimals)
    {
        _label.Content = $"{_slider.Value.ToString($"F{decimals}")}";
    }

    public void Save()
    {
        ConfigurationControls.SaveOverlayConfigField(_field);
    }
}
