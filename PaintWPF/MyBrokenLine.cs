using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintWPF
{
	internal class MyBrokenLine : MyFigure
	{
		public List<MyLine> arr_lines = new List<MyLine>();
		public int last_line = -1;
		public void Calc(Point newPoint)
		{
			arr_lines[last_line].line.X2 = newPoint.X;
			arr_lines[last_line].line.Y2 = newPoint.Y;
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
	}
}
