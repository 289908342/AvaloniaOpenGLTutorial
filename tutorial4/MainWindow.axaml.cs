using System;
using System.Numerics;

using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using static Avalonia.OpenGL.GlConsts;
using static Common.GlConstExtensions;

namespace Tutorial4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MyOpenGlControl myControl = new MyOpenGlControl();
            Content = myControl;
        }

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

                Console.WriteLine(gl.LinkProgramAndGetError(_shaderProgram));

                gl.UseProgram(_shaderProgram);
            }

            void CreateFragmentShader(GlInterface gl)
            {
                _fragmentShader = gl.CreateShader(GL_FRAGMENT_SHADER);
                var s = gl.CompileShaderAndGetError(_fragmentShader, FragmentShaderSource);
                Console.WriteLine(s);
                gl.AttachShader(_shaderProgram, _fragmentShader);
            }

            void CreateVertexShader(GlInterface gl)
            {
                _vertexShader = gl.CreateShader(GL_VERTEX_SHADER);
                var s = gl.CompileShaderAndGetError(_vertexShader, VertexShaderSource);
                Console.WriteLine(s);
                gl.AttachShader(_shaderProgram, _vertexShader);
            }

            protected override void OnOpenGlRender(GlInterface gl, int fb)
            {
                gl.ClearColor(0, 0, 0, 1);
                gl.Clear(GL_COLOR_BUFFER_BIT);

                gl.Viewport(0, 0, (int)Bounds.Width, (int)Bounds.Height);

                gl.DrawArrays(GL_TRIANGLES, 0, new IntPtr(3));
                gl.CheckError();
            }

            void CreateVertexBuffer(GlInterface gl)
            {
                Vector3[] vertices = new Vector3[]
                {
                    new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(1f, 0f, 0f),
                    new Vector3(0.5f, -0.5f, 0.0f), new Vector3(0f, 1f, 0f),
                    new Vector3(0.0f, 0.5f, 0.0f), new Vector3(0f, 0f, 1f),
                };

                _vbo = gl.GenBuffer();
                gl.BindBuffer(GL_ARRAY_BUFFER, _vbo);

                fixed (void* pVertices = vertices)
                    gl.BufferData(GL_ARRAY_BUFFER, new IntPtr(sizeof(Vector3) * vertices.Length),
                    new IntPtr(pVertices), GL_STATIC_DRAW);

                _vao = gl.GenVertexArray();
                gl.BindVertexArray(_vao);

                gl.VertexAttribPointer(
                    0, 3, GL_FLOAT, GL_FALSE, 2 * sizeof(Vector3), IntPtr.Zero);
                gl.EnableVertexAttribArray(0);
                gl.VertexAttribPointer(
                    1, 3, GL_FLOAT, GL_FALSE, 2 * sizeof(Vector3), new IntPtr(sizeof(Vector3)));
                gl.EnableVertexAttribArray(1);
            }

            // HELPME: �ο��� https://learnopengl-cn.github.io/01%20Getting%20started/05%20Shaders/#_5
            // ������ #Version �� layout �﷨����ɫ������ʱ�����б������� layout��������ϢΪ��"ERROR: 0:3: 'location' : syntax error syntax error\n\n"
            string VertexShaderSource => GlExtensions.GetShader(GlVersion, false, @"
                //layout(location = 0) in vec3 Position;
                //layout(location = 1) in vec3 fragColor;

                in vec3 Position;
                out vec3 myColor;

                void main()
                {
                    gl_Position = vec4(Position, 1.0);
                    myColor = vec3(0,0,1);
                }
            ");
            string FragmentShaderSource => GlExtensions.GetShader(GlVersion, true, @"
                out vec4 FragColor;
                in vec3 myColor;

                void main()
                {
                    FragColor = vec4(myColor, 1.0);
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