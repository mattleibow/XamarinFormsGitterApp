using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using GitterApp.Platform.Renderers;
using GitterApp.Views;

[assembly: ExportRenderer(typeof(SelectionViewCell), typeof(SelectionViewCellRenderer))]

namespace GitterApp.Platform.Renderers
{
	public class SelectionViewCellRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
			cell.SelectedBackgroundView = new UIView
			{
				BackgroundColor = ((SelectionViewCell)item).SelectionColor.ToUIColor()
			};
			return cell;
		}
	}
}
