using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintWPF
{
	internal class MyLine : MyFigure
	{
		public Line line;

		public MyLine(Point startPoint)
		{
			line = new Line
			{
				Stroke = Brushes.Black,
				StrokeThickness = 2,
				X1 = startPoint.X,
				Y1 = startPoint.Y,
				X2 = startPoint.X,
				Y2 = startPoint.Y
			};
		}

		public void Calc(Point newPoint)
		{
			line.X2 = newPoint.X;
			line.Y2 = newPoint.Y;
		}

		public Line GetFigure()
		{
			return line;
		}
	}
}
