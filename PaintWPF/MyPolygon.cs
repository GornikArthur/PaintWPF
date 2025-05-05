using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MyFigureLibrary;

namespace PaintWPF
{
	public class MyPolygon : MyFigure
	{
		public override string Name => "FPolygon";
		public List<MyLine> arr_lines { get; set; } = new List<MyLine>();
		public int last_line { get; set; } = 0;
		public List<Point> Points { get; set; } = new List<Point>();
		public string StrokeColor { get; set; }
		public double StrokeThickness { get; set; }

		[JsonIgnore] // Не сериализуем сам WPF-объект
		private Polygon polygon;

		public static MyPolygon my_polygon = null;

		public MyPolygon() { } // Пустой конструктор для десериализации

		public MyPolygon(Point startPoint, Color selectedColor, int thickness, Canvas Paint_canvas, List<MyFigure> arr_figures)
		{
			MyLine new_line = new MyLine(startPoint, selectedColor, thickness, Paint_canvas, arr_figures);
			arr_lines.Add(new_line);
			StrokeColor = selectedColor.ToString();
			StrokeThickness = thickness;
		}

		public override void Calc(Point newPoint)
		{
			arr_lines[last_line].line.X2 = newPoint.X;
			arr_lines[last_line].line.Y2 = newPoint.Y;

			arr_lines[0].line.X2 = newPoint.X;
			arr_lines[0].line.Y2 = newPoint.Y;
		}

		public MyLine GetLineByIndex(int index)
		{
			return arr_lines[index];
		}
		public Polygon GetFigure()
		{
			if (polygon != null) return polygon;
			return null;
		}

		public void AddLine(MyLine new_line)
		{
			last_line++;
			arr_lines.Add(new_line);
		}

		public void Make_Polygon(Canvas Paint_canvas, List<MyFigure> arr_figures,  Color color, int thickness)
		{
			Points.Clear();
			Points.Add(new Point(arr_lines[0].line.X1, arr_lines[0].line.Y1));
			for (int i = 1; i < arr_lines.Count; i++)
			{
				Points.Add(new Point(arr_lines[i].line.X2, arr_lines[i].line.Y2));
			}

			foreach (var line in arr_lines)
			{
				Paint_canvas.Children.Remove(line.GetFigure());
				arr_figures.Remove(line);
			}

			arr_lines.Clear();

			polygon = new Polygon
			{
				Points = new PointCollection(Points),
				Stroke = new SolidColorBrush(color),
				StrokeThickness = thickness
			};

			Paint_canvas.Children.Add(polygon);
		}

		public override bool IsPointInside(Point point)
		{
			int n = Points.Count;
			bool isInside = false;

			for (int i = 0, j = n - 1; i < n; j = i++)
			{
				Point vertex1 = Points[i];
				Point vertex2 = Points[j];

				if ((vertex1.Y > point.Y) != (vertex2.Y > point.Y) &&
					(point.X < (vertex2.X - vertex1.X) * (point.Y - vertex1.Y) / (vertex2.Y - vertex1.Y) + vertex1.X))
				{
					isInside = !isInside;
				}
			}

			return isInside;
		}

		public override void SetFillColor(Color color)
		{
			if (polygon != null)
				polygon.Fill = new SolidColorBrush(color);
		}

		public override void RemoveFigure(Canvas canvas)
		{
			if (polygon != null)
				canvas.Children.Remove(polygon);
		}
		private void InitializePolygon()
		{
			if (Points.Count == 0) return; // Если нет точек, нельзя создать полигон

			polygon = new Polygon
			{
				Points = new PointCollection(Points),
				Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(StrokeColor),
				StrokeThickness = StrokeThickness
			};
		}

		public override void AddFigure(Canvas canvas)
		{
			if (polygon == null) InitializePolygon();
			canvas.Children.Add(polygon);
		}
		public override void MouseMove(Point pos, Canvas Paint_canvas, List<MyFigure> arr_figures)
		{
			if (my_polygon != null && my_polygon.GetLineByIndex(my_polygon.last_line) != null)
			{
				my_polygon.GetLineByIndex(my_polygon.last_line).Calc(pos);
			}
		}
		public override void CustomMouseMove(Point currentPoint)
		{
			if (my_polygon != null && my_polygon.GetLineByIndex(last_line) != null)
			{
				my_polygon.GetLineByIndex(0).Calc(currentPoint);
			}
		}

		public static MyLine CreatePolygonLine(Point pos, Color color, int thickness, Canvas Paint_canvas, List<MyFigure> arr_figures)
		{
			// Создаём полигон, если его ещё нет
			if (my_polygon == null)
			{
				my_polygon = new MyPolygon(pos, color, thickness, Paint_canvas, arr_figures);

				// Добавляем первую линию полигона, только если её ещё нет на холсте
				var firstLine = my_polygon.GetLineByIndex(0).GetFigure();
				if (!Paint_canvas.Children.Contains(firstLine))
				{
					Paint_canvas.Children.Add(firstLine);
				}
			}

			// Создаём новую линию для полигона
			MyLine line = new MyLine(pos, color, thickness, Paint_canvas, arr_figures);
			my_polygon.AddLine(line);

			// Добавляем новую линию только если её ещё нет на холсте
			var newLine = my_polygon.GetLineByIndex(my_polygon.last_line).GetFigure();
			if (!Paint_canvas.Children.Contains(newLine))
			{
				Paint_canvas.Children.Add(newLine);
			}

			return line;
		}
		public static bool FinishPolygon(Color color, int thickness, Canvas Paint_canvas, List<MyFigure> arr_figures, Key key)
		{
			if (my_polygon != null && key == Key.Escape)
			{
				my_polygon.Make_Polygon(Paint_canvas, arr_figures, color, thickness);
				arr_figures.Add(my_polygon);
				my_polygon = null;
				return false;
			}
			return true;
		}
		public override int UndoAction(Canvas canvas, int cur_action_pos, List<MyFigureLibrary.Action> arr_actions)
		{
			this.RemoveFigure(canvas);
			cur_action_pos--;
			return cur_action_pos;
		}
		public override int RedoAction(Canvas canvas, int cur_action_pos, List<MyFigureLibrary.Action> arr_actions)
		{
			this.AddFigure(canvas);
			cur_action_pos++;
			return cur_action_pos;
		}
		public override bool AreEqualFigures(MyFigure fig1, MyFigure fig2)
		{
			if (fig1.GetType() != fig2.GetType()) return false;
			Polygon сfig1 = ((MyPolygon)fig1).GetFigure();
			Polygon сfig2 = ((MyPolygon)fig2).GetFigure();

			if (сfig1.Points.Count != сfig2.Points.Count) return false;

			for (int i = 0; i < сfig1.Points.Count - 1; i++)
			{
				if (сfig1.Points[i] != сfig2.Points[i]) return false;
			}
			return true;
		}
	}
}
