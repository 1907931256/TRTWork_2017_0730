using System;

namespace IntegrationSys.Flow
{
	internal class ExecuteFinishEventArgs : EventArgs
	{
		private FlowItem flowItem_;

		public FlowItem FlowItem
		{
			get
			{
				return this.flowItem_;
			}
		}

		public ExecuteFinishEventArgs(FlowItem flowItem)
		{
			this.flowItem_ = flowItem;
		}
	}
}
