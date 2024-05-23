using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_CsLo;

namespace StandaloneExample
{
	public static class Program
	{
		public static List<Vector2> getAirfoil(string filepath, ref string airfoilName)
		{
			var lines = File.ReadAllLines(filepath);
			airfoilName = lines[0].Trim();
			var points = new List<Vector2>();
			for (var i = 1; i < lines.Length - 1; i += 1)
			{
				var line = lines[i];
				line = line.Replace("      ", "  ");
				string[] pointPosition;
				if (line.Trim().Contains(" -"))
				{
					pointPosition = line.Trim().Split(" ");
					pointPosition[0] = pointPosition[0].Trim();
					pointPosition[1] = pointPosition[1].Trim();
				}
				else
				{
					pointPosition = line.Trim().Split("  ");
					pointPosition[0] = pointPosition[0].Trim();
					pointPosition[1] = pointPosition[1].Trim();
				}
				Vector2 pointVector = new Vector2(float.Parse(pointPosition[0]), float.Parse(pointPosition[1]));
				//Console.WriteLine("|" + pointPosition[0] + "|" + pointPosition[1] + "|");
				points.Add(pointVector);
			}
			return points;
		}
		public static float positive(float num)
		{
			return num > 0 ? num : -num;
		}
		public static Vector2 normal(Vector2 start, Vector2 end)
		{
			float dx = end.X - start.X;
			float dy = end.Y - start.Y;
			Vector2 normal = normalizeVector(new Vector2(dx, dy));
			//Vector2 normal = new Vector2(dx, dy);
			return normal;
		}
		public static Vector2 averageVector(Vector2 v1, Vector2 v2)
		{
			return new Vector2((v1.X + v2.X) / 2, (v1.Y + v2.Y) / 2);
		}
		public static Vector2 normalizeVector(Vector2 vector)
		{
			Vector2 returnVector = Vector2.Zero;
			bool negativex = vector.X < 0;

			bool negativey = vector.Y < 0;

			if (positive(vector.X) > positive(vector.Y))
			{
				returnVector.Y = vector.Y / vector.X;
				returnVector.X = 1;
			}
			else if (positive(vector.X) < positive(vector.Y))
			{
				returnVector.X = vector.X / vector.Y;
				returnVector.Y = 1;
			}
			return returnVector;
		}

		public static void Main(string[] args)
		{
			string airfoilName = "";

			int airfoilScale = 700;

			int windowWidth = 1280;
			int windowHeight = 720;
			Raylib.InitWindow(windowWidth, windowHeight, "Aviary");
			Raylib.SetTargetFPS(120);

			List<Vector2> currentAirfoil = getAirfoil(filepath: "C:/Users/Aaron/Aviary/airfoils/n0009sm.dat", ref airfoilName);
			Console.WriteLine("Current airfoil: " + airfoilName);

			int triangleSize = 50;

			while (!Raylib.WindowShouldClose()) // Detect window close button or ESC key
			{
				Raylib.BeginDrawing();
				Raylib.ClearBackground(new Color(2, 2, 2, 255));
				Raylib.DrawTriangle(
					new Vector2(windowWidth - triangleSize, windowHeight),
					new Vector2(windowWidth, windowHeight),
					new Vector2(windowWidth, windowHeight - triangleSize),
					new Color(255, 150, 100, 255)
				);
				Raylib.DrawText("Current airfoil: " + airfoilName, 150, 10, 20, Raylib.RED);
				Vector2 lastPoint = currentAirfoil[0];

				Raylib.DrawLineEx(
					new Vector2(
						windowWidth / 2 - airfoilScale / 2,
						windowHeight / 2
					),
					new Vector2(
						windowWidth / 2 + airfoilScale / 2,
						windowHeight / 2
					),
					3,
					new Color(100, 150, 255, 255)
				);

				for (int i = 1; i < currentAirfoil.Count - 1; i++)
				{
					Vector2 currentPoint = currentAirfoil[i];
					int lastPointX = (int)MathF.Round(lastPoint.X * airfoilScale);
					int lastPointY = (int)MathF.Round(lastPoint.Y * airfoilScale);

					int currentPointX = (int)MathF.Round(currentPoint.X * airfoilScale);
					int currentPointY = (int)MathF.Round(currentPoint.Y * airfoilScale);

					Raylib.DrawLineEx(
						new Vector2(
							windowWidth / 2 + lastPointX - airfoilScale / 2,
							windowHeight / 2 + lastPointY
							),
						new Vector2(
							windowWidth / 2 + currentPointX - airfoilScale / 2,
							windowHeight / 2 + currentPointY
						),
						3,
						Raylib.GREEN
					);
					if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
					{
						Vector2 normalizedFace = normal(lastPoint * airfoilScale, currentPoint * airfoilScale) * 50;
						Vector2 averageFace = averageVector(lastPoint * airfoilScale, currentPoint * airfoilScale);
						//Console.WriteLine(normalizedFace);
						Raylib.DrawLine(
							windowWidth / 2 + (int)averageFace.X - airfoilScale / 2,
							windowHeight / 2 + (int)averageFace.Y,
							windowWidth / 2 + (int)averageFace.X + (int)-normalizedFace.X - airfoilScale / 2,
							windowHeight / 2 + (int)averageFace.Y + (int)normalizedFace.Y,
							Raylib.YELLOW
						);
					}
					lastPoint = currentPoint;
				}
				Raylib.DrawFPS(10, 10);
				//Raylib.DrawCircle((int)Raylib.GetMousePosition().X, (int)Raylib.GetMousePosition().Y, 10, Raylib.BROWN);
				int mousePositionX = (int)Raylib.GetMousePosition().X;
				int mousePositionY = (int)Raylib.GetMousePosition().Y;
				Raylib.EndDrawing();
			}
			Raylib.CloseWindow();
		}
	}
}