using ControlExs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;


namespace System.Windows.Forms
{
    class ButtonXin : Button
    {
        public ButtonXin()
        {
            //首先开启双缓冲，防止闪烁
            //双缓冲的一大堆设置 具体参数含义参照msdn的ControlStyles枚举值
            //this.SetStyle(ControlStyles.UserPaint, true);
            //this.SetStyle(ControlStyles.ResizeRedraw, true);
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.BackColor = Color.Blue;
            //this.BackgroundImage = Station.Properties.Resources.button3;
        }


        ////用来标示是否鼠标正在悬浮在按钮上  true:悬浮在按钮上 false:鼠标离开了按钮
        //private bool m_bMouseHover;
        ////用来标示是否鼠标点击了按钮  true：按下了按钮 false：松开了按钮
        //private bool m_bMouseDown;

        ////重载鼠标悬浮的事件
        //protected override void OnMouseEnter(EventArgs e)
        //{
        //    //当鼠标进入控件时，标示变量为进入了控件
        //    m_bMouseHover = true;
        //    //刷新面板触发OnPaint重绘
        //    this.Invalidate();
        //    base.OnMouseEnter(e);
        //}

        ////重载鼠标离开的事件
        //protected override void OnMouseLeave(EventArgs e)
        //{
        //    //当鼠标离开控件时，标示变量为离开了控件
        //    m_bMouseHover = false;
        //    //刷新面板触发OnPaint重绘
        //    this.Invalidate();
        //    base.OnMouseLeave(e);
        //}

        ////重载鼠标按下的事件
        //protected override void OnMouseDown(MouseEventArgs mevent)
        //{
        //    //当鼠标按下控件时，标示变量为按下了控件
        //    m_bMouseDown = true;
        //    //刷新面板触发OnPaint重绘
        //    this.Invalidate();
        //    base.OnMouseDown(mevent);
        //}

        ////重载鼠标松开的事件
        //protected override void OnMouseUp(MouseEventArgs mevent)
        //{
        //    //当鼠标松开时，标示变量为按下并松开了控件
        //    m_bMouseDown = false;
        //    //刷新面板触发OnPaint重绘
        //    this.Invalidate();
        //    base.OnMouseUp(mevent);
        //}

        ////重写button类的重绘事件
        //protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        //{

        //    base.OnPaint(e);
        
        //    if (!this.Enabled)
        //    {
        //        this.BackgroundImage = Station.Properties.Resources.button3;
        //    }
        //    else if (m_bMouseDown)
        //    {
        //        this.BackgroundImage = Station.Properties.Resources.button2;
        //    }
        //    else if (m_bMouseHover)
        //    {
        //        this.BackgroundImage = Station.Properties.Resources.button3;
        //    }
        //    else if (this.Focused)
        //    {
        //        this.BackgroundImage = Station.Properties.Resources.button3;
        //    }
        //    #region
        //    // if (!this.Enabled) bmpDraw = Station.Properties.Resources.button2;
        //    // else if (m_bMouseDown) bmpDraw = Station.Properties.Resources.button1;
        //    // //else if (m_bMouseHover) bmpDraw = Properties.Resources.QBtn_High;
        //    ////  如果禁用了，则使用禁用时的样式图片绘制，否则调用其他满足条件的样式图片绘制
        //    // if (!this.Enabled) bmpDraw = Properties.Resources.Qbtn_Gray;
        //    // else if (m_bMouseDown) bmpDraw = Properties.Resources.QBtn_Down;
        //    // else if (m_bMouseHover) bmpDraw = Properties.Resources.QBtn_High;
        //    // else if (this.Focused) bmpDraw = Properties.Resources.QBtn_Focus;


        //    //判断使用什么资源图
        //    //Image bmpDraw = Station.Properties.Resources.button1;
        //    //if (!this.Enabled) bmpDraw = Station.Properties.Resources.button2;
        //    //else if (m_bMouseDown) bmpDraw = Station.Properties.Resources.button1;
        //    ////else if (m_bMouseHover) bmpDraw = Properties.Resources.QBtn_High;
        //    // 如果禁用了，则使用禁用时的样式图片绘制，否则调用其他满足条件的样式图片绘制
        //    //if (!this.Enabled) bmpDraw = Properties.Resources.Qbtn_Gray;
        //    //else if (m_bMouseDown) bmpDraw = Properties.Resources.QBtn_Down;
        //    //else if (m_bMouseHover) bmpDraw = Properties.Resources.QBtn_High;
        //    //else if (this.Focused) bmpDraw = Properties.Resources.QBtn_Focus;
        //    ////绘制背景(若不知道这句啥意思 参照九宫切图里面的代码)
        //    //Graphics g = this.CreateGraphics();
        //    //RenderHelper.DrawImageWithNineRect(g, bmpDraw, this.ClientRectangle, rec);
        //    ////int radious=20;
        //    //RenderHelper.GraphicsPath(rec,radious);




        //}




        ////重载鼠标按下的事件
        ////protected override void OnMouseDown(MouseEventArgs mevent)
        ////{
        ////    //当鼠标按下控件时，标示变量为按下了控件
        ////    m_bMouseDown = true;
        ////    //刷新面板触发OnPaint重绘
        ////    this.Invalidate();
        ////    base.OnMouseDown(mevent);
        ////}
        ////protected override void OnMouseEnter(EventArgs e)
        ////{
        ////    Graphics g = this.CreateGraphics();
        ////    g.DrawEllipse(new Pen(Color.Red), 0, 0, this.Width, this.Height);
        ////    g.Dispose();
        ////}
        ///// <summary>
        ///// 按钮设置成背景透明
        ///// </summary>
        ///// <param name="btn"></param>
        ////public static void SetBtnStyle(Button btn)
        ////{

        ////    btn.FlatStyle = FlatStyle.Flat;//样式  
        ////    btn.ForeColor = Color.Transparent;//前景  
        ////    btn.BackColor = Color.Transparent;//去背景  
        ////    btn.FlatAppearance.BorderSize = 0;//去边线  
        ////    btn.FlatAppearance.MouseOverBackColor = Color.Transparent;//鼠标经过  
        ////    btn.FlatAppearance.MouseDownBackColor = Color.Transparent;//鼠标按下  
        ////}
        //    #endregion

    }

    public class TouMing_btn : Button
    {

        //public static void SetBtnStyle(Button btn)
        //{

        //    btn.FlatStyle = FlatStyle.Flat;//样式  
        //    btn.ForeColor = Color.Transparent;//前景  
        //    btn.BackColor = Color.Transparent;//去背景  
        //    btn.FlatAppearance.BorderSize = 0;//去边线  
        
        //}



    }


}
