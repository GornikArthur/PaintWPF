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
	[JsonDerivedType(typeof(MyLine), typeDiscriminator: "Line")]
	[JsonDerivedType(typeof(MyRectangle), typeDiscriminator: "Rectangle")]
	[JsonDerivedType(typeof(MyEllipse), typeDiscriminator: "Ellipse")]
	[JsonDerivedType(typeof(MyBrokenLine), typeDiscriminator: "BrokenLine")]
	[JsonDerivedType(typeof(MyPolygon), typeDiscriminator: "Polygon")]
	public abstract class MyFigure : Action
	{
		string Name { get; }
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
