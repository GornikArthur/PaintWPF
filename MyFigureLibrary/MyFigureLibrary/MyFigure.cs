using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyFigureLibrary
{
	public abstract class MyFigure : Action
	{
		public abstract string Name { get; }
		public abstract void Calc(Point newPoint);
		public abstract bool IsPointInside(Point point);
		public abstract void SetFillColor(Color color);
		public abstract void RemoveFigure(Canvas canvas);
		public abstract void AddFigure(Canvas canvas);
		public abstract bool AreEqualFigures(MyFigure fig1, MyFigure fig2);
		public abstract void MouseMove(Point pos, Canvas Paint_canvas, List<MyFigure> arr_figures);
		public virtual void CustomMouseMove(Point currentPoint) { }

	}
}
