using System;
using System.Collections.Generic;

namespace IntegrationSys.Flow
{
	internal class Item
	{
		private List<Method> methodList_;

		public int Id
		{
			get;
			set;
		}

		public Property Property
		{
			get;
			set;
		}

		public List<Method> MethodList
		{
			get
			{
				return this.methodList_;
			}
		}

		public void AddMethod(Method method)
		{
			if (this.methodList_ == null)
			{
				this.methodList_ = new List<Method>();
			}
			this.methodList_.Add(method);
		}

		public Method GetMethod(int index)
		{
			if (this.methodList_ == null || this.methodList_.Count <= index)
			{
				return null;
			}
			return this.methodList_[index];
		}
	}
}
