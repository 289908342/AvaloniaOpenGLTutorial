using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Common;
using static Avalonia.OpenGL.GlConsts;

namespace Tutorial1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MyOpenGlControl myControl = new MyOpenGlControl();

            this.Content = myControl;
        }

        // 在 Avalonia 中，OpenGL 相关的控件可直接派生自 OpenGlControlBase，
        // 覆写 OnOpenGlInit/OnOpenGlDeinit/OnOpenGlRender 这三个方法即可，
        // 这三个方法的作用分别是：初始化/释放/绘制
        class MyOpenGlControl : OpenGlControlBase
        {
            // 查看 OpenGlControlBase.cs，fb 实际上是 FrameBuffer 的缩写
            protected override void OnOpenGlInit(GlInterface gl, int fb)
            {
                base.OnOpenGlInit(gl, fb);
            }

            protected override void OnOpenGlDeinit(GlInterface gl, int fb)
            {
                base.OnOpenGlDeinit(gl, fb);
            }

            protected override void OnOpenGlRender(GlInterface gl, int fb)
            {
                // 根据 https://ogldev.org/www/tutorial01/tutorial01.html，有
                // The call above sets the color that will be used when clearing the framebuffer (described later).
                // The color has four channels (RGBA) and it is specified as a normalized value between 0.0 and 1.0.
                // ...
                // The only thing we do in our render function is to clear the framebuffer (using the color specified above - try changing it). 
                // 结合 https://www.cnblogs.com/1024Planet/p/5643651.html，认为
                // 通过 GlInterface.ClearColor 设置了清除时使用的颜色，颜色通过 RGBA 进行指定
                // 通过 GlInterface.Clear 设置了需要清除的缓冲区（可同时清空多种缓冲区，使用 | 组合），GL_COLOR_BUFFER_BIT 代表当前可写的颜色缓冲区
                gl.ClearColor(0.0f, 1.0f, 0.0f, 0.7f);
                gl.Clear(GL_COLOR_BUFFER_BIT);
            }
        }
    }
}