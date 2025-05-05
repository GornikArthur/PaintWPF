using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PaintWPF
{
	public abstract class Action
	{
		public abstract int UndoAction(Canvas canvas, int cur_action_pos, List<Action> arr_actions);
		public abstract int RedoAction(Canvas canvas, int cur_action_pos, List<Action> arr_actions);
	}
}
