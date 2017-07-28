
using System.Data;
using System.Linq;
using FlowtestEdit.FlowtestInstance;
using System.Xml.Linq;

namespace FlowtestEdit
{
    /*
     * 流程编辑工具
     * 
     * 加载流程
     * 
     * 
     * */
    public class Flow
    {
        private static Flow flowtIntest;

        public static Flow CreateFlow()
        {
            if (flowtIntest!=null)
            {
                return flowtIntest;
            }
            else
            {
              return flowtIntest = new Flow();
            }
        }
       
            
            
        private DataSet CreatDataSet()
        {
            DataSet ds = new DataSet();
            flowtIntest.CreateFlowtestTables(ds);

            return ds;
        }

        private void CreateFlowtestTables(DataSet ds)
        {
            DataTable items = new DataTable("Items");
            items.Columns.Add("ItemId", typeof(int));

            ds.Tables.Add(items);

            DataTable methods = new DataTable("Methods");

            methods.Columns.Add("ItemId", typeof(int));
            methods.Columns.Add("MethodId", typeof(string));
            methods.Columns.Add("MethodName", typeof(string));
            methods.Columns.Add("Action", typeof(string));
            //methods.Columns.Add("parameters", typeof(string));
            //methods.Columns.Add("compare", typeof(string));
            //methods.Columns.Add("disable", typeof(string));
            //methods.Columns.Add("bedepend", typeof(string));
            //methods.Columns.Add("depend", typeof(string));

            ds.Tables.Add(methods);

            //DataTable propertys = new DataTable("Propertys");

            //propertys.Columns.Add("ItemId", typeof(string));
            //propertys.Columns.Add("PropertyName", typeof(string));
            //propertys.Columns.Add("spec", typeof(string));
            //propertys.Columns.Add("specdescribe", typeof(string));
            //propertys.Columns.Add("enspecdescribe", typeof(string));
            //propertys.Columns.Add("errcode", typeof(string));
            //propertys.Columns.Add("specprefix", typeof(string));
            //propertys.Columns.Add("specsuffix", typeof(string));
            //propertys.Columns.Add("Switch", typeof(string));
            //propertys.Columns.Add("alarm", typeof(string));
            //propertys.Columns.Add("disable", typeof(string));
            //propertys.Columns.Add("brother", typeof(string));
            //propertys.Columns.Add("timeout", typeof(string));
            //propertys.Columns.Add("editable", typeof(string));
            //propertys.Columns.Add("loop", typeof(string));
            //propertys.Columns.Add("hide", typeof(string));
            //propertys.Columns.Add("condition", typeof(string));
            //propertys.Columns.Add("depend", typeof(string));

            //ds.Tables.Add(items);

            //转换2个之间的父子关系
            DataRelation co = new DataRelation("ItemMethods", items.Columns["ItemId"], methods.Columns["ItemId"]);
            ds.Relations.Add(co);

            //co = new DataRelation("ItemPropertys", items.Columns["ItemId"], propertys.Columns["ItemId"]);
            //ds.Relations.Add(co);

            var ItemList = (
                from e in XDocument.Load("Flowtest.xml").Root.Elements("Item")
                select new Item
                {
                    //CustomerID = (string)e.Element("id"),
                    id = (int)e.Attribute("id"),
                    methods = (

                                from m in e.Elements("Method")
                                select new Method
                                {
                                    name = (string)m.Element("name"),
                                    action = (string)m.Element("action"),
                                    //parameters = (string)m.Element("parameters"),
                                    //compare = (string)m.Element("compare"),
                                    //disable = (int)m.Element("disable"),
                                    //bedepend = (int)m.Element("bedepend"),
                                    //depend = (int)m.Element("depend")
                                }).ToArray(),




                }).ToList();

            foreach (Item item in ItemList)
            {
                items.Rows.Add(new object[] { item.id });
                foreach (Method method in item.methods)
                {
                    methods.Rows.Add(new object[] { method.name, method.action });
                }
            }



        }


        public void DataTest()
        {
            var items = flowtIntest.CreatDataSet().Tables["Items"].AsEnumerable();

            var itemsGroup =
                from x in items
                select
                new
                {
                    ItemId = x.Field<int>("ItemId"),
                };

            foreach (var item in itemsGroup)
            {
                System.Console.WriteLine(item.ItemId);
            }
        }

    }
}
