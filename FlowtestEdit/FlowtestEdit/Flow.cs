
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
        private Item[] ItemList;

        public static Flow CreateFlow()
        {
            if (flowtIntest != null)
            {
                return flowtIntest;
            }
            else
            {
                return flowtIntest = new Flow();
            }
        }

        /// <summary>
        /// 返回item集合
        /// </summary>
        /// <returns></returns>
        public Item[] ReturnItemList(string pathFile,out string errMessing)
        {
            try
            {
                ItemList = (
            from e in XDocument.Load(pathFile).Root.Elements("Item")
            select new Item
            {
                id = (int)e.Attribute("id"),

                property_name = (from p in e.Elements("Property").Attributes()
                                  select p.ToString()
                                 ).ToArray()[0],
                 property_spec = (from p in e.Elements("Property").Attributes()
                                  select p.ToString()
                                 ).ToArray()[1],
                 property_specdescribe = (from p in e.Elements("Property").Attributes()
                                          select p.ToString()
                                 ).ToArray()[2],
                 property_enspecdescribe = (from p in e.Elements("Property").Attributes()
                                            select p.ToString()
                                 ).ToArray()[3],
                 property_errcode = (from p in e.Elements("Property").Attributes()
                                     select p.ToString()
                                 ).ToArray()[4],
                 property_specprefix = (from p in e.Elements("Property").Attributes()
                                        select p.ToString()
                                 ).ToArray()[5],
                 property_specsuffix = (from p in e.Elements("Property").Attributes()
                                        select p.ToString()
                                 ).ToArray()[6],
                 property_switch_ = (from p in e.Elements("Property").Attributes()
                                     select p.ToString()
                                 ).ToArray()[7],
                 property_alarm = (from p in e.Elements("Property").Attributes()
                                   select p.ToString()
                                 ).ToArray()[8],
                 property_disable = (from p in e.Elements("Property").Attributes()
                                     select p.ToString()
                                 ).ToArray()[9],
                 property_specenable = (from p in e.Elements("Property").Attributes()
                                        select p.ToString()
                                 ).ToArray()[10],
                 property_brother = (from p in e.Elements("Property").Attributes()
                                     select p.ToString()
                                 ).ToArray()[11],
                 property_timeout = (from p in e.Elements("Property").Attributes()
                                     select p.ToString()
                                 ).ToArray()[12],
                 property_editable = (from p in e.Elements("Property").Attributes()
                                      select p.ToString()
                                 ).ToArray()[13],
                 property_loop = (from p in e.Elements("Property").Attributes()
                                  select p.ToString()
                                 ).ToArray()[14],
                 property_hide = (from p in e.Elements("Property").Attributes()
                                  select p.ToString()
                                 ).ToArray()[15],
                 property_condition = (from p in e.Elements("Property").Attributes()
                                       select p.ToString()
                                 ).ToArray()[16],
                 property_depend = (from p in e.Elements("Property").Attributes()
                                    select p.ToString()
                                 ).ToArray()[17],

                 methods = (
                             from m in e.Elements("Method")
                             select new Method
                             {
                                 name = (string)m.Attribute("name"),
                                 action = (string)m.Attribute("action"),
                                 parameters = (string)m.Attribute("parameters"),
                                 compare = (string)m.Attribute("compare"),
                                 disable = (string)m.Attribute("disable"),
                                 bedepend = (string)m.Attribute("bedepend"),
                                 depend = (string)m.Attribute("depend")

                             }).ToArray(),

             }).ToArray();

             errMessing = null;
             return ItemList;
            }
          
            catch( System.IndexOutOfRangeException range)
            {
                errMessing = "Property 属性个数不对,属性个数少于标定！";
            }
            catch (System.Exception e)
            {
                errMessing = e.ToString();
                System.Console.WriteLine(e.ToString());
                //throw e;
            }


            return ItemList;

        }




        private DataSet CreatDataSet()
        {
            DataSet ds = new DataSet();
            flowtIntest.CreateFlowtestTables(ds);

            return ds;
        }

        private void CreateFlowtestTables(DataSet ds)
        {
            //创建一个Item表
            DataTable itemsTable = new DataTable("Items");

            itemsTable.Columns.Add("ItemId", typeof(int));
            itemsTable.Columns.Add("property_name", typeof(string));
            itemsTable.Columns.Add("property_spec", typeof(string));
            itemsTable.Columns.Add("property_specdescribe", typeof(string));
            itemsTable.Columns.Add("property_enspecdescribe", typeof(string));
            itemsTable.Columns.Add("property_errcode", typeof(string));
            itemsTable.Columns.Add("property_specprefix", typeof(string));
            itemsTable.Columns.Add("property_specsuffix", typeof(string));
            itemsTable.Columns.Add("property_switch_", typeof(string));
            itemsTable.Columns.Add("property_alarm", typeof(string));
            itemsTable.Columns.Add("property_disable", typeof(string));
            itemsTable.Columns.Add("property_specenable", typeof(string));
            itemsTable.Columns.Add("property_brother", typeof(string));
            itemsTable.Columns.Add("property_timeout", typeof(string));
            itemsTable.Columns.Add("property_editable", typeof(string));
            itemsTable.Columns.Add("property_loop", typeof(string));
            itemsTable.Columns.Add("property_hide", typeof(string));
            itemsTable.Columns.Add("property_condition", typeof(string));
            itemsTable.Columns.Add("property_depend", typeof(string));
            ds.Tables.Add(itemsTable);

            DataTable methods = new DataTable("Methods");

            methods.Columns.Add("ItemId", typeof(int));
            methods.Columns.Add("MethodName", typeof(string));
            methods.Columns.Add("Action", typeof(string));
            methods.Columns.Add("parameters", typeof(string));
            methods.Columns.Add("compare", typeof(string));
            methods.Columns.Add("disable", typeof(string));
            methods.Columns.Add("bedepend", typeof(string));
            methods.Columns.Add("depend", typeof(string));

            ds.Tables.Add(methods);

            //转换2个之间的父子关系
            DataRelation co = new DataRelation("ItemMethods", itemsTable.Columns["ItemId"], methods.Columns["ItemId"]);
            ds.Relations.Add(co);






            var ItemList = (
                from e in XDocument.Load("Flowtest.xml").Root.Elements("Item")
                select new Item
                {
                    id = (int)e.Attribute("id"),

                    property_name = (from p in e.Elements("Property")
                                     select p.ToString()
                                    ).ToArray()[0],
                    property_spec = (from p in e.Elements("Property")
                                     select p.ToString()
                                    ).ToArray()[1],
                    property_specdescribe = (from p in e.Elements("Property")
                                             select p.ToString()
                                    ).ToArray()[2],
                    property_enspecdescribe = (from p in e.Elements("Property")
                                               select p.ToString()
                                    ).ToArray()[3],
                    property_errcode = (from p in e.Elements("Property")
                                        select p.ToString()
                                    ).ToArray()[4],
                    property_specprefix = (from p in e.Elements("Property")
                                           select p.ToString()
                                    ).ToArray()[5],
                    property_specsuffix = (from p in e.Elements("Property")
                                           select p.ToString()
                                    ).ToArray()[6],
                    property_switch_ = (from p in e.Elements("Property")
                                        select p.ToString()
                                    ).ToArray()[7],
                    property_alarm = (from p in e.Elements("Property")
                                      select p.ToString()
                                    ).ToArray()[8],
                    property_disable = (from p in e.Elements("Property")
                                        select p.ToString()
                                    ).ToArray()[9],
                    property_specenable = (from p in e.Elements("Property")
                                           select p.ToString()
                                    ).ToArray()[10],
                    property_brother = (from p in e.Elements("Property")
                                        select p.ToString()
                                    ).ToArray()[11],
                    property_timeout = (from p in e.Elements("Property")
                                        select p.ToString()
                                    ).ToArray()[12],
                    property_editable = (from p in e.Elements("Property")
                                         select p.ToString()
                                    ).ToArray()[13],
                    property_loop = (from p in e.Elements("Property")
                                     select p.ToString()
                                    ).ToArray()[14],
                    property_hide = (from p in e.Elements("Property")
                                     select p.ToString()
                                    ).ToArray()[15],
                    property_condition = (from p in e.Elements("Property")
                                          select p.ToString()
                                    ).ToArray()[16],
                    property_depend = (from p in e.Elements("Property")
                                       select p.ToString()
                                    ).ToArray()[17],

                    methods = (
                                from m in e.Elements("Method")
                                select new Method
                                {
                                    name = (string)m.Attribute("name"),
                                    action = (string)m.Attribute("action"),
                                    parameters = (string)m.Attribute("parameters"),
                                    compare = (string)m.Attribute("compare"),
                                    disable = (string)m.Attribute("disable"),
                                    bedepend = (string)m.Attribute("bedepend"),
                                    depend = (string)m.Attribute("depend")

                                }).ToArray(),

                }).ToList();


            foreach (Item item in ItemList)
            {
                itemsTable.Rows.Add(new object[] {item.id,item.property_name,item.property_spec,item.property_specdescribe,
                    item.property_enspecdescribe,item.property_errcode,item.property_specprefix,item.property_specsuffix,
                    item.property_switch_,
                    item.property_alarm,item.property_disable,item.property_specenable,item.property_brother,item.property_timeout,
                    item.property_editable,
                    item.property_loop,item.property_hide,item.property_condition,item.property_depend
                });
                foreach (Method method in item.methods)
                {
                    methods.Rows.Add(new object[] { item.id, method.name, method.action, method.parameters, method.compare, method.disable, method.bedepend, method.depend });
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
                    name=x.Field<string>("property_name"),
                    spec=x.Field<string>("property_spec"),


                    methods =
                    from o in x.GetChildRows("ItemMethods")
                    group o by x.GetChildRows("ItemMethods") into methods
                    select new
                    {
                        names = from m in methods
                                select new { name = m.Field<string>("MethodName") },

                        actions = from o in methods
                                  select new { action = o.Field<string>("Action") }
                    }

                };

            foreach (var item in itemsGroup)
            {
                System.Console.WriteLine(item.ItemId.ToString());
                System.Console.WriteLine(item.name);
                System.Console.WriteLine(item.spec);

                foreach (var m in item.methods)
                {

                    foreach (var x in m.names)
                    {
                        System.Console.WriteLine(x.name);
                    }
                    foreach (var y in m.actions)
                    {
                        System.Console.WriteLine(y.action);
                    }
                }
            }
        }





    }
}
