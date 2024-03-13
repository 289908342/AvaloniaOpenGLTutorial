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

        // ���Ʋ�������
        // - �������㻺����� VBO
        // - �������㻺������ VAO
        // - ����������ɫ����Ƭ����ɫ��������󸽼ӵ�������
        // - ����ͼԪ
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

                // ������ɫ�����򣬲�ͨ�� GlInterface.UseProgram ����
                Console.WriteLine(gl.LinkProgramAndGetError(_shaderProgram));
                gl.UseProgram(_shaderProgram);
            }

            void CreateFragmentShader(GlInterface gl)
            {
                // ���� GlInterface.CreateShader ����Ƭ����ɫ����
                // ������ GlInterface.ComplieShaderAndGetError �� GLSL ����Ϊ��ɫ������
                // ͨ�� GlInterface.AttachShader ����ɫ�����ӵ���ɫ������ _shaderProgram ��
                _fragmentShader = gl.CreateShader(GL_FRAGMENT_SHADER);
                Console.WriteLine(gl.CompileShaderAndGetError(_fragmentShader, FragmentShaderSource));
                gl.AttachShader(_shaderProgram, _fragmentShader);
            }

            void CreateVertexShader(GlInterface gl)
            {
                // ���� GlInterface.CreateShader ����������ɫ��
                // �������̺�Ƭ����ɫ��������ͬ��ֻ��һЩѡ���ϵĲ���
                _vertexShader = gl.CreateShader(GL_VERTEX_SHADER);
                Console.WriteLine(gl.CompileShaderAndGetError(_vertexShader, VertexShaderSource));
                gl.AttachShader(_shaderProgram, _vertexShader);
            }

            protected override void OnOpenGlRender(GlInterface gl, int fb)
            {
                gl.ClearColor(0, 0, 0, 1);
                gl.Clear( GL_COLOR_BUFFER_BIT);

                // ͨ�� Viewport �����ӿڱ任������׼���豸����任Ϊ��Ļ�ռ�����
                gl.Viewport(0, 0, (int)Bounds.Width, (int)Bounds.Height);

                // ����ͼԪ
                gl.DrawArrays(GL_POINTS,    // ͼԪ����
                    0,                      // ����������ʼ����
                    new IntPtr(1));         // ���ƵĶ������
                gl.CheckError();
            }

            void CreateVertexBuffer(GlInterface gl)
            {
                // ������һ����ά����������ʾ�����λ�ã����������ֱ��ʾ X/Y/Z �����ϵġ���׼���豸���ꡱ
                // ���ڱ�׼���豸����ĸ���ο���https://learnopengl-cn.github.io/01%20Getting%20started/04%20Hello%20Triangle/
                // ���� Tutorial2 һ��ʼ�ǳ�ʼ��Ϊ Vector3(0,0,0)����ô���ڻ�������λ����ʾһ����
                // �����޸�Ϊ (0.5,0.5,0)����ô����ʾ��λ�ý��������Ϸ������������Ϊ 3:1
                Vector3 vertex = new Vector3(0.5f, 0.5f, 0);

                // GlInterface ���ṩһϵ�е� GenXX �ķ��������ڹ�����Ӧ����
                // ���ﹹ������ VBO��Ҳ���Ƕ��㻺������������Դ��д洢�����Ķ��㡣
                _vbo = gl.GenBuffer();

                // �󶨻������ͣ����㻺�����Ļ��������� GL_ARRAY_BUFFER
                gl.BindBuffer(GL_ARRAY_BUFFER, _vbo);
                // ͨ�� BufferData ������õĶ������ݸ��Ƶ�������ڴ���
                // ���ĸ������������Կ��������ݵĲ��ԣ���Ϊ��
                // - GL_STATIC_DRAW�����ݲ���򼸺�����
                // - GL_DYNAMIC_DRAW�����ݻ�ı�ܶ�
                // - GL_STREAM_DRAW������ÿ�λ���ʱ����ı�
                gl.BufferData(GL_ARRAY_BUFFER,      // Ŀ�껺������ͣ��붥�㻺���������ͱ���һ��
                    new IntPtr(sizeof(Vector3)),    // ָ���������ݵĴ�С�����ֽ�Ϊ��λ��ͨ��ʹ�� sizeof ����ȡ
                    new IntPtr(&vertex),            // ϣ�����͵�ʵ������
                    GL_STATIC_DRAW);

                // ���� VAO��Ҳ���Ƕ��㻺�����飬������ OpenGL ���Ͷ��������붥����ɫ���Ķ�Ӧ��ϵ
                _vao = gl.GenVertexArray();
                gl.BindVertexArray(_vao);

                gl.VertexAttribPointer(0,
                    3,                      // �������ԵĴ�С��Ӧ�� sizeof(Vector3) Ҳ�ǿ��еģ�
                    GL_FLOAT,               // ָ�����ݵ����ͣ�GLSL �е� Vec* ���� float ����
                    GL_FALSE,               // �Ƿ�ϣ�����ݱ���׼��
                    0,                      // ������Stride������������֮��ļ��
                    IntPtr.Zero);           // λ�������ڻ�����ʼλ�õ�ƫ����
                gl.EnableVertexAttribArray(0);
            }

            // ������ɫ�������� GLSL ��д
            // ���� in �ؼ��֣����� vec3 ���͵� aPos ��һ���������
            // gl_Position �Ƕ�����ɫ�������õ�������������ڴ洢���㴦����λ��
            // ��Ҫע����ǣ�gl_Position ��������һ����ά������꣨���ĸ�ֵ��͸�ӳ�����أ������Բ��� vec4 ����
            string VertexShaderSource => GlExtensions.GetShader(GlVersion, false, @" 
                in vec3 aPos;

                void main()
                {
                    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
                }
            ");

            // Ƭ����ɫ��
            // ���� out �ؼ��֣����� vec4 ���͵� color ��һ���������
            // ʹ�� vec4 ���ͣ�����Ϊ��Ƭ����ɫ���У���ɫ�ǲ��� RGBA ��ʾ��
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