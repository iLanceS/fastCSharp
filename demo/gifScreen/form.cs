using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using fastCSharp;
using fastCSharp.drawing.gif;

namespace fastCSharp.demo.gifScreen
{
    public partial class form : Form
    {
        /// <summary>
        /// GIF文件扩展名
        /// </summary>
        private const string gifFileName = ".gif";
        /// <summary>
        /// 坐标重置委托集合
        /// </summary>
        private fastCSharp.stateSearcher.ascii<Action> sets;
        /// <summary>
        /// 坐标移动委托集合
        /// </summary>
        private fastCSharp.stateSearcher.ascii<Action<int>> incs;
        /// <summary>
        /// 最后一次生成的GIF文件名
        /// </summary>
        private string currentFileName;
        /// <summary>
        /// GIF文件截屏
        /// </summary>
        private copyScreen copyScreen;
        public form()
        {
            InitializeComponent();

            string[] names = new string[] { "left", "top", "right", "bottom" };
            sets = new stateSearcher.ascii<Action>(names, names.getArray(name => (Action)Delegate.CreateDelegate(typeof(Action), this, "set_" + name)));
            incs = new stateSearcher.ascii<Action<int>>(names, names.getArray(name => (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), this, "inc_" + name)));
            foreach (string name in names) sets.Get(name)();
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onFormClose(object sender, FormClosedEventArgs e)
        {
            pub.Dispose(ref copyScreen);
            pub.Dispose(ref sets);
            pub.Dispose(ref incs);
        }
        /// <summary>
        /// 坐标设置按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onClickButton(object sender, EventArgs e)
        {
            string[] names = ((Control)sender).Name.Split('_');
            switch (names[0])
            {
                case "set":
                    for (int index = names.Length - 1; --index > 0; sets.Get(names[index])()) ;
                    break;
                case "inc":
                    incs.Get(names[1])(parse(incTextBox.Text, 1));
                    break;
                case "dec":
                    incs.Get(names[1])(-parse(incTextBox.Text, 1));
                    break;
            }
        }
        /// <summary>
        /// 坐标重置
        /// </summary>
        private void set_left()
        {
            leftTextBox.Text = "0";
        }
        /// <summary>
        /// 坐标重置
        /// </summary>
        private void set_top()
        {
            topTextBox.Text = "0";
        }
        /// <summary>
        /// 坐标重置
        /// </summary>
        private void set_right()
        {
            rightTextBox.Text = Screen.PrimaryScreen.WorkingArea.Right.toString();
        }
        /// <summary>
        /// 坐标重置
        /// </summary>
        private void set_bottom()
        {
            bottomTextBox.Text = Screen.PrimaryScreen.WorkingArea.Bottom.toString();
        }
        /// <summary>
        /// 修改坐标
        /// </summary>
        /// <param name="inc">偏移量</param>
        private void inc_left(int inc)
        {
            int left = parse(leftTextBox.Text, 0) + inc, right = parse(rightTextBox.Text, Screen.PrimaryScreen.WorkingArea.Right);
            if (left > right)
            {
                rightTextBox.Text = left.toString();
                left = right;
            }
            leftTextBox.Text = left.toString();
        }
        /// <summary>
        /// 修改坐标
        /// </summary>
        /// <param name="inc">偏移量</param>
        private void inc_right(int inc)
        {
            int left = parse(leftTextBox.Text, 0), right = parse(rightTextBox.Text, Screen.PrimaryScreen.WorkingArea.Right) + inc;
            if (left > right)
            {
                leftTextBox.Text = right.toString();
                right = left;
            }
            rightTextBox.Text = right.toString();
        }
        /// <summary>
        /// 修改坐标
        /// </summary>
        /// <param name="inc">偏移量</param>
        private void inc_top(int inc)
        {
            int top = parse(topTextBox.Text, 0) + inc, bottom = parse(bottomTextBox.Text, Screen.PrimaryScreen.WorkingArea.Bottom);
            if (top > bottom)
            {
                bottomTextBox.Text = top.toString();
                top = bottom;
            }
            topTextBox.Text = top.toString();
        }
        /// <summary>
        /// 修改坐标
        /// </summary>
        /// <param name="inc">偏移量</param>
        private void inc_bottom(int inc)
        {
            int top = parse(topTextBox.Text, 0), bottom = parse(bottomTextBox.Text, Screen.PrimaryScreen.WorkingArea.Bottom) + inc;
            if (top > bottom)
            {
                topTextBox.Text = bottom.toString();
                bottom = top;
            }
            bottomTextBox.Text = bottom.toString();
        }
        /// <summary>
        /// 字符串转换整数值
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="defaultValue">默认失败值</param>
        /// <returns>整数值</returns>
        private static int parse(string value, int defaultValue)
        {
            int returnValue;
            return int.TryParse(value, out returnValue) ? returnValue : defaultValue;
        }
        /// <summary>
        /// 开始截屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, EventArgs e)
        {
            int left = parse(leftTextBox.Text, 0), right = parse(rightTextBox.Text, Screen.PrimaryScreen.WorkingArea.Right);
            int top = parse(topTextBox.Text, 0), bottom = parse(bottomTextBox.Text, Screen.PrimaryScreen.WorkingArea.Bottom);
            if (left > right)
            {
                int temp = left;
                left = right;
                right = temp;
            }
            if (top > bottom)
            {
                int temp = top;
                top = bottom;
                bottom = temp;
            }
            leftTextBox.Text = left.toString();
            rightTextBox.Text = right.toString();
            topTextBox.Text = top.toString();
            bottomTextBox.Text = bottom.toString();
            if (left < right && top < bottom)
            {
                setEnabled(fileButton.Enabled = false);
                int frame = parse(frameTextBox.Text, 24);
                if (frame <= 0 || frame > 24) frame = 24;
                frameTextBox.Text = frame.toString();
                string fileName = fileNameTextBox.Text.Trim();
                if (fileName.Length == 0 || fileName == currentFileName) fileNameTextBox.Text = fileName = fastCSharp.io.file.BakPrefix + ((ulong)date.NowSecond.Ticks).toHex16() + ((uint)pub.Identity).toHex8();
                try
                {
                    copyScreen = new copyScreen(fileName + gifFileName, (double)1000 / frame, left, top, right - left, bottom - top);
                    currentFileName = fileName;
                    stopButton.Enabled = true;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.ToString());
                    setEnabled(true);
                    fileButton.Enabled = currentFileName != null;
                }
            }
        }
        /// <summary>
        /// 控件可用设置
        /// </summary>
        /// <param name="isEnabled"></param>
        private void setEnabled(bool isEnabled)
        {
            leftTextBox.Enabled = rightTextBox.Enabled = topTextBox.Enabled = bottomTextBox.Enabled = incTextBox.Enabled = fileNameTextBox.Enabled = frameTextBox.Enabled = isEnabled;
            set_bottom_button.Enabled = set_left_bottom_Button.Enabled = set_left_button.Enabled = set_left_right_top_bottom_button.Enabled = set_left_top_Button.Enabled = set_right_bottom_Button.Enabled = set_right_button.Enabled = set_right_top_Button.Enabled = set_top_button.Enabled = isEnabled;
            inc_bottom_button.Enabled = inc_left_button.Enabled = inc_right_button.Enabled = inc_top_button.Enabled = isEnabled;
            dec_bottom_button.Enabled = dec_left_button.Enabled = dec_right_button.Enabled = dec_top_button.Enabled = isEnabled;
            startButton.Enabled = isEnabled;
        }
        /// <summary>
        /// 停止截屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            copyScreen.Dispose();
            pub.Dispose(ref copyScreen);
            setEnabled(fileButton.Enabled = true);
        }
        /// <summary>
        /// 打开最后一次截屏的GIF文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileButton_Click(object sender, EventArgs e)
        {
            fastCSharp.diagnostics.process.StartNew(currentFileName + gifFileName);
        }
    }
}
