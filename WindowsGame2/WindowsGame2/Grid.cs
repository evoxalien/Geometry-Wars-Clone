using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GeometryWars
{
    public class Grid
    {
        Spring[] springs;
        PointMass[,] points;

        #region Grid
        public Grid(Rectangle size, Vector2 spacing)
        {
            var springList = new List<Spring>();

            int numColumns = (int)(size.Width / spacing.X) + 1;
            int numRows = (int)(size.Height / spacing.Y) + 1;
            points = new PointMass[numColumns, numRows];

            //These fixed points will be used to anchor the grid to fixed positions on screen
            PointMass[,] fixedPoints = new PointMass[numColumns, numRows];

            //create the point masses
            int column = 0, row = 0;
            for (float y = size.Top; y <= size.Bottom; y += spacing.Y)
            {
                for (float x = size.Left; x <= size.Right; x += spacing.X)
                {
                    points[column, row] = new PointMass(new Vector3(x, y, 0), 1);
                    fixedPoints[column, row] = new PointMass(new Vector3(x, y, 0), 0);
                    column++;
                }
                row++;
                column = 0;
            }

            //link the point masses with springs
            for (int y = 0; y < numRows; y++)
            {
                for (int x = 0; x < numColumns; x++)
                {
                    if (x == 0 || y == 0 || x == numColumns - 1 || y == numRows - 1)    // anchor the border of the grid 
                        springList.Add(new Spring(fixedPoints[x, y], points[x, y], 0.1f, 0.1f));
                    else if (x % 3 == 0 && y % 3 == 0)                                  // loosely anchor 1/9th of the point masses 
                        springList.Add(new Spring(fixedPoints[x, y], points[x, y], 0.002f, 0.02f));

                    const float stiffness = 0.28f;
                    const float damping = 0.10f;
                    //const float stiffness = 0.28f;
                    //const float damping = 0.06f;
                    if (x > 0)
                        springList.Add(new Spring(points[x - 1, y], points[x, y], stiffness, damping));
                    if (y > 0)
                        springList.Add(new Spring(points[x, y - 1], points[x, y], stiffness, damping));
                }
            }
            springs = springList.ToArray();

        }
        #endregion

        #region AdditionalFunctions
        public Vector2 ToVec2(Vector3 v)
        {
            // do a perspective projection
            float factor = (v.Z + 2000) / 2000;
            return (new Vector2(v.X, v.Y) - GameRoot.ScreenSize / 2f) * factor + GameRoot.ScreenSize / 2;
        }
        #endregion

        #region ApplyDirectedForce
        public void ApplyDirectedForce(Vector2 force, Vector2 position, float radius)
        {
            ApplyDirectedForce(new Vector3(force, 0), new Vector3(position, 0), radius);
        }

        public void ApplyDirectedForce(Vector3 force, Vector3 position, float radius)
        {
            foreach (var mass in points)
                if (Vector3.DistanceSquared(position, mass.Position) < radius * radius)
                    mass.ApplyForce(10 * force / (10 + Vector3.Distance(position, mass.Position)));
        }
        #endregion

        #region ApplyImplosiveForce
        public void ApplyImplosiveForce(float force, Vector2 position, float radius)
        {
            ApplyImplosiveForce(force, new Vector3(position, 0), radius);
        }

        public void ApplyImplosiveForce(float force, Vector3 position, float radius)
        {
            foreach (var mass in points)
            {
                float dist2 = Vector3.DistanceSquared(position, mass.Position);
                if (dist2 < radius * radius)
                {
                    mass.ApplyForce(10 * force * (position - mass.Position) / (100 + dist2));
                    mass.IncreaseDamping(0.6f);
                }
            }
        }
        #endregion

        #region ApplyExplosiveForce
        public void ApplyExplosiveForce(float force, Vector2 position, float radius)
        {
            ApplyExplosiveForce(force, new Vector3(position, 0), radius);
        }

        public void ApplyExplosiveForce(float force, Vector3 position, float radius)
        {
            foreach (var mass in points)
            {
                float dist2 = Vector3.DistanceSquared(position, mass.Position);
                if (dist2 < radius * radius)
                {
                    mass.ApplyForce(100 * force * (mass.Position - position) / (10000 + dist2));
                    mass.IncreaseDamping(0.6f);
                }
            }
        }
        #endregion

        #region Update
        public void Update()
        {
            foreach (var spring in springs)
                spring.Update();

            foreach (var mass in points)
                mass.Update();
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            int width = points.GetLength(0);
            int height = points.GetLength(1);
            Color color = new Color(30, 30, 139, 85);   // dark blue

            for (int y = 1; y < height; y++)
            {
                for (int x = 1; x < width; x++)
                {

                    Vector2 left = new Vector2(), up = new Vector2(); Vector2 p = ToVec2(points[x, y].Position);
                    if (x > 1)
                    {
                        left = ToVec2(points[x - 1, y].Position);
                        float thickness = y % 3 == 1 ? 3f : 1f;

                        // use Catmull-Rom interpolation to help smooth bends in the grid
                        int clampedX = Math.Min(x + 1, width - 1);
                        Vector2 mid = Vector2.CatmullRom(ToVec2(points[x - 2, y].Position), left, p, ToVec2(points[clampedX, y].Position), 0.5f);

                        // If the grid is very straight here, draw a single straight line. Otherwise, draw lines to our
                        // new interpolated midpoint
                        if (Vector2.DistanceSquared(mid, (left + p) / 2) > 1)
                        {
                            spriteBatch.DrawLine(left, mid, color, thickness);
                            spriteBatch.DrawLine(mid, p, color, thickness);
                        }
                        else
                            spriteBatch.DrawLine(left, p, color, thickness);
                    }
                    if (y > 1)
                    {
                        up = ToVec2(points[x, y - 1].Position);
                        float thickness = x % 3 == 1 ? 3f : 1f;
                        int clampedY = Math.Min(y + 1, height - 1);
                        Vector2 mid = Vector2.CatmullRom(ToVec2(points[x, y - 2].Position), up, p, ToVec2(points[x, clampedY].Position), 0.5f);

                        if (Vector2.DistanceSquared(mid, (up + p) / 2) > 1)
                        {
                            spriteBatch.DrawLine(up, mid, color, thickness);
                            spriteBatch.DrawLine(mid, p, color, thickness);
                        }
                        else
                            spriteBatch.DrawLine(up, p, color, thickness);
                    }
                }
            }
        }
        #endregion
    }
}
