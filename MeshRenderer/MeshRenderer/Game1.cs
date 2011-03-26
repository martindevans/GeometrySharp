using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Primitives3D;
using GeometrySharp.HalfEdgeGeometry;
using GeometrySharp.ConstructiveSolidGeometry.Operations;
using GeometrySharp.ConstructiveSolidGeometry.Primitives;

namespace MeshRenderer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpherePrimitive sphere;
        CylinderPrimitive cylinder;
        BasicEffect faceEffect;
        BasicEffect vertexEffect;
        BasicEffect edgeEffect;

        Mesh mesh;

        Matrix world;
        Matrix view;
        Matrix projection;

        MouseState previousMouse;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;

            var tree = new Transform(new Sphere(3), Matrix.CreateScale(10));

            mesh = tree.MakeMesh();

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, GraphicsDevice.Viewport.AspectRatio, 0.1f, 100);
            view = Matrix.CreateLookAt(new Vector3(0, 0, -15), Vector3.Zero, Vector3.Up);
            world = Matrix.CreateScale(1);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sphere = new SpherePrimitive(GraphicsDevice);
            cylinder = new CylinderPrimitive(GraphicsDevice);

            faceEffect = new BasicEffect(GraphicsDevice);
            faceEffect.PreferPerPixelLighting = true;

            vertexEffect = new BasicEffect(GraphicsDevice);
            vertexEffect.EnableDefaultLighting();
            vertexEffect.PreferPerPixelLighting = true;

            edgeEffect = new BasicEffect(GraphicsDevice);
            edgeEffect.EnableDefaultLighting();
            edgeEffect.PreferPerPixelLighting = true;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState m = Mouse.GetState();
            if (previousMouse.LeftButton == ButtonState.Pressed && m.LeftButton == ButtonState.Pressed)
            {
                float sensitivity = 0.01f;
                Vector2 move = new Vector2(m.X - previousMouse.X, m.Y - previousMouse.Y) * sensitivity;

                world *= Matrix.CreateRotationY(move.X);
                world *= Matrix.CreateRotationX(-move.Y);
            }
            previousMouse = m;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawMesh(mesh, world, view, projection);

            base.Draw(gameTime);
        }

        private void DrawMesh(Mesh m, Matrix world, Matrix view, Matrix projection)
        {
            var previousDepthStencil = GraphicsDevice.DepthStencilState;

            DrawPrimitives(m, faceEffect, ref world, ref view, ref projection);

            GraphicsDevice.DepthStencilState = new DepthStencilState()
            {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = true,
            };
            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                FillMode = FillMode.Solid,
            };

            DrawVertices(m, vertexEffect, world, view, projection);
            //DrawEdges(m, edgeEffect, world, view, projection);
            //DrawFaces(m, faceEffect, world, view, projection);

            GraphicsDevice.DepthStencilState = previousDepthStencil;
        }

        private void DrawPrimitives(Mesh m, Effect effect, ref Matrix world, ref Matrix view, ref Matrix projection)
        {
            List<int> indices = new List<int>();
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();
            m.CopyData<VertexPositionColor, int>(indices.Add, a => { vertices.Add(a); return vertices.Count - 1; }, a => new VertexPositionColor(a.Position, Color.White));

            GraphicsDevice.DepthStencilState = new DepthStencilState()
            {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = false,
            };
            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                FillMode = FillMode.WireFrame,
            };

            Matrix myWorld = world;

            effect.Parameters["World"].SetValue(myWorld);
            effect.Parameters["WorldViewProj"].SetValue(myWorld * view * projection);
            if (effect.Parameters["DiffuseColor"] != null)
                effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector4());

            foreach (EffectPass effectPass in effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count, indices.ToArray(), 0, indices.Count / 3);
            }
        }

        private void DrawVertices(Mesh m, Effect effect, Matrix world, Matrix view, Matrix projection)
        {
            foreach (var v in m.Vertices.Select(a => a.Position))
            {
                Matrix myWorld = Matrix.CreateScale(0.4f) * Matrix.CreateTranslation(v) * world;

                effect.Parameters["World"].SetValue(myWorld);
                effect.Parameters["WorldViewProj"].SetValue(myWorld * view * projection);
                if (effect.Parameters["DiffuseColor"] != null)
                    effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
                sphere.Draw(effect);
            }
        }

        private void DrawEdges(Mesh m, Effect effect, Matrix world, Matrix view, Matrix projection)
        {
            foreach (var edge in m.HalfEdges.Where(a => a.Primary))
            {
                var start = edge.Twin.End.Position;
                var end = edge.End.Position;

                Vector3 position = start * 0.5f + end * 0.5f;
                Vector3 difference = end - start;

                float length = difference.Length();
                Matrix scale = Matrix.CreateScale(new Vector3(0.2f, length, 0.2f));

                difference.Normalize();
                Matrix rotation = Matrix.Identity;
                Vector3 axis = Vector3.Cross(Vector3.Up, difference);
                if (axis != Vector3.Zero)
                {
                    axis.Normalize();
                    float angle = (float)Math.Acos(Vector3.Dot(Vector3.Up, difference));
                    rotation = Matrix.CreateFromAxisAngle(axis, (float.IsNaN(angle) ? 0 : angle));
                }

                Matrix myWorld = scale * rotation * Matrix.CreateTranslation(position) * world;

                effect.Parameters["World"].SetValue(myWorld);
                effect.Parameters["WorldViewProj"].SetValue(myWorld * view * projection);
                if (effect.Parameters["DiffuseColor"] != null)
                    effect.Parameters["DiffuseColor"].SetValue(Color.Green.ToVector4());

                cylinder.Draw(effect);
            }
        }

        private void DrawFaces(Mesh m, Effect effect, Matrix world, Matrix view, Matrix projection)
        {
            var triangles = m.Faces.SelectMany(a => a.TriangulateFromSinglePoint).Select(a => new VertexPositionColor(a.Position, Color.WhiteSmoke)).ToArray();

            if (triangles.Length > 0)
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState()
                {
                    DepthBufferEnable = true,
                    DepthBufferWriteEnable = false,
                };
                GraphicsDevice.RasterizerState = new RasterizerState()
                {
                    CullMode = CullMode.None,
                    FillMode = FillMode.Solid,
                };

                Matrix myWorld = world;

                effect.Parameters["World"].SetValue(myWorld);
                effect.Parameters["WorldViewProj"].SetValue(myWorld * view * projection);
                if (effect.Parameters["DiffuseColor"] != null)
                    effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector4());

                foreach (var t in effect.CurrentTechnique.Passes)
                {
                    t.Apply();

                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, triangles, 0, triangles.Length / 3);
                }
            }
        }
    }
}
