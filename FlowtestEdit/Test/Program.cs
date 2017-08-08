#define Test

using FlowtestEdit;
using System;
using FlowtestEdit.FlowtestInstance;
using System.Text.RegularExpressions;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathFile = "Flowtest.xml";
            string errmesg;
            Program p = new Program();

            Flow flow = Flow.CreateFlow();

            Item[] items = flow.ReturnItemList(pathFile,out errmesg);

            Console.WriteLine(errmesg);
            if (items!=null)
            {
                p.CheckCount(items);

                p.CheckName(items);

                p.CheckLoop(items);

                p.CheckSameId(items);

                p.CheckMethodName(items);
            }
           



            Console.WriteLine("Test OK");
            Console.Read();
        }

        /// <summary>
        /// compare spec enspecdescribe  specenable 个数是否相等
        /// </summary>
        /// <param name="items"></param>
        private void CheckCount(Item[] items)
        {
            foreach (var item in items)
            {
                foreach (var method in item.methods)
                {
                    if (method.compare != "")
                    {
                        int compardCount = Regex.Matches(method.compare, @" ").Count;

                        if (compardCount != Regex.Matches(item.property_spec, @" ").Count)
                        {
#if Test
                            Console.WriteLine("id=" + item.id.ToString() + "  compare spec 不匹配");
#endif
                        }
                        else if (compardCount != Regex.Matches(item.property_enspecdescribe, @" ").Count)
                        {
#if Test
                            Console.WriteLine("id=" + item.id.ToString() + " compare enspecdescribe 不匹配");
#endif
                        }
                        else if (compardCount != Regex.Matches(item.property_specenable, @" ").Count)
                        {
#if Test
                            Console.WriteLine("id=" + item.id.ToString() + "compare specenable 不匹配");
#endif
                        }


                    }


                }

            }
        }

        /// <summary>
        /// 检测对应的属性名称是否正确
        /// 检测 propety属性
        /// </summary>
        /// <param name="items"></param>
        private void CheckName(Item[] items)
        {
            foreach (var item in items)
            {
                if (!(item.property_name.IndexOf("name=") != -1))//name
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " name=" + item.property_name);
#endif

                }
                else if (!(item.property_spec.IndexOf("spec=") != -1))//spec
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " spec=" + item.property_spec);
#endif
                }
                else if (!(item.property_specdescribe.IndexOf("specdescribe=") != -1))//specdescribe
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " specdescribe=" + item.property_specdescribe);
#endif
                }
                else if (!(item.property_enspecdescribe.IndexOf("enspecdescribe=") != -1))//enspecdescribe
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " enspecdescribe=" + item.property_enspecdescribe);
#endif
                }
                else if (!(item.property_errcode.IndexOf("errcode=") != -1))//errcode
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " errcode=" + item.property_errcode);
#endif
                }
                else if (!(item.property_specprefix.IndexOf("specprefix=") != -1))//specprefix
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " specprefix=" + item.property_specprefix);
#endif
                }
                else if (!(item.property_specsuffix.IndexOf("specsuffix=") != -1))//specsuffix
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " specsuffix=" + item.property_specsuffix);
#endif
                }
                else if (!(item.property_switch_.IndexOf("switch=") != -1))//switch
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " switch=" + item.property_switch_);
#endif
                }
                else if (!(item.property_alarm.IndexOf("alarm=") != -1)) //alarm
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " alarm=" + item.property_alarm);
#endif
                }
                else if (!(item.property_disable.IndexOf("disable=") != -1))//disable
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " disable=" + item.property_disable);
#endif
                }
                else if (!(item.property_specenable.IndexOf("specenable=") != -1))//specenable
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " specenable=" + item.property_specenable);
#endif
                }
                else if (!(item.property_brother.IndexOf("brother=") != -1))//brother
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " brother=" + item.property_brother);
#endif
                }
                else if (!(item.property_timeout.IndexOf("timeout=") != -1))//timeout
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " timeout=" + item.property_timeout);
#endif
                }
                else if (!(item.property_editable.IndexOf("editable=") != -1))//editable
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " editable=" + item.property_editable);
#endif
                }
                else if (!(item.property_loop.IndexOf("loop=") != -1))//loop
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " loop=" + item.property_loop);
#endif
                }
                else if (!(item.property_hide.IndexOf("hide=") != -1))//hide
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " hide=" + item.property_hide);
#endif
                }
                else if (!(item.property_condition.IndexOf("condition=") != -1))//condition
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " condition=" + item.property_condition);
#endif
                }
                else if (!(item.property_depend.IndexOf("depend=") != -1))//depand
                {
#if Test
                    Console.WriteLine("CheckPropertyName erro id=" + item.id.ToString() + " depend=" + item.property_depend);
#endif
                }



            }


        }

        /// <summary>
        /// 检测method 的属性  是否正确
        /// </summary>
        /// <param name="items"></param>
        private void CheckMethodName(Item[] items)
        {

            foreach (var item in items)
            {
                foreach (var method in item.methods)
                {
                    if (method.name ==null)//name
                    {
#if Test
                        Console.WriteLine("CheckMethodName erro id=" + item.id.ToString() + " name=" + method.name);
#endif

                    }


                }
            }



        }

        /// <summary>
        /// 检测是否存在死循环
        /// 检测 depend里包含id 号
        /// </summary>
        /// <param name="items"></param>
        private bool CheckLoop(Item[] items)
        {
            bool flag = true;
            foreach (var item in items)
            {
                if (item.property_depend != "depend=\"\"")
                {
                    string str = item.property_depend.Substring(item.property_depend.IndexOf("\"") + 1, (item.property_depend.LastIndexOf("\"") - item.property_depend.IndexOf("\"") - 1));

                    string[] str_ = str.Split(' ');

                    
                    foreach (var item_ in str_)
                    {
                        if (Convert.ToInt32(item_)>item.id)
                        {
#if Test
                            Console.WriteLine("依赖项存在问题不能依赖后面的测试项目 id=" + item.id+" depend="+item.property_depend);
#endif
                            flag = false;
                            //break;
                        }
                        else if (Convert.ToInt32(item_) == item.id)
                        {
#if Test
                            Console.WriteLine("不能依赖自己 存在死循环 id=" + item.id + " depend=" + item.property_depend);
#endif
                            flag = false;
                        }
                        
                    }
                    
                }
            }


            return flag;
        }

        /// <summary>
        /// id号必须是递增的
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private bool CheckSameId(Item[] items)
        {
            bool flag = true;
            int i = 1;
            foreach (var item in items)
            {
                if (item.id != i)
                {
#if Test
                    Console.WriteLine("id不是按顺序序排列 id=" + item.id.ToString());
#endif
                }
                i++;

            }


            return flag;



        }



    }
}
