using System.Text.Json.Serialization;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Numerics;
using System.Windows.Navigation;

namespace PaintWPF
{
	public class MyRectangle : MyFigure
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public string StrokeColor { get; set; }
		public int StrokeThickness { get; set; }
		public string FillColor { get; set; } = "Transparent";

		[JsonIgnore] // Исключаем объект WPF, так как он не сериализуемый
		private Rectangle rect;

		public MyRectangle() { }

		public MyRectangle(Point startPoint, Color color, int thickness, Canvas Paint_canvas, List<MyFigure> arr_figures)
		{
			X = startPoint.X;
			Y = startPoint.Y;
			Width = 0;
			Height = 0;
			StrokeColor = color.ToString();
			StrokeThickness = thickness;
			InitializeRectangle();

			Paint_canvas.Children.Add(this.GetFigure());
			arr_figures.Add(this);
		}

		private void InitializeRectangle()
		{
			rect = new Rectangle
			{
				Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(StrokeColor),
				StrokeThickness = StrokeThickness,
				Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor),
				Width = Width,
				Height = Height

			};
			Canvas.SetLeft(rect, X);
			Canvas.SetTop(rect, Y);
		}

		public override void Calc(Point newPoint)
		{
			X = Math.Min(X, newPoint.X);
			Y = Math.Min(Y, newPoint.Y);
			Width = Math.Abs(newPoint.X - X);
			Height = Math.Abs(newPoint.Y - Y);

			if (rect != null)
			{
				Canvas.SetLeft(rect, X);
				Canvas.SetTop(rect, Y);
				rect.Width = Width;
				rect.Height = Height;
			}
		}

		public Rectangle GetFigure()
		{
			if (rect == null) InitializeRectangle();
			return rect;
		}

		public override bool IsPointInside(Point point)
		{
			return point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;
		}

		public override void SetFillColor(Color color)
		{
			FillColor = color.ToString();
			if (rect != null)
			{
				rect.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(FillColor);
			}
		}

		public override void RemoveFigure(Canvas canvas)
		{
			canvas.Children.Remove(rect);
		}

		public override void AddFigure(Canvas canvas)
		{
			if (rect == null) InitializeRectangle();
			canvas.Children.Add(rect);
		}
		public override void MouseMove(Point pos, Canvas Paint_canvas, List<MyFigure> arr_figures)
		{
			arr_figures[arr_figures.Count - 1].Calc(pos);
		}
		public override int UndoAction(Canvas canvas, int cur_action_pos, List<Action> arr_actions)
		{
			RemoveFigure(canvas);
			cur_action_pos--;
			return cur_action_pos;
		}
		public override int RedoAction(Canvas canvas, int cur_action_pos, List<Action> arr_actions)
		{
			AddFigure(canvas);
			cur_action_pos++;
			return cur_action_pos;
		}

		public override bool AreEqualFigures(MyFigure fig1, MyFigure fig2)
		{
			if (fig1.GetType() != fig2.GetType()) return false;
			System.Windows.Shapes.Rectangle сfig1 = ((MyRectangle)fig1).GetFigure();
			System.Windows.Shapes.Rectangle сfig2 = ((MyRectangle)fig2).GetFigure();

			if (сfig1.Width != сfig2.Width || сfig1.Height != сfig2.Height || Canvas.GetLeft(сfig1) != Canvas.GetLeft(сfig2) || Canvas.GetTop(сfig1) != Canvas.GetTop(сfig2)) return false;
			return true;
		}

	}
}
