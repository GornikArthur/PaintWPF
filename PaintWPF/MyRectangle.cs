using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace PaintWPF
{
	internal class MyRectangle : MyFigure
	{
		private Rectangle rect;
		private int start_x, start_y;

		public MyRectangle(Point startPoint)
		{
			rect = new Rectangle
			{
				Stroke = Brushes.Black,
				StrokeThickness = 2,
			};
			start_x = (int)startPoint.X;
			start_y = (int)startPoint.Y;
			Canvas.SetLeft(rect, startPoint.X);
			Canvas.SetTop(rect, startPoint.Y);
		}

		public void Calc(Point newPoint)
		{
			Canvas.SetLeft(rect, Math.Min(this.start_x, newPoint.X));
			Canvas.SetTop(rect, Math.Min(this.start_y, newPoint.Y));
			rect.Width = Math.Abs(newPoint.X - this.start_x);
			rect.Height = Math.Abs(newPoint.Y - this.start_y);
		}

		public Rectangle GetFigure()
		{
			return rect;
		}
	}
}
