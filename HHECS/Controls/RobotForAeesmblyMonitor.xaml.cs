using HHECS.Controls.Model;
using HHECS.Controls.MonitorProps;
using HHECS.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HHECS.Controls
{
    /// <summary>
    /// RobotForAeesmblyMonitor.xaml 的交互逻辑
    /// 组焊机器人监控界面
    /// </summary>
    public partial class RobotForAeesmblyMonitor : UserControl
    {
        public Equipment Self { get; set; }
        public string ControlName
        {
            get { return (string)GetValue(ControlNameProperty); }
            set { SetValue(ControlNameProperty, value); }
        }

        public static readonly DependencyProperty ControlNameProperty =
            DependencyProperty.Register("ControlName", typeof(string), typeof(RobotForAeesmblyMonitor), new PropertyMetadata(""));
        public RobotForAeesmblyMonitor(int maxW, int maxH)
        {
            InitializeComponent();
            this.Width = maxW;
            this.Height = maxH;
            txt_RobotForAeesmblyName.SetBinding(TextBlock.TextProperty, new Binding("ControlName") { Source = this });
        }

        /// <summary>
        /// 赋值属性RobotForAeesmbly
        /// </summary>
        /// <param name="RobotForAeesmbly"></param>
        public void SetRobotForAeesmblyProps(Equipment RobotForAeesmbly)
        {
            Self = RobotForAeesmbly;
            #region 赋值属性

            //程序运行中
            var Prg_running = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Prg_running.ToString());
            if (Prg_running != null)
            {
                this.txt_Prg_running.Foreground = Brushes.Blue;
                switch (Prg_running.Value)
                {
                    case "True":
                        txt_Prg_running.Text = "True";
                        break;
                    case "False":
                        txt_Prg_running.Text = "False";
                        break;
                    default:
                        txt_Prg_running.Text = "未知";
                        txt_Prg_running.Foreground = Brushes.Red;
                        break;
                }
            }

            var Start_OK = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Start_OK.ToString());
            if (Start_OK != null)
            {
                this.txt_Start_OK.Foreground = Brushes.Blue;
                switch (Start_OK.Value)
                {
                    case "True":
                        txt_Start_OK.Text = "True";
                        break;
                    case "False":
                        txt_Start_OK.Text = "False";
                        break;
                    default:
                        txt_Start_OK.Text = "未知";
                        txt_Start_OK.Foreground = Brushes.Red;
                        break;
                }

            }

            //准备完成
            var Ready_OK = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Ready_OK.ToString());
            if (Ready_OK != null)
            {
                this.txt_Ready_OK.Foreground = Brushes.Blue;
                switch (Ready_OK.Value)
                {
                    case "True":
                        txt_Ready_OK.Text = "True";
                        break;
                    case "False":
                        txt_Ready_OK.Text = "False";
                        txt_Ready_OK.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_Ready_OK.Text = "未知";
                        txt_Ready_OK.Foreground = Brushes.Red;
                        break;
                }
            }

            //上料准备完成
            var Ready_Load = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Ready_Load.ToString());
            if (Ready_Load != null)
            {
                this.txt_Ready_Load.Foreground = Brushes.Blue;
                switch (Ready_Load.Value)
                {
                    case "True":
                        txt_Ready_Load.Text = "True";
                        break;
                    case "False":
                        txt_Ready_Load.Text = "False";
                        txt_Ready_Load.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_Ready_Load.Text = "未知";
                        txt_Ready_Load.Foreground = Brushes.Red;
                        break;
                }
            }
            //绗架允许上料
            var Load_Ready = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Load_Ready.ToString());
            if (Load_Ready != null)
            {
                txt_Load_Ready.Foreground = Brushes.Blue;
                switch (Load_Ready.Value)
                {
                    case "True":
                        txt_Load_Ready.Text = "True";
                        break;
                    case "False":
                        txt_Load_Ready.Text = "False";
                        txt_Load_Ready.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_Load_Ready.Text = "未知";
                        txt_Load_Ready.Foreground = Brushes.Red;
                        break;
                }
            }
            //请求上料
            var Request_Load = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Request_Load.ToString());
            if (Request_Load != null)
            {
                this.txt_Request_Load.Foreground = Brushes.Blue;
                switch (Request_Load.Value)
                {
                    case "True":
                        txt_Request_Load.Text = "True";
                        break;
                    case "False":
                        txt_Request_Load.Text = "False";
                        txt_Request_Load.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_Request_Load.Text = "未知";
                        txt_Request_Load.Foreground = Brushes.Red;
                        break;
                }
            }

            //上料完成
            var Load_Compelte = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Load_Compelte.ToString());
            if (Load_Compelte != null)
            {
                txt_Load_Compelte.Foreground = Brushes.Blue;
                switch (Load_Compelte.Value)
                {
                    case "True":
                        txt_Load_Compelte.Text = "True";
                        break;
                    case "False":
                        txt_Load_Compelte.Text = "False";
                        break;
                    default:
                        txt_Load_Compelte.Text = "未知";
                        txt_Load_Compelte.Foreground = Brushes.Red;
                        break;
                }
            }

            //下料准备完成
            var Ready_Blank = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Ready_Blank.ToString());
            if (Ready_Blank != null)
            {
                this.txt_Ready_Blank.Foreground = Brushes.Blue;
                switch (Ready_Blank.Value)
                {
                    case "True":
                        txt_Ready_Blank.Text = "True";
                        break;
                    case "False":
                        txt_Ready_Blank.Text = "False";
                        txt_Ready_Blank.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_Ready_Blank.Text = "未知";
                        txt_Ready_Blank.Foreground = Brushes.Red;
                        break;
                }
            }

            //请求下料
            var Request_Blank = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Request_Blank.ToString());
            if (Request_Blank != null)
            {
                this.txt_Request_Blank.Foreground = Brushes.Blue;
                switch (Request_Blank.Value)
                {
                    case "True":
                        txt_Request_Blank.Text = "True";
                        break;
                    case "False":
                        txt_Request_Blank.Text = "False";
                        txt_Request_Blank.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_Request_Blank.Text = "未知";
                        txt_Request_Blank.Foreground = Brushes.Red;
                        break;
                }
            }

            //HOLD暂停中
            var Motion_held = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Motion_held.ToString());
            if (Motion_held != null)
            {
                this.txt_Motion_held.Foreground = Brushes.Blue;
                switch (Motion_held.Value)
                {
                    case "True":
                        txt_Motion_held.Text = "True";
                        break;
                    case "False":
                        txt_Motion_held.Text = "False";
                        break;
                    default:
                        txt_Motion_held.Text = "未知";
                        txt_Motion_held.Foreground = Brushes.Red;
                        break;
                }
            }
            //任务号
            var Step_Trace_Id = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Step_Trace_Id.ToString());
            if (Step_Trace_Id != null)
            {
                this.txt_Step_Trace_Id.Foreground = Brushes.Blue;
                this.txt_Step_Trace_Id.Text = Step_Trace_Id.Value;
            }
            //类型
            var TYPE_Feedback = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.TYPE_Feedback.ToString());
            if (TYPE_Feedback != null)
            {
                this.txt_TYPE_Feedback.Foreground = Brushes.Blue;
                this.txt_TYPE_Feedback.Text = TYPE_Feedback.Value;
            }
            //暂停中
            var Prg_paused = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Prg_paused.ToString());
            if (Prg_paused != null)
            {
                this.txt_Prg_paused.Foreground = Brushes.Blue;
                switch (Prg_paused.Value)
                {
                    case "True":
                        txt_Prg_paused.Text = "True";
                        break;
                    case "False":
                        txt_Prg_paused.Text = "False";
                        break;
                    default:
                        txt_Prg_paused.Text = "未知";
                        txt_Prg_paused.Foreground = Brushes.Red;
                        break;
                }
            }
            //报警1
            var Fault = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Fault.ToString());
            if (Fault != null)
            {
                this.txt_Fault.Foreground = Brushes.Blue;
                switch (Fault.Value)
                {
                    case "True":
                        txt_Fault.Text = "True";
                        break;
                    case "False":
                        txt_Fault.Text = "False";
                        break;
                    default:
                        txt_Fault.Text = "未知";
                        txt_Fault.Foreground = Brushes.Red;
                        break;
                }
            }
            //错误
            var Err = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Err.ToString());
            if (Err != null)
            {
                this.txt_Err.Foreground = Brushes.Blue;
                switch (Err.Value)
                {
                    case "True":
                        txt_Err.Text = "True";
                        break;
                    case "False":
                        txt_Err.Text = "False";
                        txt_Err.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_Err.Text = "未知";
                        txt_Err.Foreground = Brushes.Red;
                        break;
                }
            }

            var Blank_Complete = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Blank_Complete.ToString());
            if (Blank_Complete != null)
            {
                this.txt_Blank_Complete.Foreground = Brushes.Blue;
                switch (Blank_Complete.Value)
                {
                    case "True":
                        txt_Blank_Complete.Text = "True";
                        break;
                    case "False":
                        txt_Blank_Complete.Text = "False";
                        txt_Blank_Complete.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_Blank_Complete.Text = "未知";
                        txt_Blank_Complete.Foreground = Brushes.Red;
                        break;
                }
            }

            var CompleteBlank_OK = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.CompleteBlank_OK.ToString());
            if (CompleteBlank_OK != null)
            {
                this.txt_CompleteBlank_OK.Foreground = Brushes.Blue;
                switch (CompleteBlank_OK.Value)
                {
                    case "True":
                        txt_CompleteBlank_OK.Text = "True";
                        break;
                    case "False":
                        txt_CompleteBlank_OK.Text = "False";
                        txt_CompleteBlank_OK.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_CompleteBlank_OK.Text = "未知";
                        txt_CompleteBlank_OK.Foreground = Brushes.Red;
                        break;
                }
            }

            var Number = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Number.ToString());
            if (Number != null)
            {
                this.txt_Number.Foreground = Brushes.Blue;
                switch (Number.Value)
                {
                    case "True":
                        txt_Number.Text = "True";
                        break;
                    case "False":
                        txt_Number.Text = "False";
                        txt_Number.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_Number.Text = "未知";
                        txt_Number.Foreground = Brushes.Red;
                        break;
                }
            }


            var Task_OK = Self.EquipmentProps.FirstOrDefault(t => t.EquipmentTypeTemplateCode == RobotForAeesmblyMonitorProps.Task_OK.ToString());
            if (Task_OK != null)
            {
                this.txt_Task_OK.Foreground = Brushes.Blue;
                switch (Task_OK.Value)
                {
                    case "True":
                        txt_Task_OK.Text = "True";
                        break;
                    case "False":
                        txt_Task_OK.Text = "False";
                        txt_Task_OK.Foreground = Brushes.Red;
                        break;
                    default:
                        txt_Task_OK.Text = "未知";
                        txt_Task_OK.Foreground = Brushes.Red;
                        break;
                }
            }
            #endregion
        }
    }
}

