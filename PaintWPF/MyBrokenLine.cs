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

namespace PaintWPF
{
	public class MyBrokenLine : MyFigure
	{
		private Polyline polyline;
		public List<MyLine> arr_lines { get; set; }
		public List<Point> Points { get; set; } = new List<Point>();
		public int last_line { get; set; } = -1;

		private static MyBrokenLine broken_line = null;

		public MyBrokenLine()
		{
			arr_lines = new List<MyLine>();
		}
		public override void Calc(Point newPoint)
		{
			if (last_line >= 0)
			{
				arr_lines[last_line].line.X2 = newPoint.X;
				arr_lines[last_line].line.Y2 = newPoint.Y;
			}
		}
		public Polyline GetFigure()
		{
			if (polyline != null) return polyline;
			return null;
		}
		public void AddLine(MyLine new_line)
		{
			last_line++;
			arr_lines.Add(new_line);
		}

		public MyLine GetLineByIndex(int index)
		{
			return arr_lines[index];
		}

		public override void SetFillColor(Color color)
		{
			// Ломаная линия не заполняется
		}

		public override bool IsPointInside(Point point)
		{
			return false;
		}

		public override void RemoveFigure(Canvas canvas)
		{
			if (polyline != null)
				canvas.Children.Remove(polyline);
		}

		public override void AddFigure(Canvas canvas)
		{
			canvas.Children.Add(polyline);
		}
		public override void MouseMove(Point pos, Canvas Paint_canvas, List<MyFigure> arr_figures)
		{
			if (broken_line != null && broken_line.GetLineByIndex(broken_line.last_line) != null)
			{
				broken_line.GetLineByIndex(broken_line.last_line).Calc(pos);
			}
		}

		public void Make_Polyline(Canvas Paint_canvas, List<MyFigure> arr_figures, Color color, int thickness)
		{
			Points.Clear();
			for (int i = 0; i < arr_lines.Count; i++)
			{
				Points.Add(new Point(arr_lines[i].line.X1, arr_lines[i].line.Y1));
			}
			Points.Add(new Point(arr_lines[arr_lines.Count-1].line.X2, arr_lines[arr_lines.Count-1].line.Y2));

			foreach (var line in arr_lines)
			{
				Paint_canvas.Children.Remove(line.GetFigure());
				arr_figures.Remove(line);
			}

			arr_lines.Clear();

			polyline = new Polyline
			{
				Points = new PointCollection(Points),
				Stroke = new SolidColorBrush(color),
				StrokeThickness = thickness
			};

			Paint_canvas.Children.Add(polyline);
		}

		public static MyLine CreatingLine(Point startPoint, Color selectedColor, int thickness, Canvas Paint_canvas, List<MyFigure> arr_figures)
		{
			if (broken_line == null) broken_line = new MyBrokenLine();
			MyLine line = new MyLine(startPoint, selectedColor, thickness, Paint_canvas, arr_figures);
			broken_line.AddLine(line);
			if (!Paint_canvas.Children.Contains(line.GetFigure()))
			{
				Paint_canvas.Children.Add(line.GetFigure());
			}
			return line;
		}
		public static bool FinishBrokenLine(Color color, int thickness, Canvas Paint_canvas, List<MyFigure> arr_figures, Key key)
		{
			if (broken_line != null && key == Key.Escape)
			{
				broken_line.Make_Polyline(Paint_canvas, arr_figures, color, thickness);
				arr_figures.Add(broken_line);
				broken_line = null;  // Сбрасываем для новой линии
				return false;
			}
			return true;
		}
		public override int UndoAction(Canvas canvas, int cur_action_pos, List<Action> arr_actions)
		{
			this.RemoveFigure(canvas);
			cur_action_pos--;
			return cur_action_pos;
		}
		public override int RedoAction(Canvas canvas, int cur_action_pos, List<Action> arr_actions)
		{
			this.AddFigure(canvas);
			cur_action_pos++;
			return cur_action_pos;
		}

		public override bool AreEqualFigures(MyFigure fig1, MyFigure fig2)
		{
			if (fig1.GetType() != fig2.GetType()) return false;

			Polyline сfig1 = ((MyBrokenLine)fig1).GetFigure();
			Polyline сfig2 = ((MyBrokenLine)fig2).GetFigure();

			if (сfig1.Points.Count != сfig2.Points.Count) return false;

			for (int i = 0; i < сfig1.Points.Count - 1; i++)
			{
				if (сfig1.Points[i] != сfig2.Points[i]) return false;
			}
			return true;
		}
	}
}
