//#define Test
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FlowtestEdit;
using FlowtestEdit.FlowtestInstance;

namespace CheckFlowtest
{
    public partial class Form1 : Form
    {
        private string file;
        private Flow flow = Flow.CreateFlow();

        public Form1()
        {

            InitializeComponent();
        }

        /// <summary>
        /// 路径加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = file;
            openFileDialog1.Filter = "apk files (*.xml)|*.xml";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            string strFile = openFileDialog1.FileName;
                            filePathtxt.Text = strFile;
                            file = strFile;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 流程检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            string errMesg;
            Item[] items = flow.ReturnItemList(file, out errMesg);

            dispalytxt.AppendText("***** Start ********" + "\r\n");

            dispalytxt.AppendText(errMesg + "\r\n");
            if (items != null)
            {
                CheckCount(items);

                CheckName(items);

                CheckLoop(items);

                CheckSameId(items);

                CheckMethodName(items);
            }

            dispalytxt.AppendText("****** End ********!" + "\r\n");
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
                            dispalytxt.AppendText("Item id=" + item.id.ToString() + "  compare spec 不匹配" + "\r\n");
#if Test
                            Console.WriteLine("id=" + item.id.ToString() + "  compare spec 不匹配");
#endif
                        }
                        else if (compardCount != Regex.Matches(item.property_enspecdescribe, @" ").Count)
                        {
                            dispalytxt.AppendText("Item id=" + item.id.ToString() + " compare enspecdescribe 不匹配" + "\r\n");
#if Test
                            Console.WriteLine("id=" + item.id.ToString() + " compare enspecdescribe 不匹配");
#endif
                        }
                        else if (compardCount != Regex.Matches(item.property_specenable, @" ").Count)
                        {
                            dispalytxt.AppendText("Item id=" + item.id.ToString() + "compare specenable 不匹配" + "\r\n");
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
        /// </summary>
        /// <param name="items"></param>
        private void CheckName(Item[] items)
        {
            foreach (var item in items)
            {
                if (!(item.property_name.IndexOf("name=") != -1))//name
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " name=" + item.property_name + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " name=" + item.property_name);
#endif

                }
                else if (!(item.property_spec.IndexOf("spec=") != -1))//spec
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " spec=" + item.property_spec + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " spec=" + item.property_spec);
#endif
                }
                else if (!(item.property_specdescribe.IndexOf("specdescribe=") != -1))//specdescribe
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " specdescribe=" + item.property_specdescribe + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " specdescribe=" + item.property_specdescribe);
#endif
                }
                else if (!(item.property_enspecdescribe.IndexOf("enspecdescribe=") != -1))//enspecdescribe
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " enspecdescribe=" + item.property_enspecdescribe + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " enspecdescribe=" + item.property_enspecdescribe);
#endif
                }
                else if (!(item.property_errcode.IndexOf("errcode=") != -1))//errcode
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " errcode=" + item.property_errcode + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " errcode=" + item.property_errcode);
#endif
                }
                else if (!(item.property_specprefix.IndexOf("specprefix=") != -1))//specprefix
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " specprefix=" + item.property_specprefix + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " specprefix=" + item.property_specprefix);
#endif
                }
                else if (!(item.property_specsuffix.IndexOf("specsuffix=") != -1))//specsuffix
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " specsuffix=" + item.property_specsuffix + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " specsuffix=" + item.property_specsuffix);
#endif
                }
                else if (!(item.property_switch_.IndexOf("switch=") != -1))//switch
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " switch=" + item.property_switch_ + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " switch=" + item.property_switch_);
#endif
                }
                else if (!(item.property_alarm.IndexOf("alarm=") != -1)) //alarm
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " alarm=" + item.property_alarm + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " alarm=" + item.property_alarm);
#endif
                }
                else if (!(item.property_disable.IndexOf("disable=") != -1))//disable
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " disable=" + item.property_disable + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " disable=" + item.property_disable);
#endif
                }
                else if (!(item.property_specenable.IndexOf("specenable=") != -1))//specenable
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " specenable=" + item.property_specenable + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " specenable=" + item.property_specenable);
#endif
                }
                else if (!(item.property_brother.IndexOf("brother=") != -1))//brother
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " brother=" + item.property_brother + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " brother=" + item.property_brother);
#endif
                }
                else if (!(item.property_timeout.IndexOf("timeout=") != -1))//timeout
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " timeout=" + item.property_timeout + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " timeout=" + item.property_timeout);
#endif
                }
                else if (!(item.property_editable.IndexOf("editable=") != -1))//editable
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " editable=" + item.property_editable + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " editable=" + item.property_editable);
#endif
                }
                else if (!(item.property_loop.IndexOf("loop=") != -1))//loop
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " loop=" + item.property_loop + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " loop=" + item.property_loop);
#endif
                }
                else if (!(item.property_hide.IndexOf("hide=") != -1))//hide
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " hide=" + item.property_hide + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " hide=" + item.property_hide);
#endif
                }
                else if (!(item.property_condition.IndexOf("condition=") != -1))//condition
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " condition=" + item.property_condition + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " condition=" + item.property_condition);
#endif
                }
                else if (!(item.property_depend.IndexOf("depend=") != -1))//depand
                {
                    dispalytxt.AppendText("CheckPropertyName erro Item id=" + item.id.ToString() + " depend=" + item.property_depend + "\r\n");
#if Test
                    Console.WriteLine("CheckName erro id=" + item.id.ToString() + " depend=" + item.property_depend);
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
                    if (method.name == null)//name
                    {
                        dispalytxt.AppendText("CheckMethodName erro Item id=" + item.id.ToString() + "   Method里的 name 属性存在异常" + "\r\n");
#if Test
                        Console.WriteLine("CheckMethodName erro id=" + item.id.ToString() + " name=" + method.name);
#endif

                    }
                    else if (method.action == null)
                    {
                        dispalytxt.AppendText("CheckMethodName erro Item id=" + item.id.ToString() + "   Method里的 action 属性存在异常" + "\r\n");
                    }
                    else if (method.bedepend == null)
                    {
                        dispalytxt.AppendText("CheckMethodName erro Item id=" + item.id.ToString() + "   Method里的 bedepend 属性存在异常" + "\r\n");
                    }
                    else if (method.compare == null)
                    {
                        dispalytxt.AppendText("CheckMethodName erro Item id=" + item.id.ToString() + "   Method里的 compare 属性存在异常" + "\r\n");
                    }
                    else if (method.depend == null)
                    {
                        dispalytxt.AppendText("CheckMethodName erro Item id=" + item.id.ToString() + "   Method里的 depend 属性存在异常" + "\r\n");
                    }
                    else if (method.disable == null)
                    {
                        dispalytxt.AppendText("CheckMethodName erro Item id=" + item.id.ToString() + "   Method里的 disable 属性存在异常" + "\r\n");
                    }
                    else if (method.parameters == null)
                    {
                        dispalytxt.AppendText("CheckMethodName erro Item id=" + item.id.ToString() + "   Method里的 parameters 属性存在异常" + "\r\n");
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
                    if (str_.Length==1&&str_[0]=="\"\"")
                    {
                        dispalytxt.AppendText("依赖项目存在问题 Item id=" + item.id + " depend=" + item.property_depend + "\r\n");
                        break;
                    }

                    foreach (var item_ in str_)
                    {
                        try//depand 和depand的情况特殊处理
                        {
                            if (Convert.ToInt32(item_) > item.id)
                            {
                                dispalytxt.AppendText("依赖项存在问题不能依赖后面的测试项目 Item id=" + item.id + " depend=" + item.property_depend + "\r\n");
#if Test
                            Console.WriteLine("依赖项存在问题不能依赖后面的测试项目 id=" + item.id+" depend="+item.property_depend);
#endif
                                flag = false;
                                //break;
                            }
                            else if (Convert.ToInt32(item_) == item.id)
                            {
                                dispalytxt.AppendText("不能依赖自己 存在死循环 Item id=" + item.id + " depend=" + item.property_depend + "\r\n");
#if Test
                            Console.WriteLine("不能依赖自己 存在死循环 id=" + item.id + " depend=" + item.property_depend);
#endif
                                flag = false;
                            }
                        }
                        catch (Exception e)
                        {

                            dispalytxt.AppendText(e.ToString()+"  Item id=" + item.id + " depend=" + item.property_depend + "\r\n");
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
                    dispalytxt.AppendText("id不是按顺序排列 Item id=" + item.id.ToString() + "\r\n");
#if Test
                    Console.WriteLine("id不是按循序排列 id=" + item.id.ToString());
#endif
                }
                i++;

            }


            return flag;



        }
    }
}
