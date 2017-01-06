using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace SwpieControlUWP
{
    [ContentProperty(Name = "Content")]
    public sealed class SwipeControl : Control
    {
        private static readonly double SIGNFICANT_TRANSLATE_FACTOR = 0.40d;
        private FrameworkElement contentElement;
        public event EventHandler<SwipeEventArgs> swipeEventHandler;

        public object LeftContent
        {
            get { return (object)GetValue(LeftContentProperty); }
            set { SetValue(LeftContentProperty, value); }
        }
        public static readonly DependencyProperty LeftContentProperty =
            DependencyProperty.Register(nameof(LeftContent), typeof(object), typeof(SwipeControl), new PropertyMetadata(null));

        public object RightContent
        {
            get { return (object)GetValue(RightContentProperty); }
            set { SetValue(RightContentProperty, value); }
        }
        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register(nameof(RightContent), typeof(object), typeof(SwipeControl), new PropertyMetadata(null));

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(SwipeControl), new PropertyMetadata(null));

        public DataTemplate LeftContentTemplate
        {
            get { return (DataTemplate)GetValue(LeftContentTemplateProperty); }
            set { SetValue(LeftContentTemplateProperty, value); }
        }
        public static readonly DependencyProperty LeftContentTemplateProperty =
            DependencyProperty.Register(nameof(LeftContentTemplate), typeof(DataTemplate), typeof(SwipeControl), new PropertyMetadata(null));

        public DataTemplate RightContentTemplate
        {
            get { return (DataTemplate)GetValue(RightContentTemplateProperty); }
            set { SetValue(RightContentTemplateProperty, value); }
        }
        public static readonly DependencyProperty RightContentTemplateProperty =
            DependencyProperty.Register(nameof(RightContentTemplate), typeof(DataTemplate), typeof(SwipeControl), new PropertyMetadata(null));

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(SwipeControl), new PropertyMetadata(null));

        public SwipeControl()
        {
            this.DefaultStyleKey = typeof(SwipeControl);
            this.ManipulationMode = ManipulationModes.TranslateX;
            this.ManipulationDelta += SwipeControl_ManipulationDelta;
            this.ManipulationCompleted += SwipeControl_ManipulationCompleted;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.contentElement = this.GetTemplateChild("ContentPresenter") as FrameworkElement;
        }

        private void SwipeControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            this.SetVisualStateForManipulation(e.Cumulative.Translation);
            this.TranslateContent(e.Cumulative.Translation.X);
        }

        private void SwipeControl_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Cumulative.Translation.X);
            if (e.Cumulative.Translation.X < 0)
            {
                this.TranslateContent(-(RightContent as FrameworkElement).ActualWidth / 3);
            }
            else
            {
                this.TranslateContent((LeftContent as FrameworkElement).ActualWidth / 3);
            }
            if (IsManipulationSignificant(e.Cumulative.Translation.X))
            {
                var args = new SwipeEventArgs()
                {
                    Direction = e.Cumulative.Translation.X > 0 ? SwipeDirection.Left : SwipeDirection.Right
                };
                this.swipeEventHandler?.Invoke(this, args);
                System.Diagnostics.Debug.WriteLine(args.Direction);
            }
        }

        private bool IsManipulationSignificant(double x)
        {
            var result = Math.Abs(x) > (SIGNFICANT_TRANSLATE_FACTOR * this.contentElement.ActualWidth);
            return result;
        }

        private void SetVisualStateForManipulation(Point point)
        {
            var direction = this.directionOfManipulation(point);
            VisualStateManager.GoToState(this, direction.ToString(), true);
        }

        private void TranslateContent(double x)
        {
            if (this.contentElement != null)
            {
                var transform = (this.contentElement.RenderTransform as TranslateTransform);
                if (transform == null)
                    transform = new TranslateTransform();
                this.contentElement.RenderTransform = transform;
                transform.X = x;
            }
        }

        private SwipeDirection directionOfManipulation(Point point)
        {
            var defaultDirection = SwipeDirection.Default;
            defaultDirection = point.X > 0 ? SwipeDirection.Left : SwipeDirection.Right;
            return defaultDirection;
        }
    }

    public class SwipeEventArgs : EventArgs
    {
        public SwipeDirection Direction { get; set; }
    }

    public enum SwipeDirection
    {
        Left, Right, Default
    }
}
