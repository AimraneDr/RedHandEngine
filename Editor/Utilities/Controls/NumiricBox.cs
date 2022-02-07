using Editor.Utilities.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Editor.Utilities.Controls
{
    [TemplatePart(Name ="PART_TextBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    internal class NumiricBox : Control
    {
        private double _originalValue;
        private double _mousePosXStart;
        private double _muliplier;
        private bool _mouseCaptured = false;
        private bool _valueChanged = false;
        public double Multiplier
        {
            get => (double)GetValue(MultiplierProperty);
            set => SetValue(MultiplierProperty, value);
        }
        public static readonly DependencyProperty MultiplierProperty =
            DependencyProperty.Register(nameof(Multiplier), typeof(double), typeof(NumiricBox),
            new FrameworkPropertyMetadata(1.0));
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumiricBox),
            new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        static NumiricBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumiricBox), new FrameworkPropertyMetadata(typeof(NumiricBox)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if(GetTemplateChild("PART_TextBlock") is TextBlock textBlock)
            {
                textBlock.MouseLeftButtonDown += OnTextBlock_MouseLeftButton_Down;
                textBlock.MouseLeftButtonUp += OnTextBlock_MouseLeftButton_Up;
                textBlock.MouseMove+= OnTextBlock_MouseMove;
            }
        }

        


        private void OnTextBlock_MouseLeftButton_Down(object sender, MouseButtonEventArgs e)
        {
            double.TryParse(Value, out _originalValue);
            Mouse.Capture(sender as UIElement);
            _mouseCaptured = true;
            _valueChanged = false;
            e.Handled = true;
            _mousePosXStart = e.GetPosition(this).X;
            Focus();
        }

        private void OnTextBlock_MouseLeftButton_Up(object sender, MouseButtonEventArgs e)
        {
            if (_mouseCaptured)
            {
                Mouse.Capture(null);
                _mouseCaptured = false;
                e.Handled= true;
                if(!_valueChanged && GetTemplateChild("PART_TextBox") is TextBox textBox)
                {
                    textBox.Visibility = Visibility.Visible;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private void OnTextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseCaptured)
            {
                var mouseX = e.GetPosition(this).X;
                var d = mouseX - _mousePosXStart;
                if(Math.Abs(d) > SystemParameters.MinimumHorizontalDragDistance)
                {
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) _muliplier = 0.001;
                    else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) _muliplier = 0.1;
                    else _muliplier = 0.01;

                    var newValue = _originalValue + (d * _muliplier * Multiplier);
                    Value = newValue.ToString("0.######");
                    _valueChanged = true;
                }

            }
        }

    }
}
