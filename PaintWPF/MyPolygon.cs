using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintWPF
{
	internal class MyPolygon : MyFigure
	{
		public List<MyLine> arr_lines = new List<MyLine>();
		public int last_line = 0;

		public MyPolygon(Point startPoint)
		{
			MyLine new_line = new MyLine(startPoint);
			arr_lines.Add(new_line);
		}
		public void Calc(Point newPoint)
		{
			arr_lines[last_line].line.X2 = newPoint.X;
			arr_lines[last_line].line.Y2 = newPoint.Y;
		}
		public MyLine GetLineByIndex(int index)
		{
			return arr_lines[index];
		}
		public void AddLine(MyLine new_line)
		{
			last_line++;
			arr_lines.Add(new_line);
		}
	}
}
