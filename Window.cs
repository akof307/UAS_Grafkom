using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;



namespace Pertemuan1
{
    static class Constants
    {
        public const string path = "../../../Shaders/";
        public const string obj = "../../../";
    }
    internal class Window : GameWindow
    {
        private readonly List<Vector3> _pointLightPositions = new List<Vector3>()
        {
            new Vector3(-5.0f, 4.354f, 1.73988f), //1
            new Vector3(-5.0f, 4.354f, -2.59259f), //2
            new Vector3(-5.0f, 4.354f, -0.989761f), //3
            new Vector3(-5.0f, 4.354f, 0.273065f),//4
            new Vector3(-3.5f, 4.354f, -2.58492f),//5
            new Vector3(-3.5f, 4.354f, -0.989761f),//6
            new Vector3(-3.5f, 4.354f, 0.273065f),//7
            new Vector3(-3.5f, 4.354f, 1.73988f),//8
            new Vector3(-2.0f, 4.354f, 1.73988f),//9
            new Vector3(-2.0f, 4.354f, 0.273065f),//10
            new Vector3(-2.0f, 4.354f, -0.989761f),//11
            new Vector3(-2.0f, 4.354f, -2.58492f),//12
            new Vector3(-0.5f, 4.354f, 1.73988f),//13
            new Vector3(-0.5f, 4.354f, 0.273065f),//14
            new Vector3(-0.5f, 4.354f, -0.989761f),//15
            new Vector3(-0.5f, 4.354f, -2.58492f),//16
            new Vector3(1.0f, 4.354f, 1.73988f),//17
            new Vector3(1.0f, 4.354f, 0.273065f),//18
            new Vector3(1.0f, 4.354f, -0.989761f),//19
            new Vector3(1.0f, 4.354f, -2.58492f),//20
            new Vector3(-0.680973f, 0.254018f, 0.581485f),//21
            new Vector3(-3.70541f, -1.25754f, 0.581485f),//22
            new Vector3(-5.89687f, 0.969758f, 1.85081f)//23

        };
        private readonly List<Vector3> point_light_color_difuse = new List<Vector3>()
        {
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.0f, 1f, 0f),
            new Vector3(0.0f, 1f, 0f),
            new Vector3(0.0f, 1f, 0f)
        };
        public class Material
        {
            public string name;
            public Vector3 ambient;
            public Vector3 diffuse;
            public Vector3 specular;
            public List<Vector3> getDiffSpec(List<Material> materials, string search)
            {
                List<Vector3> done = new List<Vector3>();
                foreach (var material in materials)
                {
                    if (string.Equals(material.name, search))
                    {
                        done.Add(material.ambient);
                        done.Add(material.diffuse);
                        done.Add(material.specular);
                        break;
                    }
                }
                return done;
            }
        }
        private List<Material> materials = new List<Material>();
        private void readFileMTL(List<Material> mtls, string pathfilename)
        {
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ".";
            
            Material material = new Material();
            foreach (string line in File.ReadLines(pathfilename))
            {
                string[] words = line.Split(' ');
                if (String.Equals(words[0], "newmtl"))
                {
                    mtls.Add(material);
                    material = new Material();
                    material.name = words[1];
                }
                else if (String.Equals(words[0], "Ka"))
                {
                    material.ambient.X = float.Parse(words[1], ci);
                    material.ambient.Y = float.Parse(words[2], ci);
                    material.ambient.Z = float.Parse(words[3], ci);
                }
                else if (String.Equals(words[0], "Kd"))
                {
                    material.diffuse.X = float.Parse(words[1], ci);
                    material.diffuse.Y = float.Parse(words[2], ci);
                    material.diffuse.Z = float.Parse(words[3], ci);
                }
                else if (String.Equals(words[0], "Ks"))
                {
                    material.specular.X = float.Parse(words[1], ci);
                    material.specular.Y = float.Parse(words[2], ci);
                    material.specular.Z = float.Parse(words[3], ci);
                }
            }
            mtls.Add(material);
        }
        private void readFileOBJ(Asset3d parent, string pathfilename, List<Material> mtls, float scale)
        {
            List<Vector3> _tempVertices = new List<Vector3>();
            List<Vector3> _normVertices = new List<Vector3>();
            List<Vector3> _vertParse = new List<Vector3>();
            
            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.NumberDecimalSeparator = ".";
            Material material = new Material();
            Asset3d children = new Asset3d();
            foreach (string line in File.ReadLines(pathfilename))
            {
                string[] words = line.Split(' ');
                if (String.Equals(words[0], "o"))
                {
                    children.setVertices(_vertParse);
                    parent.addChild(children);
                    children = new Asset3d();
                    children._material = "";
                    _vertParse = new List<Vector3>();
                }
                else if (String.Equals(words[0], "v"))
                {
                    Vector3 vertice;
                    vertice.X = float.Parse(words[1], ci);
                    vertice.Y = float.Parse(words[2], ci);
                    vertice.Z = float.Parse(words[3], ci);
                    _tempVertices.Add(vertice);
                }
                //else if (String.Equals(words[0], "vt"))
                //{
                //    Vector2 texture;
                //    texture.X = float.Parse(words[1], ci);
                //    texture.Y = float.Parse(words[2], ci);
                //    _verticesTexture.Add(texture);
                //}
                else if (String.Equals(words[0], "vn"))
                {
                    Vector3 normalVertice;
                    normalVertice.X = float.Parse(words[1], ci);
                    normalVertice.Y = float.Parse(words[2], ci);
                    normalVertice.Z = float.Parse(words[3], ci);
                    _normVertices.Add(normalVertice);
                }
                else if (String.Equals(words[0], "usemtl"))
                {
                    children._mtlDiffSpec = material.getDiffSpec(mtls, words[1]);
                }
                else if (String.Equals(words[0], "f"))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string[] subword = words[i + 1].Split('/');
                        int indexVert = int.Parse(subword[0], ci);
                        int indexNorm = int.Parse(subword[2], ci);
                        _vertParse.Add(_tempVertices[indexVert - 1] * new Vector3(scale));
                        _vertParse.Add(_normVertices[indexNorm - 1] * new Vector3(scale));
                    }

                }
            }
            children.setVertices(_vertParse);
            parent.addChild(children);
        }
        private readonly List<Vector3> _spotPos = new List<Vector3>()
        {
            new Vector3(0, 3, 0),
            new Vector3(3, 0, 0),
            new Vector3(0, 0, 3),
        };
        private readonly List<Vector3> _spotDir = new List<Vector3>()
        {
            new Vector3(0,-1,0),
            new Vector3(-1,0,0),
            new Vector3(0,0,-1)
        };
        private readonly List<Vector3> _spotDiff = new List<Vector3>()
        {
            new Vector3(0.788235f, 0.101961f, 0.035294f),
            new Vector3(0.788235f, 0.101961f, 0.035294f),
            new Vector3(0.788235f, 0.101961f, 0.035294f)
        };
        Asset3d[] _object3d = new Asset3d[2];
        double _time;
        float degr = 0;
        Camera _camera;
        bool _firstMove = true;
        Vector2 _lastPos;
        Vector3 _objecPost = new Vector3(0.0f, 0.0f, 0.0f);
        float _rotationSpeed = 1f;
        Asset3d[] cahaya = new Asset3d[21];

        Asset3d blender = new Asset3d();
        bool _screenOn = true;
        Asset3d CharacterObject = new Asset3d();
        float charMargin = 0.1f;
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }
        public Matrix4 generateArbRotationMatrix(Vector3 axis, Vector3 center, float degree)
        {
            var rads = MathHelper.DegreesToRadians(degree);

            var secretFormula = new float[4, 4] {
                { (float)Math.Cos(rads) + (float)Math.Pow(axis.X, 2) * (1 - (float)Math.Cos(rads)), axis.X* axis.Y * (1 - (float)Math.Cos(rads)) - axis.Z * (float)Math.Sin(rads),    axis.X * axis.Z * (1 - (float)Math.Cos(rads)) + axis.Y * (float)Math.Sin(rads),   0 },
                { axis.Y * axis.X * (1 - (float)Math.Cos(rads)) + axis.Z * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Y, 2) * (1 - (float)Math.Cos(rads)), axis.Y * axis.Z * (1 - (float)Math.Cos(rads)) - axis.X * (float)Math.Sin(rads),   0 },
                { axis.Z * axis.X * (1 - (float)Math.Cos(rads)) - axis.Y * (float)Math.Sin(rads),   axis.Z * axis.Y * (1 - (float)Math.Cos(rads)) + axis.X * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Z, 2) * (1 - (float)Math.Cos(rads)), 0 },
                { 0, 0, 0, 1}
            };
            var secretFormulaMatix = new Matrix4
            (
                new Vector4(secretFormula[0, 0], secretFormula[0, 1], secretFormula[0, 2], secretFormula[0, 3]),
                new Vector4(secretFormula[1, 0], secretFormula[1, 1], secretFormula[1, 2], secretFormula[1, 3]),
                new Vector4(secretFormula[2, 0], secretFormula[2, 1], secretFormula[2, 2], secretFormula[2, 3]),
                new Vector4(secretFormula[3, 0], secretFormula[3, 1], secretFormula[3, 2], secretFormula[3, 3])
            );

            return secretFormulaMatix;
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            //ganti background
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            cahaya[0] = new Asset3d();
            cahaya[1] = new Asset3d();
            cahaya[2] = new Asset3d();
            cahaya[3] = new Asset3d();
            cahaya[4] = new Asset3d();
            cahaya[5] = new Asset3d();
            cahaya[6] = new Asset3d();
            cahaya[7] = new Asset3d();
            cahaya[8] = new Asset3d();
            cahaya[9] = new Asset3d();
            cahaya[10] = new Asset3d();
            cahaya[11] = new Asset3d();
            cahaya[12] = new Asset3d();
            cahaya[13] = new Asset3d();
            cahaya[14] = new Asset3d();
            cahaya[15] = new Asset3d();
            cahaya[16] = new Asset3d();
            cahaya[17] = new Asset3d();
            cahaya[18] = new Asset3d();
            cahaya[19] = new Asset3d();
            cahaya[20] = new Asset3d();


            //cahaya[0].createBoxVertices(-5.0f, 4.354f, -2.59259f, 0.3f);
            cahaya[0].createEllipsoid(0.07f, 0.07f, 0.07f, -5.0f, 4.354f, 1.73988f);
            cahaya[0].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[0].updateCameraMap();

            cahaya[1].createEllipsoid(0.07f, 0.07f, 0.07f, -5.0f, 4.354f, -2.59259f);
            cahaya[1].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[1].updateCameraMap();

            cahaya[2].createEllipsoid(0.07f, 0.07f, 0.07f, -5.0f, 4.354f, -0.989761f);
            cahaya[2].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[2].updateCameraMap();

            cahaya[3].createEllipsoid(0.07f, 0.07f, 0.07f, -5.0f, 4.354f, 0.273065f);
            cahaya[3].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[3].updateCameraMap();

            cahaya[4].createEllipsoid(0.07f, 0.07f, 0.07f, -3.5f, 4.354f, -2.58492f);
            cahaya[4].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[4].updateCameraMap();

            cahaya[5].createEllipsoid(0.07f, 0.07f, 0.07f, -3.5f, 4.354f, -0.989761f);
            cahaya[5].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[5].updateCameraMap();

            cahaya[6].createEllipsoid(0.07f, 0.07f, 0.07f, -3.5f, 4.354f, 0.273065f);
            cahaya[6].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[6].updateCameraMap();

            cahaya[7].createEllipsoid(0.07f, 0.07f, 0.07f, -3.5f, 4.354f, 1.73988f);
            cahaya[7].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[7].updateCameraMap();

            cahaya[8].createEllipsoid(0.07f, 0.07f, 0.07f, -2.0f, 4.354f, 1.73988f);
            cahaya[8].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[8].updateCameraMap();

            cahaya[9].createEllipsoid(0.07f, 0.07f, 0.07f, -2.0f, 4.354f, 0.273065f);
            cahaya[9].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[9].updateCameraMap();

            cahaya[10].createEllipsoid(0.07f, 0.07f, 0.07f, -2.0f, 4.354f, -0.989761f);
            cahaya[10].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[10].updateCameraMap();

            cahaya[11].createEllipsoid(0.07f, 0.07f, 0.07f, -2.0f, 4.354f, -2.58492f);
            cahaya[11].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[11].updateCameraMap();

            cahaya[12].createEllipsoid(0.07f, 0.07f, 0.07f, -0.5f, 4.354f, 1.73988f);
            cahaya[12].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[12].updateCameraMap();

            cahaya[13].createEllipsoid(0.07f, 0.07f, 0.07f, -0.5f, 4.354f, 0.273065f);
            cahaya[13].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[13].updateCameraMap();

            cahaya[14].createEllipsoid(0.07f, 0.07f, 0.07f, -0.5f, 4.354f, -0.989761f);
            cahaya[14].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[14].updateCameraMap();

            cahaya[15].createEllipsoid(0.07f, 0.07f, 0.07f, -0.5f, 4.354f, -2.58492f);
            cahaya[15].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[15].updateCameraMap();

            cahaya[16].createEllipsoid(0.07f, 0.07f, 0.07f, 1.0f, 4.354f, 1.73988f);
            cahaya[16].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[16].updateCameraMap();

            cahaya[17].createEllipsoid(0.07f, 0.07f, 0.07f, 1.0f, 4.354f, 0.273065f);
            cahaya[17].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[17].updateCameraMap();

            cahaya[18].createEllipsoid(0.07f, 0.07f, 0.07f, 1.0f, 4.354f, -0.989761f);
            cahaya[18].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[18].updateCameraMap();

            cahaya[19].createEllipsoid(0.07f, 0.07f, 0.07f, 1.0f, 4.354f, -2.58492f);
            cahaya[19].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[19].updateCameraMap();

            cahaya[20].createBoxVertices(-2.6f, 1.04713f, 1.5f, 0.3f);
            cahaya[20].loadLamp(Constants.path + "shader.vert", Constants.path + "shader.frag", Size.X, Size.Y);
            cahaya[20].updateCameraMap();

            _object3d[0] = new Asset3d();
            //_object3d[0].createBoxVertices2(0.0f, 0.0f, 0, 0.5f);
            _pointLightPositions.Add(cahaya[0]._centerPosition);
            point_light_color_difuse.Add(new Vector3(0.0f, 0, 0));
            //_object3d[0].createEllipsoid2(0.2f, 0.2f, 0.2f, 0.0f, 0.0f, 0.0f, 72, 24);
            //_object3d[0].createEllipsoid(0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 0.0f);
            _object3d[0].load(Constants.path + "shader.vert", Constants.path + "objectshader.frag", Size.X, Size.Y);
            _object3d[0].updateCameraMap();

            _camera = new Camera(new Vector3(0, 1, 1), Size.X / Size.Y);

            readFileMTL(materials, Constants.obj + "character.mtl");
            readFileOBJ(CharacterObject, Constants.obj + "character.obj", materials, 0.02f);
            CharacterObject.rotate(CharacterObject._centerPosition, new Vector3(0, 1, 0), -90);
            CharacterObject.translate(new Vector3(_camera.Position[0] + _camera.Front[0] * charMargin, _camera.Position[1] + _camera.Front[1] * charMargin, _camera.Position[2] + _camera.Front[2] * charMargin));
            CharacterObject.load(Constants.path + "objectshader.vert", Constants.path + "objectshader.frag", Size.X, Size.Y);

            readFileMTL(materials, Constants.obj + "unaatitled.mtl");
            readFileOBJ(blender, Constants.obj + "unaatitled.obj", materials, 0.1f);
            blender.load(Constants.path + "objectshader.vert", Constants.path + "objectshader.frag", Size.X, Size.Y);
            blender.updateCameraMap();
            CursorGrabbed = true;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            //
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //_time += 9.0 * args.Time;
            Matrix4 temp = Matrix4.Identity;
            //temp = temp * Matrix4.CreateTranslation(0.5f, 0.5f, 0.0f);
            //degr += MathHelper.DegreesToRadians(20f);
            //temp = temp * Matrix4.CreateRotationX(degr);
            //_object3d[0].rotatede(_object3d[0]._centerPosition, _object3d[0]._euler[1], 0);

            //_object3d[0].render(0,_time,temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[0].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[1].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[2].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[3].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[4].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[5].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[6].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[7].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[8].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[9].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[10].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[11].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[12].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[13].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[14].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[15].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[16].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[17].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[18].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            cahaya[19].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            //cahaya[20].render(1, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            //cahaya[0].rotatede(_object3d[0]._centerPosition, _object3d[0]._euler[1], (float)args.Time * 90);
            //_pointLightPositions[4] = cahaya[0]._centerPosition;
            //_object3d[0].setFragVariable(new Vector3(0.1f, 0.5f, 0.5f), new Vector3(0,1,.8f), cahaya[0]._centerPosition, _camera.Position);

            //for (int i = 0; i < 1; i++)
            //{
            //    _object3d[i].render(0, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            //    _object3d[i].setFragVariable(new Vector3(1.0f, 0.5f, 0.31f), _camera.Position);
            //    _object3d[i].setDirectionalLight(new Vector3(0.0f, -1, 0.0f), new Vector3(0.1f), new Vector3(1), new Vector3(0.5f));
            //    //_object3d[i].setPointLight(cahaya[0]._centerPosition, new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f);
            //    //_object3d[i].setSpotLight(_camera.Position, _camera.Front, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f),
            //    //1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
            //    _object3d[i].setSpotLight(_camera.Position, _camera.Front, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f),
            //    1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
            //    _object3d[i].setPointLights(_pointLightPositions, new Vector3(0.05f, 0.05f, 0.05f), point_light_color_difuse, new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f);
            //}
            blender.render(0, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            //blender.setLightColor(new Vector3(1f, 0.1f, 0.1f));
            blender.setFragVariable(new Vector3(0.1f, 0.5f, 0.31f), _camera.Position); //ganti warna object
            //blender.setDirectionalLight(new Vector3(0.0f, 0, 0.0f), new Vector3(0.1f), new Vector3(1), new Vector3(0.5f));
            //_object3d[i].setPointLight(cahaya[0]._centerPosition, new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.8f, 0.8f, 0.8f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, 0.09f, 0.032f);
            blender.setSpotLight(new Vector3(-5.0f, 1.04713f, 1.7f), new Vector3(0, -1, 0), new Vector3(0), new Vector3(1f), new Vector3(1.0f),
            1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(15f)), MathF.Cos(MathHelper.DegreesToRadians(15f)));
            //blender.setSpotLights(_spotPos, _spotDir, new Vector3(0), _spotDiff, new Vector3(1.0f, 1.0f, 1.0f),
            //1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(12.5f)), MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
            blender.setPointLights(_pointLightPositions, new Vector3(0.1f), point_light_color_difuse, new Vector3(1.0f), 1.0f, 0.9f, 0.032f);

            CharacterObject.render(0, _time, temp, _camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            CharacterObject.setFragVariable(new Vector3(0.1f, 0.5f, 0.31f), _camera.Position); //ganti warna object
            //CharacterObject.setDirectionalLight(new Vector3(0.0f, 1, 0.0f), new Vector3(0.1f), new Vector3(1), new Vector3(0.5f));
            CharacterObject.setSpotLight(new Vector3(-5.0f, 1.04713f, 1.7f), new Vector3(0, -1, 0), new Vector3(0), new Vector3(1f), new Vector3(1.0f),
            1.0f, 0.09f, 0.032f, MathF.Cos(MathHelper.DegreesToRadians(15f)), MathF.Cos(MathHelper.DegreesToRadians(15f)));
            CharacterObject.setPointLights(_pointLightPositions, new Vector3(0.1f), point_light_color_difuse, new Vector3(1.0f), 1.0f, 0.9f, 0.032f);

            CharacterObject.resetEuler();

            SwapBuffers();

        }

        Keys lastKey = Keys.W;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 1f;
            const float sensitivity = 0.5f;

            if (input.IsKeyDown(Keys.W))
            {
                float Xv = _camera.Front[0];
                float Yv = _camera.Front[1];
                float Zv = _camera.Front[2];
                if (lastKey == Keys.S) { character_angle_late_X -= 180; lastKey = Keys.W; }
                if (lastKey == Keys.D) { character_angle_late_X -= 90; lastKey = Keys.W; }
                if (lastKey == Keys.A) { character_angle_late_X += 90; lastKey = Keys.W; }
                MoveCam((float)e.Time, Xv, Yv, Zv, cameraSpeed);
            }
            if (input.IsKeyDown(Keys.S))
            {
                float Xv = -_camera.Front[0];
                float Yv = -_camera.Front[1];
                float Zv = -_camera.Front[2];
                if (lastKey == Keys.W) { character_angle_late_X += 180; lastKey = Keys.S; }
                if (lastKey == Keys.A) { character_angle_late_X -= 90; lastKey = Keys.S; }
                if (lastKey == Keys.D) { character_angle_late_X += 90; lastKey = Keys.S; }
                MoveCam((float)e.Time, Xv, Yv, Zv, cameraSpeed);
            }
            if (input.IsKeyDown(Keys.A))
            {
                float Xv = -_camera.Right[0];
                float Yv = -_camera.Right[1];
                float Zv = -_camera.Right[2];
                if (lastKey == Keys.W) { character_angle_late_X -= 90; lastKey = Keys.A; }
                if (lastKey == Keys.D) { character_angle_late_X -= 180; lastKey = Keys.A; }
                if (lastKey == Keys.S) { character_angle_late_X += 90; lastKey = Keys.A; }
                MoveCam((float)e.Time, Xv, Yv, Zv, cameraSpeed);
            }
            if (input.IsKeyDown(Keys.D))
            {
                float Xv = _camera.Right[0];
                float Yv = _camera.Right[1];
                float Zv = _camera.Right[2];
                if (lastKey == Keys.W) { character_angle_late_X += 90; lastKey = Keys.D; }
                if (lastKey == Keys.A) { character_angle_late_X += 180; lastKey = Keys.D; }
                if (lastKey == Keys.S) { character_angle_late_X -= 90; lastKey = Keys.D; }
                MoveCam((float)e.Time, Xv, Yv, Zv, cameraSpeed);
            }
            if (input.IsKeyDown(Keys.Up))
            {
                float Xv = _camera.Up[0];
                float Yv = _camera.Up[1];
                float Zv = _camera.Up[2];
                MoveCam((float)e.Time, Xv, Yv, Zv, cameraSpeed);
            }
            if (input.IsKeyDown(Keys.Down))
            {
                float Xv = -_camera.Up[0];
                float Yv = -_camera.Up[1];
                float Zv = -_camera.Up[2];
                MoveCam((float)e.Time, Xv, Yv, Zv, cameraSpeed);
            }

            var mouse = MouseState;

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);
                var pivot = CharacterObject._centerPosition;

                if (CheckPos(Asset3d.getRotationResult(pivot, new Vector3(0, 1, 0), -MathHelper.DegreesToRadians(deltaX * sensitivity), _camera.Position)))
                {
                    _camera.Position -= pivot;
                    _camera.Yaw += deltaX * sensitivity;
                    _camera.Position = Vector3.Transform(_camera.Position,
                        generateArbRotationMatrix(new Vector3(0, 1, 0), pivot, deltaX * sensitivity).ExtractRotation());
                    _camera.Position += pivot;
                    _camera._front = -Vector3.Normalize(_camera.Position - pivot);

                    _camera.Pitch -= deltaY * sensitivity;

                    character_angle_late_X += deltaX * sensitivity;
                    character_angle_late_Y += deltaY * sensitivity;
                }
            }
            if (Math.Abs(character_angle_late_Y) > character_turnaround_velocity)
            {
                CharacterObject.rotate(CharacterObject._centerPosition, new Vector3(1, 0, 0), -character_angle_late_Y * character_turnaround_velocity);
                character_angle_late_Y -= character_angle_late_Y * character_turnaround_velocity;
            }
            if (input.IsKeyDown(Keys.O) && _screenOn)
            {
                _screenOn = false;
                Console.WriteLine("Screen Off");
                blender.Child[1]._mtlDiffSpec[1] = new Vector3(0);
            }
            if (input.IsKeyDown(Keys.I) && !_screenOn)
            {
                _screenOn = true;
                Console.WriteLine("Screen On");
                blender.Child[1]._mtlDiffSpec[1] = new Vector3(1);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }


        public static float gridSize = 0.1f;     // Bisa Diubah2
        public static float Xrange = 10.0f;      // Range 3dworldnya, kalo onload benda yg diluar ini bisa error
        public static float Yrange = 10.0f;
        public static float Zrange = 10.0f;
        public static int[,,] CameraMap = new int[(int)(Xrange / gridSize) * 2, (int)(Yrange / gridSize) * 2, (int)(Zrange / gridSize) * 2];

        float Cmargin = 0.1f;   // Margin cameranya ke benda
        float character_angle_late_X = 0;
        float character_angle_late_Y = 0;
        float character_turnaround_velocity = 0.1f;
        public void MoveCam(float time, float Xv, float Yv, float Zv, float speed)
        {
            if (CheckPos(new Vector3(_camera.Position[0] + Cmargin * Xv, _camera.Position[1], _camera.Position[2]))) { _camera.Position += new Vector3(Xv, 0, 0) * speed * time; }
            if (CheckPos(new Vector3(_camera.Position[0], _camera.Position[1] + Cmargin * Yv, _camera.Position[2]))) { _camera.Position += new Vector3(0, Yv, 0) * speed * time; }
            if (CheckPos(new Vector3(_camera.Position[0], _camera.Position[1], _camera.Position[2] + Cmargin * Zv))) { _camera.Position += new Vector3(0, 0, Zv) * speed * time; }

            // Move Character
            CharacterObject.translate(new Vector3(_camera.Position[0] + _camera.Front[0] * charMargin, _camera.Position[1] + _camera.Front[1] * charMargin, _camera.Position[2] + _camera.Front[2] * charMargin) - CharacterObject._centerPosition);
            CharacterObject.rotate(CharacterObject._centerPosition, CharacterObject._euler[1], 0.000000000000000000001f);   // Gak tau 
            if (Math.Abs(character_angle_late_X) > character_turnaround_velocity)
            {
                CharacterObject.rotate(CharacterObject._centerPosition, CharacterObject._euler[1], -character_angle_late_X * character_turnaround_velocity);
                character_angle_late_X -= character_angle_late_X * character_turnaround_velocity;
            }
        }
        public bool CheckPos(Vector3 pos)
        {
            int X = (int)Math.Round((pos[0] + Xrange) / gridSize);
            int Y = (int)Math.Round((pos[1] + Yrange) / gridSize);
            int Z = (int)Math.Round((pos[2] + Zrange) / gridSize);

            if (X >= CameraMap.GetLength(0) || Y >= CameraMap.GetLength(1) || Z >= CameraMap.GetLength(2)) { return false; }
            if (CameraMap[X, Y, Z] == 1) { return false; }
            else { return true; }
        }
    }
}
