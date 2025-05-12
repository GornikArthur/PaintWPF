using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MyFigureLibrary;

namespace PaintWPF_Library
{
	public class MyTrapezoid : MyFigure
	{
		public override string Name => "FTrapezoid";
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public string StrokeColor { get; set; }
		public int StrokeThickness { get; set; }
		public string FillColor { get; set; } = "Transparent";

		[JsonIgnore]
		private Polygon polygon;

		public MyTrapezoid() { }

		public MyTrapezoid(Point startPoint, Color color, int thickness, Canvas Paint_canvas, List<MyFigure> arr_figures)
		{
			X = startPoint.X;
			Y = startPoint.Y;
			Width = 0;
			Height = 0;
			StrokeColor = color.ToString();
			StrokeThickness = thickness;

			InitializeTrapezoid();
			Paint_canvas.Children.Add(GetFigure());
			arr_figures.Add(this);
		}

		private void InitializeTrapezoid()
		{
			polygon = new Polygon
			{
				Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(StrokeColor),
				StrokeThickness = StrokeThickness,
				Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor)
			};
			polygon.Points = new PointCollection();
		}

		public override void Calc(Point newPoint)
		{
			X = Math.Min(X, newPoint.X);
			Y = Math.Min(Y, newPoint.Y);
			Width = Math.Abs(newPoint.X - X);
			Height = Math.Abs(newPoint.Y - Y);

			// Верхняя сторона короче нижней
			double delta = Width * 0.2;

			Point topLeft = new Point(X + delta, Y);
			Point topRight = new Point(X + Width - delta, Y);
			Point bottomRight = new Point(X + Width, Y + Height);
			Point bottomLeft = new Point(X, Y + Height);

			polygon.Points = new PointCollection { topLeft, topRight, bottomRight, bottomLeft };
		}

		public Polygon GetFigure()
		{
			if (polygon == null)
				InitializeTrapezoid();
			return polygon;
		}

		public override bool IsPointInside(Point point)
		{
			var geometry = new StreamGeometry();
			using (var context = geometry.Open())
			{
				context.BeginFigure(polygon.Points[0], true, true);
				context.PolyLineTo(polygon.Points.Skip(1).ToList(), true, true);
			}
			return geometry.FillContains(point);
		}

		public override void SetFillColor(Color color)
		{
			FillColor = color.ToString();
			if (polygon != null)
			{
				polygon.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor);
			}
		}

		public override void RemoveFigure(Canvas canvas)
		{
			canvas.Children.Remove(polygon);
		}

		public override void AddFigure(Canvas canvas)
		{
			if (polygon == null)
				InitializeTrapezoid();
			canvas.Children.Add(polygon);
		}

		public override void MouseMove(Point pos, Canvas Paint_canvas, List<MyFigure> arr_figures)
		{
			arr_figures[arr_figures.Count - 1].Calc(pos);
		}

		public override int UndoAction(Canvas canvas, int cur_action_pos, List<MyFigureLibrary.Action> arr_actions)
		{
			RemoveFigure(canvas);
			return --cur_action_pos;
		}

		public override int RedoAction(Canvas canvas, int cur_action_pos, List<MyFigureLibrary.Action> arr_actions)
		{
			AddFigure(canvas);
			return ++cur_action_pos;
		}

		public override bool AreEqualFigures(MyFigure fig1, MyFigure fig2)
		{
			if (fig1 is not MyTrapezoid t1 || fig2 is not MyTrapezoid t2)
				return false;

			return t1.X == t2.X && t1.Y == t2.Y && t1.Width == t2.Width && t1.Height == t2.Height;
		}
	}
}
