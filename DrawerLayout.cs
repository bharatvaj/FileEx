﻿using System;
using System.Linq;
using System.Windows;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace FileEx
{
    public class DrawerLayout : Grid
    {
        #region Globals and events
        private readonly PropertyPath _translatePath = new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)");
        private readonly PropertyPath _colorPath = new PropertyPath("(Grid.Background).(SolidColorBrush.Color)");

        private readonly TranslateTransform _listFragmentTransform = new TranslateTransform();
        private readonly TranslateTransform _deltaTransform = new TranslateTransform();
        private const int MaxAlpha = 245;

        public delegate void DrawerEventHandler(object sender);
        public event DrawerEventHandler DrawerOpened;
        public event DrawerEventHandler DrawerClosed;

        private Storyboard _fadeInStoryboard;
        private Storyboard _fadeOutStoryboard;
        private Grid _listFragment;
        private Grid _mainFragment;
        private Grid _shadowFragment;
        private double grabCornerLeft = 40;
        #endregion

        #region Properties

        public bool IsDrawerOpen { get; set; }
        private PropertyPath TranslatePath
        {
            get { return _translatePath; }
        }
        private PropertyPath ColorPath
        {
            get { return _colorPath; }
        }

        #endregion

        #region Methods

        public DrawerLayout()
        {
            IsDrawerOpen = false;
        }

        public void InitializeDrawerLayout()
        {
            if (Children == null) return;
            if (Children.Count < 2) return;

            try
            {
                _mainFragment = Children.OfType<Grid>().ElementAt(1);
                _listFragment = Children.OfType<Grid>().ElementAt(2);
            }
            catch (Exception)
            {
                //ShowStatus(ex.Message);
                return;
            }

            if (_mainFragment == null || _listFragment == null) return;

            _mainFragment.Name = "_mainFragment";
            _listFragment.Name = "_listFragment";

            // _mainFragment
            _mainFragment.HorizontalAlignment = HorizontalAlignment.Left;
            _mainFragment.VerticalAlignment = VerticalAlignment.Stretch;

            // Render transform _listFragment
            _listFragment.HorizontalAlignment = HorizontalAlignment.Left;
            _listFragment.VerticalAlignment = VerticalAlignment.Stretch;
            _listFragment.Width = Window.Current.Bounds.Width;//Window.Current.Bounds.Width > Window.Current.Bounds.Height ? (Window.Current.Bounds.Height * 2) / 3 : (Window.Current.Bounds.Width * 2) / 3; //for landscape support + partial list
            //if (_listFragment.Background == null) _listFragment.Background = new SolidColorBrush(Color.FromArgb(40, 46, 46, 46));

            var animatedTranslateTransform = new TranslateTransform { X = -_listFragment.Width, Y = 0 };

            _listFragment.RenderTransform = animatedTranslateTransform;
            _listFragment.RenderTransformOrigin = new Point(0.5, 0.5);

            _listFragment.UpdateLayout();


            // Create a shadow element
            _shadowFragment = new Grid
            {
                Name = "_shadowFragment",
                Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Visibility = Visibility.Collapsed
            };

            // Set ZIndexes
            Canvas.SetZIndex(_shadowFragment, 49);
            Canvas.SetZIndex(_listFragment, 51);
            Children.Add(_shadowFragment);

            // Create a new fadeIn animation storyboard
            _fadeInStoryboard = new Storyboard();

            // New double animation
            var doubleAnimation1 = new DoubleAnimation
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100)),
                To = 0
            };

            Storyboard.SetTarget(doubleAnimation1, _listFragment);
            Storyboard.SetTargetProperty(doubleAnimation1, TranslatePath.Path);
            _fadeInStoryboard.Children.Add(doubleAnimation1);

            // New color animation for _shadowFragment
            var colorAnimation1 = new ColorAnimation
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100)),
                To = Color.FromArgb(245, 0, 0, 0)
            };

            Storyboard.SetTarget(colorAnimation1, _shadowFragment);
            Storyboard.SetTargetProperty(colorAnimation1, ColorPath.Path);
            _fadeInStoryboard.Children.Add(colorAnimation1);

            // Create a new fadeOut animation storyboard
            _fadeOutStoryboard = new Storyboard();

            // New double animation
            var doubleAnimation2 = new DoubleAnimation
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100)),
                To = -_listFragment.Width
            };

            Storyboard.SetTarget(doubleAnimation2, _listFragment);
            Storyboard.SetTargetProperty(doubleAnimation2, TranslatePath.Path);
            _fadeOutStoryboard.Children.Add(doubleAnimation2);

            // New color animation for _shadowFragment
            var colorAnimation2 = new ColorAnimation
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100)),
                To = Color.FromArgb(0, 0, 0, 0)
            };

            Storyboard.SetTarget(colorAnimation2, _shadowFragment);
            Storyboard.SetTargetProperty(colorAnimation2, ColorPath.Path);
            _fadeOutStoryboard.Children.Add(colorAnimation2);


            _mainFragment.ManipulationMode = ManipulationModes.TranslateX;
            _mainFragment.ManipulationStarted += mainFragment_ManipulationStarted;

            //_listFragment.ManipulationMode = ManipulationModes.TranslateX;
            //_listFragment.ManipulationStarted += listFragment_ManipulationStarted;

            //_shadowFragment.ManipulationMode = ManipulationModes.TranslateX;
            //_shadowFragment.ManipulationStarted += shadowFragment_ManipulationStarted;

            //_shadowFragment.Tapped += shadowFragment_Tapped;

        }


        public void OpenDrawer()
        {
            if (_fadeInStoryboard == null || _mainFragment == null || _listFragment == null) return;
            _shadowFragment.Visibility = Visibility.Visible;
            _shadowFragment.IsHitTestVisible = true;
            _fadeInStoryboard.Begin();
            IsDrawerOpen = true;

            if (DrawerOpened != null)
                DrawerOpened(this);
        }
        public void CloseDrawer()
        {
            if (_fadeOutStoryboard == null || _mainFragment == null || _listFragment == null) return;
            _fadeOutStoryboard.Begin();
            _fadeOutStoryboard.Completed += fadeOutStoryboard_Completed;
            IsDrawerOpen = false;

            if (DrawerClosed != null)
                DrawerClosed(this);
        }
        private void shadow_Completed(object sender, object e)
        {
            _shadowFragment.IsHitTestVisible = false;
            _shadowFragment.Visibility = Visibility.Collapsed;
            IsDrawerOpen = false;

            // raise close event
            if (DrawerClosed != null) DrawerClosed(this);
        }
        private void fadeOutStoryboard_Completed(object sender, object e)
        {
            _shadowFragment.Visibility = Visibility.Collapsed;
            IsDrawerOpen = false;
            if (DrawerClosed != null) DrawerClosed(this);
        }
        private void MoveListFragment(double left, Color color)
        {
            var s = new Storyboard();

            var doubleAnimation = new DoubleAnimation
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100)),
                To = left
            };

            Storyboard.SetTarget(doubleAnimation, _listFragment);
            Storyboard.SetTargetProperty(doubleAnimation, TranslatePath.Path);
            s.Children.Add(doubleAnimation);

            var colorAnimation = new ColorAnimation { Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100)), To = color };

            Storyboard.SetTarget(colorAnimation, _shadowFragment);
            Storyboard.SetTargetProperty(colorAnimation, ColorPath.Path);
            s.Children.Add(colorAnimation);

            s.Begin();
        }
        private void FadeShadowFragment(double left)
        {
            // Show shadow fragment
            //_shadowFragment.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            _shadowFragment.Visibility = Visibility.Visible;

            // Set bg color based on current _listFragment position.
            var maxLeft = _listFragment.ActualWidth;
            var currentLeft = maxLeft - left;

            var temp = Convert.ToInt32((currentLeft / maxLeft) * MaxAlpha);

            // Limit temp variable to 190 to avoid OverflowException
            if (temp > MaxAlpha) temp = MaxAlpha;

            byte alphaColorIndex;
            try
            {
                alphaColorIndex = Convert.ToByte(MaxAlpha - temp);
            }
            catch
            {
                alphaColorIndex = 0;
            }

            _shadowFragment.Background = new SolidColorBrush(Color.FromArgb(alphaColorIndex, 0, 0, 0));
        }

        #endregion


        #region Main fragment manipulation events

        private void mainFragment_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            // If the user has the first touch on the left side of canvas, that means he's trying to swipe the drawer
            if (e.Position.X <= grabCornerLeft)
            {
                //Manipulation can be allowed
                _mainFragment.ManipulationDelta += mainFragment_ManipulationDelta;
                _mainFragment.ManipulationCompleted += mainFragment_ManipulationCompleted;
            }
        }
        private void mainFragment_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (Math.Abs(e.Cumulative.Translation.X) > 0 && e.Cumulative.Translation.X <= _listFragment.Width)
            {
                _deltaTransform.X = -_listFragment.Width + e.Cumulative.Translation.X;
                _listFragment.RenderTransform = _deltaTransform;
                FadeShadowFragment(e.Cumulative.Translation.X);
                //ShowStatus(e.Cumulative.Translation.X.ToString());
            }
            //if ()
            //{
            //    //mainFragment_ManipulationCompleted(this, null);
            //    return;
            //}

        }
        private void mainFragment_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            // Get left of _listFragment
            var transform = (TranslateTransform)_listFragment.RenderTransform;
            if (transform == null) return;
            var left = transform.X;

            // Set snap divider to 1/20 of _mainFragment width
            var snapLimit = _mainFragment.ActualWidth / 20;

            // Get init position of _listFragment
            var initialPosition = -_listFragment.Width;

            // If current left coordinate is smaller than snap limit, close drawer
            if (Math.Abs(initialPosition - left) < snapLimit + 50)
            {
                MoveListFragment(initialPosition, Color.FromArgb(0, 0, 0, 0));
                _shadowFragment.Visibility = Visibility.Collapsed;
                _shadowFragment.IsHitTestVisible = false;

                _mainFragment.ManipulationDelta -= mainFragment_ManipulationDelta;
                _mainFragment.ManipulationCompleted -= mainFragment_ManipulationCompleted;
                IsDrawerOpen = false;

                // raise DrawerClosed event
                if (DrawerClosed != null) DrawerClosed(this);
            }
            // else open drawer
            else if (Math.Abs(initialPosition - left) > snapLimit)
            {
                // move drawer to zero
                MoveListFragment(0, Color.FromArgb(245, 0, 0, 0));
                _shadowFragment.Visibility = Visibility.Visible;
                _shadowFragment.IsHitTestVisible = true;
                _mainFragment.ManipulationDelta -= mainFragment_ManipulationDelta;
                _mainFragment.ManipulationCompleted -= mainFragment_ManipulationCompleted;
                IsDrawerOpen = true;

                // raise DrawerClosed event
                if (DrawerOpened != null) DrawerOpened(this);
            }
        }

        #endregion

        #region List Fragment manipulation events

        private void listFragment_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            var listWidth = _listFragment.Width;
            if (e.Position.X >= listWidth) return;
            _listFragment.ManipulationDelta += listFragment_ManipulationDelta;
            _listFragment.ManipulationCompleted += listFragment_ManipulationCompleted;
        }
        private void listFragment_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X <= -_listFragment.Width)
            {
                listFragment_ManipulationCompleted(this, null);
                return;
            }
            else if (_listFragmentTransform.X >= 0 && e.Delta.Translation.X > 0)
            {
                listFragment_ManipulationCompleted(this, null);
                return;
            }
            _listFragmentTransform.X = e.Cumulative.Translation.X;
            _listFragment.RenderTransform = _listFragmentTransform;
            FadeShadowFragment(e.Cumulative.Translation.X + _listFragment.Width);
        }
        private void listFragment_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            // Get left of _listFragment
            var transform = (TranslateTransform)_listFragment.RenderTransform;
            if (transform == null) return;
            var left = transform.X;

            // Set snap divider to 1/4 of _mainFragment height
            var snapLimit = _mainFragment.ActualHeight / 4;

            // Get init position of _listFragment
            const int initialPosition = 0;

            // If current left coordinate is smaller than snap limit, close drawer
            if (Math.Abs(initialPosition - left) > snapLimit)
            {
                MoveListFragment(-_listFragment.Width, Color.FromArgb(0, 0, 0, 0));
                _shadowFragment.Visibility = Visibility.Collapsed;
                _shadowFragment.IsHitTestVisible = false;
                _shadowFragment.ManipulationDelta -= listFragment_ManipulationDelta;

                _listFragment.ManipulationDelta -= listFragment_ManipulationDelta;
                _listFragment.ManipulationCompleted -= listFragment_ManipulationCompleted;
                IsDrawerOpen = false;

                // raise DrawerClosed event
                if (DrawerClosed != null) DrawerClosed(this);
            }
            // else open drawer
            else if (Math.Abs(initialPosition - left) < snapLimit)
            {
                // move drawer to zero
                MoveListFragment(0, Color.FromArgb(190, 0, 0, 0));
                _shadowFragment.Visibility = Visibility.Visible;
                _shadowFragment.IsHitTestVisible = true;
                _shadowFragment.ManipulationDelta += listFragment_ManipulationDelta;

                _listFragment.ManipulationDelta -= listFragment_ManipulationDelta;
                _listFragment.ManipulationCompleted -= listFragment_ManipulationCompleted;
                IsDrawerOpen = true;

                // raise Drawer_Open event
                if (DrawerOpened != null) DrawerOpened(this);
            }
        }

        #endregion
        #region Shadow fragment manipulation events
        private void shadowFragment_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X > 0) return;
            _shadowFragment.ManipulationDelta += _shadowFragment_ManipulationDelta;
            _shadowFragment.ManipulationCompleted += _shadowFragment_ManipulationCompleted;
        }

        private void _shadowFragment_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X <= -_listFragment.Width)
            {
                _shadowFragment_ManipulationCompleted(this, null);
                return;
            }
            else if (_listFragmentTransform.X >= 0 && e.Delta.Translation.X > 0)
            {
                _shadowFragment_ManipulationCompleted(this, null);
                return;
            }
            _listFragmentTransform.X = e.Cumulative.Translation.X;
            _listFragment.RenderTransform = _listFragmentTransform;
            FadeShadowFragment(e.Cumulative.Translation.X + _listFragment.Width);

        }
        private void _shadowFragment_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            // Get left of _listFragment
            var transform = (TranslateTransform)_listFragment.RenderTransform;
            if (transform == null) return;
            var left = transform.X;

            // Set snap divider to 1/4 of _mainFragment height
            var snapLimit = _mainFragment.ActualHeight / 4;

            // Get init position of _listFragment
            const int initialPosition = 0;

            // If current left coordinate is smaller than snap limit, close drawer
            if (Math.Abs(initialPosition - left) > snapLimit)
            {
                MoveListFragment(-_listFragment.Width, Color.FromArgb(0, 0, 0, 0));

                _shadowFragment.Visibility = Visibility.Collapsed;
                _shadowFragment.IsHitTestVisible = false;
                _shadowFragment.ManipulationDelta -= _shadowFragment_ManipulationDelta;
                _shadowFragment.ManipulationCompleted -= _shadowFragment_ManipulationCompleted;

                _listFragment.ManipulationDelta -= listFragment_ManipulationDelta;
                _listFragment.ManipulationCompleted -= listFragment_ManipulationCompleted;

                IsDrawerOpen = false;

                // raise DrawerClosed event
                if (DrawerClosed != null) DrawerClosed(this);
            }
            // else open drawer
            else if (Math.Abs(initialPosition - left) < snapLimit)
            {
                // move drawer to zero
                MoveListFragment(0, Color.FromArgb(190, 0, 0, 0));

                _shadowFragment.Visibility = Visibility.Visible;
                _shadowFragment.IsHitTestVisible = true;
                _shadowFragment.ManipulationDelta -= _shadowFragment_ManipulationDelta;
                _shadowFragment.ManipulationCompleted -= _shadowFragment_ManipulationCompleted;

                _listFragment.ManipulationDelta -= listFragment_ManipulationDelta;
                _listFragment.ManipulationCompleted -= listFragment_ManipulationCompleted;

                IsDrawerOpen = true;

                // raise Drawer_Open event
                if (DrawerOpened != null) DrawerOpened(this);
            }
        }

        private void shadowFragment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var shadow = new Storyboard();

            var doubleAnimation = new DoubleAnimation
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100)),
                To = -_listFragment.Width - 100
            };

            Storyboard.SetTarget(doubleAnimation, _listFragment);
            Storyboard.SetTargetProperty(doubleAnimation, TranslatePath.Path);
            shadow.Children.Add(doubleAnimation);

            var colorAnimation = new ColorAnimation
            {
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100)),
                To = Color.FromArgb(0, 0, 0, 0)
            };

            Storyboard.SetTarget(colorAnimation, _shadowFragment);
            Storyboard.SetTargetProperty(colorAnimation, ColorPath.Path);
            shadow.Children.Add(colorAnimation);

            shadow.Completed += shadow_Completed;
            shadow.Begin();
            _shadowFragment.IsHitTestVisible = false;
        }
        #endregion
        //#region ShowStatus
        //public void ShowStatus(string info)
        //{
        //    rootPage.NotifyUser(info);
        //}
        //#endregion

    }
}
