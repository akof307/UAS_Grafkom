using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pertemuan1
{
    internal class Asset2d
    {
        float[] _vertices =
        {
        };
        uint[] _indices =
        {  
        };
        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject;
        Shader _shader;
        int indexs;
        int[] _pascal = { };
        public Asset2d(float[] vertices, uint[] indices)
        {
            _vertices = vertices;
            _indices = indices;
        }
        public Asset2d()
        {
            _vertices = new float[1080];
            indexs = 0;
        }
        public void load(string shaderVert, string shaderFrag)
        {
            //Buffer
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            
            //VAO
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            //kalau mau bikin object settingannya beda dikasih if
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //0 == referensi dari atas
            GL.EnableVertexAttribArray(0);
            //ada data yanh disimpan di _indices
            if(_indices.Length != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            }
            //"shader.vert",
            //   "C:/Users/ASUS/Documents/UKPetra/Semester4/Grafkom/Pertemuan1/Shaders/shader.frag"
            _shader = new Shader(shaderVert, shaderFrag);
            _shader.Use();
        }

        public void render(float[] _assetColor, int _lines)
        {
            _shader.Use();

            int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "ourColor");
            GL.Uniform4(vertexColorLocation, _assetColor[0], _assetColor[1], _assetColor[2], _assetColor[3]);
            GL.BindVertexArray(_vertexArrayObject);
            if(_indices.Length != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                if (_lines == 0)
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                else if (_lines == 1)
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, (_vertices.Length + 1) / 3);
                else if (_lines == 2)
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, indexs);
                else if (_lines == 3)
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, (_vertices.Length + 1) / 3);
                else if (_lines == 4)
                    GL.DrawArrays(PrimitiveType.Lines, 0, 3);
            }

        }
        public void createCircle(float centerX, float centerY, float radius)
        {
            _vertices = new float[1080];
            for(int i = 0; i < 360; i++)
            {
                double degInRad = i * Math.PI / 180;
                _vertices[i * 3] = radius * (float)Math.Cos(degInRad) + centerX;
                _vertices[i*3+1] = radius * (float)Math.Sin(degInRad) + centerY;
                _vertices[i * 3 + 2] = 0;
            }
        }
        public void createHand(string typeH, float centerX, float centerY, float lengthH)
        {
            _vertices = new float[6];
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;
            if (hour > 12)
                hour -= 12;
            for (int i = 0; i < 6; i++)
            {
                _vertices[i] = 0;
            }
            if (typeH == "hour")
            {
                _vertices[3] = lengthH * (float)Math.Sin((hour + minute / 60) * 30 * Math.PI / 180) + centerX;
                _vertices[4] = lengthH * (float)Math.Cos((hour = minute / 60) * 30 * Math.PI / 180) + centerY;
            }
            else if (typeH == "minute")
            {
                _vertices[3] = lengthH * (float)Math.Sin((minute + second / 60) * 6 * Math.PI / 180) + centerX;
                _vertices[4] = lengthH * (float)Math.Cos((minute + second / 60) * 6 * Math.PI / 180) + centerY;
            }
            else if (typeH == "second")
            {
                _vertices[3] = lengthH * (float)Math.Sin(second * 6 * Math.PI / 180);
                _vertices[4] = lengthH * (float)Math.Cos(second * 6 * Math.PI / 180);
            }

            //GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            load("../../../Shaders/shader.vert", "../../../Shaders/shader.frag");
        }

        public void updateMousePosition(float _x, float _y, float _z)
        {
            _vertices[indexs * 3] = _x;
            _vertices[indexs * 3 + 1] = _y;
            _vertices[indexs * 3 + 2] = _z;
            indexs++;

            GL.BufferData(BufferTarget.ArrayBuffer, indexs * 3 * sizeof(float),
                _vertices, BufferUsageHint.StaticDraw);
            //settingan VAO
            //_vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
                3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
        public List<float> CreateCurveBezier()
        {
            List<float> _vertices_bezier = new List<float>();
            List<int> pascal = getRow(indexs - 1);
            _pascal = pascal.ToArray();
            for (float t = 0; t <= 1.0f; t += 0.01f)
            {
                Vector2 p = getP(indexs, t);
                _vertices_bezier.Add(p.X);
                _vertices_bezier.Add(p.Y);
                _vertices_bezier.Add(0);
            }
            return _vertices_bezier;
        }
        public Vector2 getP(int n, float t)
        {
            Vector2 p = new Vector2(0, 0);
            float[] k = new float[n];

            for (int i = 0; i < n; i++)
            {
                k[i] = (float)Math.Pow((1 - t), n - 1 - i) * (float)Math.Pow(t, i) * _pascal[i];
            }
            for (int i = 0; i < n; i++)
            {
                p.X += k[i] * _vertices[i * 3];
                p.Y += k[i] * _vertices[i * 3 + 1];
            }

            return p;
        }
        public List<int> getRow(int rowIndex)
        {
            List<int> currow = new List<int>();

            //element 1 dari pascal
            currow.Add(1);

            if (rowIndex == 0)
            {
                return currow;
            }
            List<int> prev = getRow(rowIndex - 1);
            //nambah element pascal yg ditengah
            for (int i = 1; i < prev.Count; i++)
            {
                int curr = prev[i - 1] + prev[i];
                currow.Add(curr);
            }
            //nambah element yg terakhir
            currow.Add(1);

            return currow;
        }

        public void setVertices(float[] vertices)
        {
            _vertices = vertices;
        }
        public bool getVerticesLength()
        {
            if (_vertices[0] == 0)
            {
                return false;
            }
            if ((_vertices.Length + 1) / 3 > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
