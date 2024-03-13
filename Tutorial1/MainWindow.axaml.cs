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

        // �� Avalonia �У�OpenGL ��صĿؼ���ֱ�������� OpenGlControlBase��
        // ��д OnOpenGlInit/OnOpenGlDeinit/OnOpenGlRender �������������ɣ�
        // ���������������÷ֱ��ǣ���ʼ��/�ͷ�/����
        class MyOpenGlControl : OpenGlControlBase
        {
            // �鿴 OpenGlControlBase.cs��fb ʵ������ FrameBuffer ����д
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
                // ���� https://ogldev.org/www/tutorial01/tutorial01.html����
                // The call above sets the color that will be used when clearing the framebuffer (described later).
                // The color has four channels (RGBA) and it is specified as a normalized value between 0.0 and 1.0.
                // ...
                // The only thing we do in our render function is to clear the framebuffer (using the color specified above - try changing it). 
                // ��� https://www.cnblogs.com/1024Planet/p/5643651.html����Ϊ
                // ͨ�� GlInterface.ClearColor ���������ʱʹ�õ���ɫ����ɫͨ�� RGBA ����ָ��
                // ͨ�� GlInterface.Clear ��������Ҫ����Ļ���������ͬʱ��ն��ֻ�������ʹ�� | ��ϣ���GL_COLOR_BUFFER_BIT ����ǰ��д����ɫ������
                gl.ClearColor(0.0f, 1.0f, 0.0f, 0.7f);
                gl.Clear(GL_COLOR_BUFFER_BIT);
            }
        }
    }
}