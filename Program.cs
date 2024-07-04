using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_CsLo;

namespace StandaloneExample
{
	public static class Program
	{
		public static List<Vector2> getAirfoil(string filepath, ref string airfoilName, bool flipAirfoil = true)
		{
			var lines = File.ReadAllLines(filepath);
			airfoilName = lines[0].Trim();
			var points = new List<Vector2>();
			for (var i = 1; i < lines.Length; i += 1)
			{
				var line = lines[i];
				line = line.Replace("      ", "  ");
				string[] pointPosition;
				if (line == "")
					continue;
				if (line.Trim().Contains(" -"))
				{
					pointPosition = line.Trim().Split(" ");
					pointPosition[0] = pointPosition[0].Trim();
					pointPosition[1] = pointPosition[1].Trim();
				}
				else
				{
					pointPosition = line.Trim().Split("  ");
					if (pointPosition.Length < 2)
						pointPosition = line.Trim().Split(" ");
					pointPosition[0] = pointPosition[0].Trim();
					pointPosition[1] = pointPosition[1].Trim();
				}
				Vector2 pointVector = new Vector2(float.Parse(pointPosition[0]) - 0.5f, float.Parse(pointPosition[1]) * (flipAirfoil ? -1 : 1));
				points.Add(pointVector);
			}
			return points;
		}
		public static float AirPressureAtAltitude(float celsius, float altitudeMeters)
		{
			const float g = 9.80665f;       // acceleration due to gravity, m/s^2
			const float M = 0.0289644f;     // molar mass of Earth's air, kg/mol
			const float R = 8.3144598f;     // universal gas constant, J/(mol*K)
			const float T0 = 288.15f;       // standard temperature at sea level, K
			const float P0 = 101325f;       // standard pressure at sea level, Pa
			const float L = 0.0065f;        // temperature lapse rate, K/m

			float kelvin = celsius + 273.15f;

			if (altitudeMeters > 11000)
				throw new ArgumentOutOfRangeException("This formula is valid only up to 11,000 meters altitude.");

			float T = T0 - (L * altitudeMeters);

			// In pascals
			float pressurePascals = P0 * MathF.Pow(T / T0, (g * M) / (R * L));
			return pressurePascals;
		}
		public static float positive(float num)
		{
			return num > 0 ? num : -num;
		}
		public static Vector2 perp(Vector2 v, bool flip = false, bool normalize = false)
		{
			return (normalize ? normalizeVector(new Vector2(v.Y, v.X)) : new Vector2(v.Y, v.X)) * (flip ? -1 : 1);
		}
		public static Vector2 normal(Vector2 start, Vector2 end, bool flipNormal = false)
		{
			float dx = end.X - start.X;
			float dy = end.Y - start.Y;
			Vector2 normal = normalizeVector(new Vector2(-dx, dy * (flipNormal ? -1 : 1)));
			//Vector2 normal = new Vector2(dx, dy);
			return normal;
		}
		public static float distance(Vector2 v1, Vector2 v2)
		{
			return (float)Math.Sqrt(Math.Pow(v1.X + v2.X, 2) + Math.Pow(v1.Y + v2.Y, 2));
		}
		public static Vector2 averageVector(Vector2 v1, Vector2 v2)
		{
			return new Vector2((v1.X + v2.X) / 2, (v1.Y + v2.Y) / 2);
		}
		public static Vector2 normalizeVector(Vector2 vector)
		{
			Vector2 a = vector;
			float m = MathF.Sqrt(a.X * a.X + a.Y * a.Y);
			a.X /= m;
			a.Y /= m;
			return a;
		}

