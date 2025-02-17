using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintWPF
{
	internal class MyEllipse : MyFigure
	{
		private Ellipse ellip;
		private int start_x, start_y;

		public MyEllipse(Point startPoint)
		{
			ellip = new Ellipse
			{
				Stroke = Brushes.Black,
				StrokeThickness = 2,
			};
			start_x = (int)startPoint.X;
			start_y = (int)startPoint.Y;
			Canvas.SetLeft(ellip, startPoint.X);
			Canvas.SetTop(ellip, startPoint.Y);
		}

		public void Calc(Point newPoint)
		{
			Canvas.SetLeft(ellip, Math.Min(this.start_x, newPoint.X));
			Canvas.SetTop(ellip, Math.Min(this.start_y, newPoint.Y));
			ellip.Width = Math.Abs(newPoint.X - this.start_x);
			ellip.Height = Math.Abs(newPoint.Y - this.start_y);
		}

		public Ellipse GetFigure()
		{
			return ellip;
		}
	}
}
