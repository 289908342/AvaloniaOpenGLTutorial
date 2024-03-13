using System;
using System.Numerics;

using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using static Avalonia.OpenGL.GlConsts;
using static Common.GlConstExtensions;

namespace Tutorial2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MyOpenGlControl myControl = new MyOpenGlControl();
            Content = myControl;
        }

        // 绘制步骤整理：
        // - 构建顶点缓冲对象 VBO
        // - 构建顶点缓冲数组 VAO
        // - 构建顶点着色器，片段着色器，编译后附加到程序上
        // - 绘制图元
        unsafe class MyOpenGlControl : OpenGlControlBase
        {
            protected override void OnOpenGlInit(GlInterface gl, int fb)
            {
                base.OnOpenGlInit(gl, fb);

                ConfigureShaders(gl);
                CreateVertexBuffer(gl);

                gl.CheckError();
            }

            protected override void OnOpenGlDeinit(GlInterface gl, int fb)
            {
                base.OnOpenGlDeinit(gl, fb);
                
                gl.BindBuffer(GL_ARRAY_BUFFER, 0);
                gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
                gl.BindVertexArray(0);
                gl.UseProgram(0);

                gl.DeleteBuffer(_vbo);
                gl.DeleteVertexArray(_vao);
                gl.DeleteProgram(_shaderProgram);
                gl.DeleteShader(_fragmentShader);
                gl.DeleteShader(_vertexShader);
                
                gl.CheckError();
            }

            void ConfigureShaders(GlInterface gl)
            {
                _shaderProgram = gl.CreateProgram();

                CreateVertexShader(gl);
                CreateFragmentShader(gl);

                // 链接着色器程序，并通过 GlInterface.UseProgram 激活
                Console.WriteLine(gl.LinkProgramAndGetError(_shaderProgram));
                gl.UseProgram(_shaderProgram);
            }

            void CreateFragmentShader(GlInterface gl)
            {
                // 采用 GlInterface.CreateShader 创建片段着色器，
                // 并利用 GlInterface.ComplieShaderAndGetError 将 GLSL 编译为着色器程序
                // 通过 GlInterface.AttachShader 将着色器附加到着色器程序 _shaderProgram 上
                _fragmentShader = gl.CreateShader(GL_FRAGMENT_SHADER);
                Console.WriteLine(gl.CompileShaderAndGetError(_fragmentShader, FragmentShaderSource));
                gl.AttachShader(_shaderProgram, _fragmentShader);
            }

            void CreateVertexShader(GlInterface gl)
            {
                // 采用 GlInterface.CreateShader 创建顶点着色器
                // 整个过程和片段着色器几乎相同，只有一些选项上的差异
                _vertexShader = gl.CreateShader(GL_VERTEX_SHADER);
                Console.WriteLine(gl.CompileShaderAndGetError(_vertexShader, VertexShaderSource));
                gl.AttachShader(_shaderProgram, _vertexShader);
            }

            protected override void OnOpenGlRender(GlInterface gl, int fb)
            {
                gl.ClearColor(0, 0, 0, 1);
                gl.Clear( GL_COLOR_BUFFER_BIT);

                // 通过 Viewport 进行视口变换，将标准化设备坐标变换为屏幕空间坐标
                gl.Viewport(0, 0, (int)Bounds.Width, (int)Bounds.Height);

                // 绘制图元
                gl.DrawArrays(GL_POINTS,    // 图元类型
                    0,                      // 顶点数组起始索引
                    new IntPtr(1));         // 绘制的顶点个数
                gl.CheckError();
            }

            void CreateVertexBuffer(GlInterface gl)
            {
                // 创建了一个三维的向量，表示顶点的位置，三个分量分别表示 X/Y/Z 方向上的“标准化设备坐标”
                // 关于标准化设备坐标的概念，参考：https://learnopengl-cn.github.io/01%20Getting%20started/04%20Hello%20Triangle/
                // 比如 Tutorial2 一开始是初始化为 Vector3(0,0,0)，那么将在画面正中位置显示一个点
                // 现在修改为 (0.5,0.5,0)，那么点显示的位置将处于右上方，且两侧比例为 3:1
                Vector3 vertex = new Vector3(0.5f, 0.5f, 0);

                // GlInterface 会提供一系列的 GenXX 的方法，用于构造相应对象。
                // 这里构建的是 VBO，也就是顶点缓冲对象，用于在显存中存储大量的顶点。
                _vbo = gl.GenBuffer();

                // 绑定缓冲类型，顶点缓冲对象的缓冲类型是 GL_ARRAY_BUFFER
                gl.BindBuffer(GL_ARRAY_BUFFER, _vbo);
                // 通过 BufferData 将定义好的顶点数据复制到缓冲的内存中
                // 第四个参数代表了显卡管理数据的策略，分为：
                // - GL_STATIC_DRAW，数据不变或几乎不变
                // - GL_DYNAMIC_DRAW，数据会改变很多
                // - GL_STREAM_DRAW，数据每次绘制时都会改变
                gl.BufferData(GL_ARRAY_BUFFER,      // 目标缓冲的类型，与顶点缓冲对象的类型保持一致
                    new IntPtr(sizeof(Vector3)),    // 指定传输数据的大小，以字节为单位，通常使用 sizeof 来获取
                    new IntPtr(&vertex),            // 希望发送的实际数据
                    GL_STATIC_DRAW);

                // 构建 VAO，也就是顶点缓冲数组，用于向 OpenGL 解释顶点数据与顶点着色器的对应关系
                _vao = gl.GenVertexArray();
                gl.BindVertexArray(_vao);

                gl.VertexAttribPointer(0,
                    3,                      // 顶点属性的大小，应该 sizeof(Vector3) 也是可行的？
                    GL_FLOAT,               // 指定数据的类型，GLSL 中的 Vec* 都是 float 类型
                    GL_FALSE,               // 是否希望数据被标准化
                    0,                      // 步长，Stride，连续顶点组之间的间隔
                    IntPtr.Zero);           // 位置数据在缓冲起始位置的偏移量
                gl.EnableVertexAttribArray(0);
            }

            // 顶点着色器，采用 GLSL 编写
            // 采用 in 关键字，声明 vec3 类型的 aPos 是一个输入变量
            // gl_Position 是顶点着色器中内置的输出变量，用于存储顶点处理后的位置
            // 需要注意的是，gl_Position 期望的是一个四维齐次坐标（第四个值与透视除法相关），所以采用 vec4 类型
            string VertexShaderSource => GlExtensions.GetShader(GlVersion, false, @" 
                in vec3 aPos;

                void main()
                {
                    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
                }
            ");

            // 片段着色器
            // 采用 out 关键字，声明 vec4 类型的 color 是一个输出变量
            // 使用 vec4 类型，是因为在片段着色器中，颜色是采用 RGBA 表示的
            string FragmentShaderSource => GlExtensions.GetShader(GlVersion, true, @"
                out vec4 color;
              
                void main()
                {
                    color = vec4(0, 1, 0.2, 0.7);
                } 
            ");

            int _vbo;
            int _vao;
            int _vertexShader;
            int _fragmentShader;
            int _shaderProgram;
        }
    }
}