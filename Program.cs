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
		public static void DrawRing(float x, float y, float radius, float thickness, Color color, float opacity, float startAngle = 0, float endAngle = 360, int segments = 6)
		{
			Raylib.DrawRing(new Vector2(x, y), radius - thickness, radius, startAngle + 180, endAngle + 180, segments, Raylib.Fade(color, opacity));
		}
		public static void DrawCircle(float x, float y, float radius, Color color, float opacity)
		{
			Raylib.DrawCircleV(new Vector2(x, y), radius, Raylib.Fade(color, opacity));

		}
		public static float CelsiusToKelvin(float celsius)
		{
			return celsius + 273.15f;
		}
		public static float AirPressureAtAltitude(float celsius, float altitudeMeters)
		{
			const float g = 9.80665f;               // acceleration due to gravity, m/s²
			const float M = 0.0289644f;             // molar mass of Earth's air, kg/mol
			const float R = 8.3144598f;             // universal gas constant, J/(mol*K)
			float T0 = CelsiusToKelvin(celsius);    // standard temperature at sea level, K
			const float P0 = 101325f;               // standard pressure at sea level, Pa
			const float L = 0.0065f;                // temperature lapse rate, K/m


			if (altitudeMeters > 11000)
				throw new ArgumentOutOfRangeException("This formula is valid only up to 11,000 meters altitude.");

			float T = T0 - (L * altitudeMeters);

			// In pascals
			float pressurePascals = P0 * MathF.Pow(T / T0, g * M / (R * L));
			return pressurePascals;
		}
		public static float AirDensity(float temperature, float pressure)
		{
			const float R = 287.05f;
			float kelvin = CelsiusToKelvin(temperature);
			return pressure / (R * kelvin);
		}
		public static float DynamicAirPressure(float density, float airSpeed, float temperature)
		{
			// we use the formula q = ½pv²
			return 0.5f * density * (float)Math.Pow(airSpeed, 2);
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
		public static float coefficientOfLift(float AoA)
		{
			//We use the formula CL = 2π sin α
			return 2 * MathF.PI * MathF.Sin(AoA);
		}
		public static float coefficientOfPressure(float staticAirPressure, float dynamicAirPressure)
		{
			/*
			We use this formula:
			 	 p − p∞
			Cp = ______
				   q∞
			*/
			// To get the value [p], we must use Bernoulli's law, which requires local flow velocity, which requires using the panel method. 
			// This system has more dependencies than Next.js 😭
			throw new NotImplementedException("This system has more dependencies than Next.js 😭");
			//return (localPressure - staticAirPressure) / dynamicAirPressure;
		}
		public static float findCirculationAtPoint(float pointOnChord, float chord, float freestreamVelocity, float angleofattack)
		{
			// γ =2U sin α (c/x −1)^(1/2)
			// where U is the freestream velocity
			// and α is the angle of attack
			// where c is the length of the chord
			// and x is the position on the chord

			// using double datatype because i don't want to keep casting over and over again
			double γ = 2 * freestreamVelocity * Math.Sin(angleofattack) * Math.Pow(chord / pointOnChord * -1, 1 / 2);
			return (float)γ;
		}
		public static float normalLiftVelocity(float Γ, float x, float normal)
		{
			// w = −(Γ / 2π) * (x−xo)/(z−zo)²+(x−xo)²
			float w = (Γ / (2 * Math.PI)) * (x - xo) / (normal - zo)²+(x - xo)²
		}
		public static void Main(string[] args)
		{
			string airfoilName = "";

			int airfoilScale = 800;

			int windowWidth = 1280;
			int windowHeight = 720;

			float airSpeed = 0;
			float wingWidth = 6;
			float wingTopArea;
			float wingBottomArea;
			float wingArea;

			float wingTopLength;
			float wingBottomLength;

			float airTemperature = 15;
			float altitude = 500;

			float staticAirPressure;
			float airDensity;
			float dynamicAirPressure;

			int simTime = 0;
			//Vector3 drag;
			//Vector3 lift;


			Camera2D camera;
			camera.target = Vector2.Zero;
			camera.offset = new Vector2(windowWidth / 2.0f, windowHeight / 2.0f);
			camera.rotation = 0.0f;
			camera.zoom = 1.0f;
			bool drawDebug = false;


			bool flipAirfoil = true;
			List<Vector2> currentAirfoil = getAirfoil(filepath: "C:/Users/Aaron/Aviary/airfoils/fauvel.dat", ref airfoilName, true);
			//List<Vector2> currentAirfoil = getAirfoil(filepath: "C:/Users/Aaron/Aviary/airfoils/n0009sm.dat", ref airfoilName, true);
			//List<Vector2> currentAirfoil = getAirfoil(filepath: "C:/Users/Aaron/Aviary/airfoils/hause.dat", ref airfoilName, false);
			//List<Vector2> currentAirfoil = getAirfoil(filepath: "C:/Users/Aaron/Aviary/airfoils/stcyr171.dat", ref airfoilName, true);
			Console.WriteLine("Current airfoil: " + airfoilName);

			int triangleSize = 50;

			//Console.WriteLine(string.Join("\n", currentAirfoil));

			int gridSizeX = 25;
			int gridSizeY = 25;

			Raylib.InitWindow(windowWidth, windowHeight, "Aviary");
			Raylib.SetTargetFPS(120);

			while (!Raylib.WindowShouldClose())
			{
				wingTopLength = 0;
				wingBottomLength = 0;

				staticAirPressure = AirPressureAtAltitude(airTemperature, altitude);
				airDensity = AirDensity(airTemperature, staticAirPressure);
				dynamicAirPressure = DynamicAirPressure(airDensity, airSpeed, airTemperature);

				Raylib.BeginDrawing();
				Raylib.ClearBackground(new Color(2, 2, 2, 255));
				Raylib.BeginMode2D(camera);
				#region Draw Grid
				//Initialize values
				int brightness = 25;
				float placeholderScale = 1;
				float scaledWindowXMin = camera.target.X - windowWidth / 2 / camera.zoom;
				float scaledWindowXMax = camera.target.X + windowWidth / 2 / camera.zoom;
				float correctedGridOffsetX = scaledWindowXMin % gridSizeX;

				float scaledWindowYMin = camera.target.Y - windowHeight / 2 / camera.zoom;
				float scaledWindowYMax = camera.target.Y + windowHeight / 2 / camera.zoom;
				float correctedGridOffsetY = scaledWindowYMin % gridSizeY;

				#region Vertical Lines
				for (float xPosition = scaledWindowXMin - correctedGridOffsetX; xPosition < scaledWindowXMax; xPosition += gridSizeX * placeholderScale)
				{
					Raylib.DrawLine((int)xPosition, (int)scaledWindowYMin, (int)xPosition, (int)scaledWindowYMax, new Color(brightness, brightness, brightness, 255));
				}
				#endregion

				#region Horizontal Lines
				for (float yPosition = scaledWindowYMin - correctedGridOffsetY; yPosition < scaledWindowYMax; yPosition += gridSizeY * placeholderScale)
				{
					Raylib.DrawLine((int)scaledWindowXMin, (int)yPosition, (int)scaledWindowXMax, (int)yPosition, new Color(brightness, brightness, brightness, 255));
				}
				#endregion

				#endregion

				Vector2 lastPoint = currentAirfoil[0];

				Vector2 chordStart = new Vector2(
						-airfoilScale / 2,
						0
					);
				Vector2 chordEnd = new Vector2(
						airfoilScale / 2,
						0
					);

				Raylib.DrawLineEx(
					chordStart,
					chordEnd,
					3,
					new Color(100, 150, 255, 255)
				);
				int scale = 5;

				if (drawDebug)
				{
					Vector2 chordNormal = perp(normal(chordStart, chordEnd) * 40, false);
					Vector2 chordAverage = averageVector(chordStart, chordEnd);
					Raylib.DrawLineEx(
						chordAverage,
						chordAverage + chordNormal * scale,
						3,
						Raylib.ORANGE
					);
				}

				for (int i = 0; i < currentAirfoil.Count; i++)
				{
					Vector2 currentPoint = currentAirfoil[i];
					bool isOnBottom = currentPoint.Y < 0 || lastPoint.Y < 0;
					if (isOnBottom)
						wingBottomLength += distance(lastPoint * airfoilScale, currentPoint * airfoilScale);
					else
						wingTopLength += distance(lastPoint * airfoilScale, currentPoint * airfoilScale);

					Vector2 lastPointScaled = lastPoint * airfoilScale;
					Vector2 curPointScaled = currentPoint * airfoilScale;
					Raylib.DrawLineEx(
						new Vector2(
							lastPointScaled.X,
							lastPointScaled.Y
						),
						new Vector2(
							curPointScaled.X,
							curPointScaled.Y
						),
						3,
						Raylib.GREEN
					);
					if (drawDebug)
					{
						Vector2 faceStart = new Vector2(lastPointScaled.X, lastPointScaled.Y);
						Vector2 faceEnd = new Vector2(curPointScaled.X, curPointScaled.Y);
						DrawCircle(faceStart.X, faceStart.Y, 4, Raylib.RED, 1f);
						//Raylib.DrawRing(new Vector2((int)faceStart.X, (int)faceStart.Y), 9, 10, 180, 0, 128, Raylib.Fade(Raylib.MAROON, 1f));
						DrawRing(faceStart.X, faceStart.Y, 10, 2, Raylib.PURPLE, 0.5f, segments: 64);
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

				if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
				{
					airSpeed++;
				}
				if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
				{
					airSpeed--;
				}

				if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
				{
					drawDebug = !drawDebug;
				}
				Raylib.EndMode2D();

				Raylib.DrawFPS(10, 10);

				Raylib.DrawText("Current airfoil: " + airfoilName, 150, 10, 20, Raylib.RED);
				Raylib.DrawTriangle(
					new Vector2(windowWidth - triangleSize, windowHeight),
					new Vector2(windowWidth, windowHeight),
					new Vector2(windowWidth, windowHeight - triangleSize),
					new Color(255, 150, 100, 255)
				);

				List<string> stats = new List<string>
				{
					$"Simulation time (ms): {MathF.Round(simTime)}",
					//$"Camera Offset (Vector2): {camera.offset}",
					//$"Camera Position (Vector2): {camera.target}",
					$"Grid movement (float): {correctedGridOffsetX}",
					$"Camera Zoom (float): {camera.zoom}",
					$"Airspeed (m/s): {airSpeed}",
					$"Static air pressure (Pa): {staticAirPressure}",
					$"Air Density (kg/m³): {airDensity}",
					$"Dynamic air pressure (Pa): {dynamicAirPressure}",
					$"Wing width (m): {wingWidth}",
					$"Wing top length (m): {wingTopLength}",
					$"Wing bottom length (m): {wingBottomLength}",
					$"Wing top area (m²): {wingTopArea}",
					$"Wing bottom area (m²): {wingBottomArea}",
					$"Wing total area (m²): {wingArea}"
				};
				for (int i = 0; i < stats.Count; i++)
				{
					Raylib.DrawText(stats[i], 10, (i * 20) + 40, 20, Raylib.YELLOW);
				}

				camera.zoom += (float)Raylib.GetMouseWheelMove() * 0.1f;
				camera.zoom = Math.Clamp(camera.zoom, 0.1f, 10f);
				if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
				{
					camera.target -= Raylib.GetMouseDelta() / camera.zoom;
				}
				Raylib.EndDrawing();
				simTime += (int)(Raylib.GetFrameTime() * 1000);
			}
			Raylib.CloseWindow();
		}
	}
}