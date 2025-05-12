using System.Windows.Controls;

namespace MyFigureLibrary
{
	public abstract class Action
	{
		public abstract int UndoAction(Canvas canvas, int cur_action_pos, List<Action> arr_actions);
		public abstract int RedoAction(Canvas canvas, int cur_action_pos, List<Action> arr_actions);
	}
}
