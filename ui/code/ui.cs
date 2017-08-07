using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using fastCSharp.reflection;
using System.Diagnostics;

namespace fastCSharp.code
{
    /// <summary>
    /// 安装界面
    /// </summary>
    internal partial class ui : Form
    {
        /// <summary>
        /// 安装控件容器名称
        /// </summary>
        public const string SetupControlName = "Setup";
        /// <summary>
        /// 当前程序集
        /// </summary>
        public static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
        /// <summary>
        /// 安装界面
        /// </summary>
        public ui()
        {
            InitializeComponent();
            Text = config.code.Default.SetupTitle + " " + pub.fastCSharp + "安装界面";
            projectPath.Text = pub.ApplicationPath;
            createSetup();
        }
        /// <summary>
        /// 生成创建安装类型按钮
        /// </summary>
        private void createSetup()
        {
            FlowLayoutPanel createTypePanel = new FlowLayoutPanel();
            createTypePanel.Dock = DockStyle.Fill;
            createTypePanel.Name = SetupControlName;
            createTypePanel.AutoSize = true;
            this.type.Controls.Add(createTypePanel);

            keyValue<Type, auto>[] autos = CurrentAssembly.GetTypes()
                .getFind(type => !type.IsInterface && !type.IsAbstract && typeof(IAuto).IsAssignableFrom(type))
                .GetArray(type => new keyValue<Type, auto>(type, type.customAttribute<auto>() ?? auto.NullAuto));
            foreach (keyValue<Type, auto> auto in autos)
            {
                CheckBox createTypeCheckBox = new CheckBox();
                createTypeCheckBox.AutoSize = true;
                createTypeCheckBox.Name = auto.Key.FullName;
                createTypeCheckBox.Text = auto.Value.ShowName(auto.Key);
                createTypePanel.Controls.Add(createTypeCheckBox);
            }
        }
        /// <summary>
        /// 选择安装目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFolderButton_Click(object sender, EventArgs e)
        {
            openFolder.SelectedPath = projectPath.Text;
            openFolder.ShowDialog();
            if (openFolder.SelectedPath != null && openFolder.SelectedPath.Length != 0) projectPath.Text = openFolder.SelectedPath;
        }
        /// <summary>
        /// 选择安装文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileButton_Click(object sender, EventArgs e)
        {
            openFile.FileName = assemblyPath.Text;
            openFile.ShowDialog();
            if (openFile.FileName != null && openFile.FileName.Length != 0)
            {
                assemblyPath.Text = openFile.FileName;
                if (openFile.FileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || openFile.FileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    FileInfo file = new FileInfo(openFile.FileName);
                    projectName.Text = defaultNamespace.Text = file.Name.Substring(0, file.Name.Length - 4);
                    if (projectName.Text.StartsWith(config.code.Default.BaseNamespace + ".", StringComparison.Ordinal))
                    {
                        projectName.Text = projectName.Text.Substring(config.code.Default.BaseNamespace.Length + 1);
                    }
                    projectPath.Text = file.DirectoryName;
                    int pathIndex = (projectPath.Text + @"\").LastIndexOf(@"\bin\");
                    if (++pathIndex != 0) projectPath.Text = projectPath.Text.Substring(0, pathIndex);
                }
            }
        }
        /// <summary>
        /// 安装参数
        /// </summary>
        private auto.parameter autoParameter
        {
            get
            {
                return new auto.parameter(projectName.Text, projectPath.Text, assemblyPath.Text, defaultNamespace.Text, isFastCSharp.Checked);
            }
        }
        /// <summary>
        /// 安装
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setup_Click(object sender, EventArgs e)
        {
            try
            {
                keyValue<Type, auto>[] autos = this.type.Controls[SetupControlName].Controls.toGeneric<Control>()
                    .getFind(control => ((CheckBox)control).Checked)
                    .GetArray(control => CurrentAssembly.GetType(control.Name))
                    .getArray(type => new keyValue<Type, auto>(type, type.customAttribute<code.auto>() ?? auto.NullAuto));
                Setup(autos, autoParameter, checkConfig.Checked);
            }
            catch (Exception error)
            {
                code.error.Add(error);
            }
            code.error.Open(false);
            if (!code.error.IsError) MessageBox.Show("安装完毕");
            code.error.Clear();
        }
        /// <summary>
        /// 安装
        /// </summary>
        /// <param name="autos">安装类型属性</param>
        /// <param name="parameter">安装参数</param>
        /// <returns>安装是否成功</returns>
        internal static bool Setup(keyValue<Type, auto>[] autos, auto.parameter parameter, bool isConfig)
        {
            bool isSetup = true;
            if (autos != null)
            {
                try
                {
                    autos = autos.sort((left, right) => string.CompareOrdinal(left.Key.FullName, right.Key.FullName));
                    HashSet<Type> types = autos.getHash(value => value.Key);
                    keyValue<Type, Type>[] depends = autos
                        .getFind(value => value.Value.DependType != null && types.Contains(value.Value.DependType))
                        .GetArray(value => new keyValue<Type, Type>(value.Key, value.Value.DependType));
                    foreach (Type type in algorithm.topologySort.Sort(depends, types, true))
                    {
                        //Stopwatch time = new Stopwatch();
                        //time.Start();
                        if (!(CurrentAssembly.CreateInstance(type.FullName) as IAuto)
                            .Run(isConfig ? config.pub.LoadConfig(parameter.Copy(), type.ToString()) : parameter))
                        {
                            error.Add(type.fullName() + " 安装失败");
                            isSetup = false;
                        }
                        //time.Stop();
                        //error.Message(parameter.ProjectName + " " + type.FullName + " : " + time.ElapsedMilliseconds.ToString() + "ms");
                    }
#if TESTCASE
                    new test().Run(isConfig ? config.pub.LoadConfig(parameter.Copy(), typeof(test).ToString()) : parameter);
#endif
                }
                catch (Exception exception)
                {
                    error.Add(exception);
                }
                finally
                {
                    coder.Output(parameter);
                }
            }
            return isSetup;
        }
        /// <summary>
        /// 显示界面
        /// </summary>
        public static void ShowSetup()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ui());
        }
    }
}
