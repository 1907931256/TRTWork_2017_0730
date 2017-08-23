using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationSys.Flow
{
    class Item
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
                return methodList_;
            }
        }

        public void AddMethod(Method method)
        {
            if (methodList_ == null)
            {
                methodList_ = new List<Method>();
            }
            methodList_.Add(method);
        }

        public Method GetMethod(int index)
        {
            if (methodList_ == null || methodList_.Count <= index)
            {
                return null;
            }

            return methodList_[index];
        }
    }
}
