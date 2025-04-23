using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintWPF
{
	public class MyLine : MyFigure
	{
		public double X1 { get; set; }
		public double Y1 { get; set; }
		public double X2 { get; set; }
		public double Y2 { get; set; }

		public string StrokeColor { get; set; }
		public int StrokeThickness { get; set; }

		[JsonIgnore] // 🔹 Исключаем `line` из сериализации, т.к. его можно восстановить
		public Line line;

		// 🔹 Конструктор без параметров для JSON-десериализации
		public MyLine() { }

		public MyLine(Point startPoint, Color color, int thickness, Canvas Paint_canvas, List<MyFigure> arr_figures)
		{
			X1 = startPoint.X;
			Y1 = startPoint.Y;
			X2 = startPoint.X;
			Y2 = startPoint.Y;
			StrokeColor = color.ToString(); // Сохраняем цвет в строковом формате
			StrokeThickness = thickness;
			InitializeLine(); // Создаём фигуру

			Paint_canvas.Children.Add(this.GetFigure());
			arr_figures.Add(this);
		}

		// 🔹 Метод для создания линии на основе свойств
		private void InitializeLine()
		{
			line = new Line
			{
				X1 = X1,
				Y1 = Y1,
				X2 = X2,
				Y2 = Y2,
				StrokeThickness = StrokeThickness,
				Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(StrokeColor) // Восстанавливаем цвет
			};
		}

		public override void Calc(Point newPoint)
		{
			X2 = newPoint.X;
			Y2 = newPoint.Y;
			if (line != null)
			{
				line.X2 = X2;
				line.Y2 = Y2;
			}
		}

		public Line GetFigure()
		{
			if (line == null) InitializeLine(); // Если `line` не был создан, создаём его
			return line;
		}

		public override bool IsPointInside(Point point)
		{
			// Можно добавить проверку, например, на расстояние до линии
			return false;
		}

		public override void SetFillColor(Color color)
		{
			// Линии не поддерживают заливку
		}

		public override void RemoveFigure(Canvas canvas)
		{
			canvas.Children.Remove(line);
		}

		public override void AddFigure(Canvas canvas)
		{
			if (line == null) InitializeLine(); // Гарантируем, что объект создан
			canvas.Children.Add(line);
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
			Line сfig1 = ((MyLine)fig1).GetFigure();
			Line сfig2 = ((MyLine)fig2).GetFigure();

			if (сfig1.X1 != сfig2.X1 || сfig1.X2 != сfig2.X2 || сfig1.Y1 != сfig2.Y1 || сfig1.Y2 != сfig2.Y2) return false;
			return true;
		}
	}
}
