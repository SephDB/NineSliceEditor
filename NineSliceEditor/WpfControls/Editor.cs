using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NineSliceEditor.Controls;
using NineSliceEditor.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

#nullable disable
namespace NineSliceEditor.WpfControls
{
    public class Editor : WpfGame
    {
        public System.Windows.Media.Color Background { get; set; }

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof(Target), typeof(Rectangle), typeof(Editor),
            new FrameworkPropertyMetadata(new Rectangle(10, 10, 100, 100), 
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnTargetChanged)));

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Editor target = (Editor)d;
            if (target.resizer is not null) target.resizer.Center = (Rectangle)e.NewValue;
        }

        public Rectangle Target
        {
            get => (Rectangle)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(Uri), typeof(Editor),
            new FrameworkPropertyMetadata(default(Uri),FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnImageChanged)));

        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Editor target = (Editor)d;

            target.image = Texture2D.FromFile(target.GraphicsDevice,((Uri)e.NewValue).AbsolutePath);
            target.resizer = new(target.primitives, new Rectangle(0, 0, (int)target.ActualWidth, (int)target.ActualHeight), target.image);
            target.Target = target.resizer.Center;
        }

        public Uri Image
        {
            get => (Uri)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch spriteBatch;
        private WpfMouse _mouse;
        private Color _bg_color { get => new(Background.R, Background.G, Background.B); }

        private Texture2D image;
        private RegionSelector resizer;
        private PrimitivesDrawer primitives;

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

            primitives = new(_graphicsDeviceManager.GraphicsDevice);
            resizer = new(primitives, new Rectangle(), image);
            Target = resizer.Center;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            resizer.ResetPosition(new(0, 0, (int)sizeInfo.NewSize.Width, (int)sizeInfo.NewSize.Height));
        }

        protected override void Update(GameTime time)
        {
            if (!IsActive) return;

            Rectangle r = resizer.Center;
            ui.Update(Enumerable.Repeat(resizer, 1));
            if(resizer.Center != r)
            {
                Target = resizer.Center;
            }
        }

        protected override void Draw(GameTime time)
        {
            if (!IsActive) return;
            _graphicsDeviceManager.GraphicsDevice.Clear(_bg_color);
            spriteBatch.Begin(samplerState:SamplerState.PointWrap);
            resizer.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