		public static void Main(string[] args)
		{
			string airfoilName = "";

			int zoom = 800;

			int airfoilScale = 1;

			int xOffset = 0;
			int yOffset = 0;

			int windowWidth = 1280;
			int windowHeight = 720;

			int homeX;
			int homeY;

			float airSpeed = 0;
			float wingWidth = 6;
			float wingTopArea = 0;
			float wingBottomArea = 0;
			float wingArea = 0;

			float wingTopLength = 0;
			float wingBottomLength = 0;

			float staticAirPressure = AirPressureAtAltitude(15, 500);

			Vector3 drag = Vector3.Zero;
			Vector3 lift = Vector3.Zero;

			Raylib.InitWindow(windowWidth, windowHeight, "Aviary");
			Raylib.SetTargetFPS(120);

			bool flipAirfoil = true;
			//List<Vector2> currentAirfoil = getAirfoil(filepath: "C:/Users/Aaron/Aviary/airfoils/fauvel.dat", ref airfoilName,flipAirfoil);
			List<Vector2> currentAirfoil = getAirfoil(filepath: "C:/Users/Aaron/Aviary/airfoils/n0009sm.dat", ref airfoilName, flipAirfoil);
			//List<Vector2> currentAirfoil = getAirfoil(filepath: "C:/Users/Aaron/Aviary/airfoils/hause.dat", ref airfoilName, flipAirfoil);
			//List<Vector2> currentAirfoil = getAirfoil(filepath: "C:/Users/Aaron/Aviary/airfoils/stcyr171.dat", ref airfoilName, flipAirfoil);
			Console.WriteLine("Current airfoil: " + airfoilName);

			int triangleSize = 50;

			//Console.WriteLine(string.Join("\n", currentAirfoil));
			while (!Raylib.WindowShouldClose())
			{
				wingTopLength = 0;
				wingBottomLength = 0;
				float scale = zoom / 140f;
				homeX = windowWidth / 2 + xOffset;
				homeY = windowHeight / 2 + yOffset;
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

				Vector2 chordStart = new Vector2(
						windowWidth / 2 + xOffset - zoom / 2 * airfoilScale,
						windowHeight / 2 + yOffset
					);
				Vector2 chordEnd = new Vector2(
						windowWidth / 2 + xOffset + zoom / 2 * airfoilScale,
						windowHeight / 2 + yOffset
					);
				Raylib.DrawLineEx(
					chordStart,
					chordEnd,
					3,
					new Color(100, 150, 255, 255)
				);
				for (int i = 0; i < currentAirfoil.Count; i++)
				{
					Vector2 currentPoint = currentAirfoil[i];
					bool isOnBottom = currentPoint.Y < 0 || lastPoint.Y < 0;
					if (isOnBottom)
						wingBottomLength += distance(lastPoint, currentPoint) * airfoilScale;
					else
						wingTopLength += distance(lastPoint, currentPoint) * airfoilScale;

					Vector2 lastPointScaled = lastPoint * airfoilScale * zoom;
					Vector2 curPointScaled = currentPoint * airfoilScale * zoom;
					Raylib.DrawLineEx(
						new Vector2(
							homeX + lastPointScaled.X,
							homeY + lastPointScaled.Y
						),
						new Vector2(
							homeX + curPointScaled.X,
							homeY + curPointScaled.Y
						),
						3,
						Raylib.GREEN
					);
					if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
					{
						Vector2 faceStart = new Vector2(homeX + lastPointScaled.X, homeY + lastPointScaled.Y);
						Vector2 faceEnd = new Vector2(homeX + curPointScaled.X, homeY + curPointScaled.Y);
						Raylib.DrawCircle((int)faceStart.X, (int)faceStart.Y, 4, Raylib.RED);
						Raylib.DrawCircle((int)faceEnd.X, (int)faceEnd.Y, 4, Raylib.RED);
						//bool toReverseNormal = curPointScaled.Y < 0;
						Vector2 normalizedFace = perp(normal(faceStart, faceEnd) * 10, flipAirfoil);
						Vector2 averageFace = averageVector(faceStart, faceEnd);
						Raylib.DrawLineEx(
							averageFace,
							averageFace + normalizedFace * scale,
							2,
							isOnBottom ? Raylib.PINK : Raylib.ORANGE
						);
					}
					lastPoint = currentPoint;
				}

				wingTopArea = wingTopLength * wingWidth;
				wingBottomArea = wingBottomLength * wingWidth;
				wingArea = wingTopArea + wingBottomArea;

				if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
				{
					Vector2 chordNormal = perp(normal(chordStart, chordEnd) * 100, false);
					Vector2 chordAverage = averageVector(chordStart, chordEnd);
					Raylib.DrawLineEx(
						chordAverage,
						chordAverage + chordNormal * scale,
						3,
						Raylib.ORANGE
					);
				}
				if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
				{
					airSpeed++;
				}
				if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
				{
					airSpeed--;
				}
				Raylib.DrawFPS(10, 10);
				List<string> stats = new List<string>
				{
					"Airspeed (m/s): " + airSpeed,
					"Air pressure (Pa): " + staticAirPressure,
					"Wing width (m): " + wingWidth,
					"Wing top length (m): " + wingTopLength,
					"Wing bottom length (m): " + wingBottomLength,
					"Wing top area (m²): " + wingTopArea,
					"Wing bottom area (m²): " + wingBottomArea,
					"Wing total area (m²): " + wingArea
				};
				for (int i = 0; i < stats.Count; i++)
				{
					Raylib.DrawText(stats[i], 10, (i * 20) + 40, 20, Raylib.YELLOW);
				}
				//Raylib.DrawCircle((int)Raylib.GetMousePosition().X, (int)Raylib.GetMousePosition().Y, 10, Raylib.BROWN);
				zoom += (int)Raylib.GetMouseWheelMoveV().Y * 50;
				zoom = Math.Clamp(zoom, 150, 3000);
				if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
				{
					Vector2 mouseDelta = Raylib.GetMouseDelta();
					xOffset += (int)mouseDelta.X;
					yOffset += (int)mouseDelta.Y;
				}
				Raylib.EndDrawing();
			}
			Raylib.CloseWindow();
		}
	}
}