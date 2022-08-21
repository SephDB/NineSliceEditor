using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NineSliceEditor.Controls;
using NineSliceEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

#nullable disable
namespace NineSliceEditor.WpfControls
{
    public class ShowRect : WpfGame
    {
        public System.Windows.Media.Color Background { get; set; }

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof(Target), typeof(Rectangle), typeof(ShowRect),
            new FrameworkPropertyMetadata(new Rectangle(10, 10, 100, 100), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnTargetChanged)));

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowRect target = (ShowRect)d;
            if (target.nineSlice is not null) target.nineSlice.center = (Rectangle)e.NewValue;
        }

        public Rectangle Target
        {
            get => (Rectangle)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(Uri), typeof(ShowRect),
            new FrameworkPropertyMetadata(default(Uri), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnImageChanged)));

        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowRect target = (ShowRect)d;

            target.image = Texture2D.FromFile(target.GraphicsDevice, ((Uri)e.NewValue).AbsolutePath);
            target.resizer = new(target.image.Bounds);
            target.nineSlice = new(target.image, target.Target);
        }

        public Uri Image
        {
            get => (Uri)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(int), typeof(ShowRect),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnScaleChanged)));

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowRect target = (ShowRect)d;
            target.resizer.control_grace = Math.Max(1, 10 / (int)e.NewValue);
        }

        public int Scale
        {
            get => (int)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }


        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch spriteBatch;
        private WpfMouse _mouse;
        private Color _bg_color { get => new(Background.R, Background.G, Background.B); }

        private Texture2D image;
        private RectangleControls resizer;
        private NineSlice nineSlice;

        private UIControl ui;

        protected override void Initialize()
        {
            // must be initialized. required by Content loading and rendering (will add itself to the Services)
            // note that MonoGame requires this to be initialized in the constructor, while WpfInterop requires it to
            // be called inside Initialize (before base.Initialize())
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);
            spriteBatch = new(_graphicsDeviceManager.GraphicsDevice);

            // wpf and keyboard need reference to the host control in order to receive input
            // this means every WpfGame control will have it's own keyboard & mouse manager which will only react if the mouse is in the control
            _mouse = new WpfMouse(this);

            ui = new(_mouse, this);

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();

            image = Content.Load<Texture2D>("Textures/volume_bar");
            resizer = new(image.Bounds);
            nineSlice = new(image, Target);
        }

        protected override void Update(GameTime time)
        {
            if (!IsActive) return;
            ui.Update(Enumerable.Repeat(resizer, 1));
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            VisualBitmapScalingMode = System.Windows.Media.BitmapScalingMode.NearestNeighbor;
            base.OnRender(dc);
        }

        protected override void Draw(GameTime time)
        {
            if (!IsActive) return;
            _graphicsDeviceManager.GraphicsDevice.Clear(_bg_color);
            spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            nineSlice.Draw(spriteBatch, resizer.Rect);
            spriteBatch.End();
        }
    }
}
