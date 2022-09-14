using System;
using System.Numerics;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Common;
using System.Numerics;
using static Avalonia.OpenGL.GlConsts;
using static Common.GlConstExtensions;

namespace Tutorial13
{
    unsafe class OpenGlControl : OpenGlControlBase
    {
        protected override void OnOpenGlInit(GlInterface gl, int fb)
        {
            base.OnOpenGlInit(gl, fb);

            ConfigureShaders(gl);
            CreateVertexBuffer(gl);
            CreateIndexBuffer(gl);
            ConfigureCamera();

            gl.CheckError();
        }

        void ConfigureCamera()
        {
            Vector3 cameraPos = new Vector3(1.0f, 1.0f, -3.0f);
            Vector3 cameraTarget = new Vector3(0.45f, 0.0f, 1.0f);
            Vector3 cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            _operations.SetCamera(cameraPos, cameraTarget, cameraUp);
        }

        protected override void OnOpenGlDeinit(GlInterface gl, int fb)
        {
            base.OnOpenGlDeinit(gl, fb);

            gl.BindBuffer(GL_ARRAY_BUFFER, 0);
            gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
            gl.BindVertexArray(0);
            gl.UseProgram(0);

            gl.DeleteBuffer(_ibo);
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

            _gTransformLoc = gl.GetUniformLocationString(_shaderProgram, "gTransform");
        }

        void CreateFragmentShader(GlInterface gl)
        {
            _fragmentShader = gl.CreateShader(GL_FRAGMENT_SHADER);
            Console.WriteLine(gl.CompileShaderAndGetError(_fragmentShader, VertexFragmentShaderSource));
            gl.AttachShader(_shaderProgram, _fragmentShader);
        }

        void CreateVertexShader(GlInterface gl)
        {
            _vertexShader = gl.CreateShader(GL_VERTEX_SHADER);
            Console.WriteLine(gl.CompileShaderAndGetError(_vertexShader, VertexShaderSource));
            gl.AttachShader(_shaderProgram, _vertexShader);
        }

        void CreateIndexBuffer(GlInterface gl)
        {
            _indices = new ushort[]
            {
                0, 1, 3,
                3, 1, 2,
                4, 5, 7,
                7, 5, 6,
                8, 9, 11,
                11, 9, 10,
                12, 13, 15,
                15, 13, 14,
                16, 17, 19,
                19, 17, 18,
                20, 21, 23,
                23, 21, 22,
            };

            _ibo = gl.GenBuffer();
            gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, _ibo);

            fixed (void* pIndicesData = _indices)
                gl.BufferData(GL_ELEMENT_ARRAY_BUFFER, new IntPtr(sizeof(ushort) * _indices.Length),
                    new IntPtr(pIndicesData), GL_STATIC_DRAW);
        }

        protected override void OnOpenGlRender(GlInterface gl, int fb)
        {
            _scale += 0.005f;

            gl.ClearColor(0, 0, 0, 1);
            gl.Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            gl.Enable(GL_DEPTH_TEST);

            gl.Viewport(0, 0, (int)Bounds.Width, (int)Bounds.Height);

            _operations.SetPerspective(_fieldOfView_rad, (float)Bounds.Width, (float)Bounds.Height, _nearPlane,
                _farPlane);
            _operations.Scale(_scaleX, _scaleY, _scaleZ);
            _operations.Position(_translateX, _translateY, _translateZ);
            _operations.Rotate(_rotateX, _rotateY, _rotateZ);

            Matrix4x4 transformation = _operations.GetTransformation();
            gl.UniformMatrix4fv(_gTransformLoc, 1, false, &transformation);

            gl.DrawElements(GL_TRIANGLES, _indices!.Length, GL_UNSIGNED_SHORT, IntPtr.Zero);
            gl.CheckError();

            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
        }

        void CreateVertexBuffer(GlInterface gl)
        {
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
            };

            _vbo = gl.GenBuffer();
            gl.BindBuffer(GL_ARRAY_BUFFER, _vbo);

            fixed (void* pVertices = vertices)
                gl.BufferData(GL_ARRAY_BUFFER, new IntPtr(sizeof(Vector3) * vertices.Length),
                    new IntPtr(pVertices), GL_STATIC_DRAW);

            _vao = gl.GenVertexArray();
            gl.BindVertexArray(_vao);

            gl.VertexAttribPointer(
                0, 3, GL_FLOAT, GL_FALSE, sizeof(Vector3), IntPtr.Zero);
            gl.EnableVertexAttribArray(0);
        }


        string VertexShaderSource => GlExtensions.GetShader(GlVersion, false, @" 
                in vec3 position;
                uniform mat4 gTransform;
                out vec3 vertPos;

                void main()
                {
                    vertPos = position;
                    gl_Position = gTransform * vec4(position, 1.0);
                }
            ");

        string VertexFragmentShaderSource => GlExtensions.GetShader(GlVersion, true, @"
                in vec3 vertPos;
                out vec4 fragColor;

                void main()
                {
                        vec3 posAbs  = abs(vertPos);
                        vec3 color = step(posAbs.yzx, posAbs) * step(posAbs.zxy, posAbs); 
                        color += (1.0 - step(color.zxy * vertPos.zxy, vec3(0.0)));

                        fragColor = vec4(color, 1.0);
                }
            ");

        int _vbo;
        int _vao;
        int _ibo;
        int _vertexShader;
        int _fragmentShader;
        int _shaderProgram;
        int _gTransformLoc;

        float _scale = 0.5f;
        ushort[]? _indices;
        readonly Pipeline _operations = new Pipeline();

        float _scaleX;
        float _scaleY;
        float _scaleZ;
        float _translateX;
        float _translateY;
        float _translateZ;
        float _rotateX;
        float _rotateY;
        float _rotateZ;
        float _fieldOfView_rad = 0.1f;
        float _nearPlane = 0;
        float _farPlane = 1;

        public void SetScaleX(double v) => _scaleX = (float)v;
        public void SetScaleY(double v) => _scaleY = (float)v;
        public void SetScaleZ(double v) => _scaleZ = (float)v;
        public void SetTranslateX(double v) => _translateX = (float)v;
        public void SetTranslateY(double v) => _translateY = (float)v;
        public void SetTranslateZ(double v) => _translateZ = (float)v;
        public void SetRotateX(double v) => _rotateX = (float)v;
        public void SetRotateY(double v) => _rotateY = (float)v;
        public void SetRotateZ(double v) => _rotateZ = (float)v;
        public void SetFieldOfView(double v) => _fieldOfView_rad = (float)v;
        public void SetNearPlane(double v) => _nearPlane = (float)v;
        public void SetFarPlane(double v) => _farPlane = (float)v;
    }
}