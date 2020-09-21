using HHECS.Bll;
using HHECS.Model.BllModel;
using HHECS.Model.Common;
using HHECS.Model.Controls;
using HHECS.Model.Entities;
using HHECS.Model.Enums;
using HHECS.View.Win;
using HHECS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HHECS.View.TaskInfo
{
    /// <summary>
    /// Frm_TaskInfo.xaml 的交互逻辑
    /// </summary>
    public partial class WinTaskInfo : HideCloseWindow
    {
        public PageInfoVM PageInfo { get; set; } = new PageInfoVM();

        List<Line> lines;
        List<Step> steps;
        List<SysDictData> stepTypes;
        List<StepStation> stepStations;
        List<Station> stations;
        public WinTaskInfo()
        {
            InitializeComponent();
            InitControl();
            //AppSession.BllService.CheckPermission(App.MenuOperations, new List<UIElement>() { btn_Delete, btn_Query, btn_SendToWCS, BtnAdd, BtnComplete });
            AppSession.BllService.CheckPermission(App.MenuOperations, new List<UIElement>() { Btn_Maintain, btn_Query, BtnComplete });
        }

        private void InitControl()
        {
            endTime.SelectedDate = DateTime.Now.AddDays(1);
            beginTime.SelectedDate = DateTime.Now.AddDays(-7);
            //任务状态
            var stepTraceStatus = CommonHelper.EnumListDic<StepTraceStatus>("");
            cbx_TaskStatusBegion.ItemsSource = stepTraceStatus;
            cbx_TaskStatusBegion.DisplayMemberPath = "Key";
            cbx_TaskStatusBegion.SelectedValuePath = "Value";
            cbx_TaskStatusBegion.SelectedValue = StepTraceStatus.设备请求下料.GetIndexInt();

            cbx_TaskStatusEnd.ItemsSource = stepTraceStatus;
            cbx_TaskStatusEnd.DisplayMemberPath = "Key";
            cbx_TaskStatusEnd.SelectedValuePath = "Value";
            cbx_TaskStatusEnd.SelectedValue = StepTraceStatus.下发桁车放货.GetIndexInt();

            //产线
            lines = AppSession.Dal.GetCommonModelByCondition<Line>("").Data;
            if (lines != null)
            {
                lines.Insert(0, new Line() { LineName = "" });
                cbx_Lines.ItemsSource = lines;
                cbx_Lines.DisplayMemberPath = "LineName";
                cbx_Lines.SelectedValuePath = "Id";
            }

            //工序表
            steps = AppSession.Dal.GetCommonModelByCondition<Step>("").Data;

            //工序类型
            stepTypes = AppSession.Dal.GetCommonModelByCondition<SysDictData>(" WHERE dictType = 'StepType'").Data;
            if (stepTypes != null)
            {
                stepTypes.Insert(0, new SysDictData() { DictLabel = "" });
                cbx_step.ItemsSource = stepTypes;
                cbx_step.DisplayMemberPath = "DictLabel";
                cbx_step.SelectedValuePath = "DictValue";

                cbx_nextStep.ItemsSource = stepTypes;
                cbx_nextStep.DisplayMemberPath = "DictLabel";
                cbx_nextStep.SelectedValuePath = "DictValue";
            }

            //工序类型工位
            stepStations = AppSession.Dal.GetCommonModelByCondition<StepStation>("").Data;

            //工位表
            stations = AppSession.Dal.GetCommonModelByCondition<Station>("").Data;
            stations.Insert(0, new Station() { Name = "" });

            this.DPMain.DataContext = PageInfo;
        }

        private void btn_Query_Click(object sender, RoutedEventArgs e)
        {
            Query();
        }

        private void Query()
        {
            string sql = "where 1=1 ";
            if (!String.IsNullOrWhiteSpace(txt_TaskNo.Text))
            {
                sql += $" and id ={txt_TaskNo.Text}";
            }
            var statusBegion = cbx_TaskStatusBegion.SelectedValue?.ToString();
            if (!String.IsNullOrWhiteSpace(statusBegion))
            {
                sql += $" and status >= {statusBegion} ";
            }
            var statusEnd = cbx_TaskStatusEnd.SelectedValue?.ToString();
            if (!String.IsNullOrWhiteSpace(statusEnd))
            {
                sql += $" and status <= {statusEnd} ";
            }
            var lindId = cbx_Lines.SelectedValue?.ToString();
            if (!String.IsNullOrWhiteSpace(lindId))
            {
                sql += $" and lineId= {lindId}";
            }
            //有站台id就不需要再去筛选工序
            var stationID = cbx_staiton.SelectedValue?.ToString();
            if (!String.IsNullOrWhiteSpace(stationID))
            {
                sql += $" and stationId ={stationID}";
            }
            else
            {
                var stepType = cbx_step.SelectedValue?.ToString();
                if (!String.IsNullOrWhiteSpace(stepType))
                {
                    var temp = steps.Where(t => t.StepType == stepType).Select(t => t.Id).ToArray();
                    if (temp.Length > 0)
                    {
                        sql += $"and stepId in ({string.Join(",", temp)})";
                    }
                }
            }
            //有下个站台id就不需要再去筛选下个工序
            var nextStaitonID = cbx_nextStaiton.SelectedValue?.ToString();
            if (!String.IsNullOrWhiteSpace(nextStaitonID))
            {
                sql += $" and nextStationId ={nextStaitonID}";
            }
            else
            {
                var nextStepType = cbx_nextStep.SelectedValue?.ToString();
                if (!String.IsNullOrWhiteSpace(nextStepType))
                {
                    var temp = steps.Where(t => t.StepType == nextStepType).Select(t => t.Id).ToArray();
                    sql += $" and nextStepId in ({string.Join(",", temp)})";
                }
            }
            if (beginTime.SelectedDate != null)
            {
                sql += $" and lineInTime >= '{beginTime}'";
            }
            if (endTime.SelectedDate != null)
            {
                sql += $" and lineInTime <= '{endTime}'";
            }
            BllResult<int> result = AppSession.Dal.GetCommonModelCount<StepTrace>(sql);
            if (result.Success)
            {
                PageInfo.TotalCount = result.Data;
                var result2 = AppSession.Dal.GetCommonModeByPageCondition<StepTrace>(PageInfo.PageIndex, PageInfo.PageSize, sql, " id desc ");
                if (!result2.Success)
                {
                    MessageBox.Show($"查询失败：{result2.Msg}");
                }
                else
                {
                    var tasks = result2.Data;
                    //var stepStation = AppSession.Dal.GetCommonModelByCondition<StepStation>("");
                    tasks.ForEach(t =>
                    {
                        t.StepIdVM = steps?.FirstOrDefault(a => a.Id == t.StepId);
                        t.StationIdVM = stations?.FirstOrDefault(a => a.Id == t.StationId);
                        t.NextStepIdVM = steps?.FirstOrDefault(a => a.Id == t.NextStepId);
                        t.NextStationIdVM = stations?.FirstOrDefault(a => a.Id == t.NextStationId);
                    });
                    dgv_1.ItemsSource = tasks;
                }
            }
            else
            {
                MessageBox.Show($"查询失败：{result.Msg}");
            }
        }

        private void btn_SendToWCS_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_1.SelectedItems == null || dgv_1.SelectedItems.Count == 0)
            {
                MessageBox.Show("未选中数据");
                return;
            }
            foreach (StepTrace item in dgv_1.SelectedItems)
            {
                BllResult a = AppSession.TaskService.SendTaskToWCS(item.Id.Value, App.User.UserCode);
                if (!a.Success)
                {
                    MessageBox.Show("下发出现问题，操作中止：" + a.Msg);
                    return;
                }
            }
            MessageBox.Show("下发成功");
            Query();
        }

        private void page_PageChanged(object sender, PageChangedEventArgs e)
        {
            PageInfo.PageIndex = e.CurrentPageIndex;
            Query();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            WinTaskAdd frm_TaskAdd = new WinTaskAdd();
            frm_TaskAdd.ShowDialog();
            btn_Query_Click(null, null);
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_1.SelectedItem == null)
            {
                MessageBox.Show("请选中一条任务后继续");
                return;
            }
            if (dgv_1.SelectedItems.Count > 1)
            {
                MessageBox.Show("每次只能选择一条");
                return;
            }
            var task = (StepTrace)dgv_1.SelectedItem;
            if (MessageBox.Show($"是否要强制完成任务：{task.Id}；这可能导致未知异常发生！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var result = AppSession.StepTraceService.StepTraceExceptionOver(task.Id.Value, App.User.UserCode, stations, steps);
                if (result.Success)
                {
                    MessageBox.Show($"完成任务{task.Id}成功");
                    Query();
                }
                else
                {
                    MessageBox.Show($"完成任务{task.Id}失败：{result.Msg}");
                }
            }
        }

        /// <summary>
        /// 任务维护
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMaintain_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_1.SelectedItem == null)
            {
                MessageBox.Show("请选中一条任务后继续");
                return;
            }
            if (dgv_1.SelectedItems.Count != 1)
            {
                MessageBox.Show("请只选中一条任务后继续");
                return;
            }
            //如果任务是完成状态原则上不能修改数据
            var res = AppSession.Dal.GetCommonModelByCondition<StepTrace>($"where id={((StepTrace)dgv_1.SelectedItem).Id.Value}");
            if (res.Success && res.Data[0].Status >= (int)StepTraceStatus.任务完成)
            {
                MessageBox.Show("任务已经完成，无法修改");
                return;
            }
            if (dgv_1.SelectedItem == null)
            {
                MessageBox.Show("请选中一条任务后继续");
                return;
            }
            else
            {
                WinTaskMaintain winTaskMaintain = new WinTaskMaintain(((StepTrace)dgv_1.SelectedItem).Id.Value);
                winTaskMaintain.ShowDialog();
                btn_Query_Click(null, null);
            }
        }

        private void cbx_Lines_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            cbx_staiton_Init();
            cbx_nextStaiton_Init();
        }

        private void cbx_step_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            cbx_staiton_Init();
        }

        private void cbx_nextStep_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            cbx_nextStaiton_Init();
        }

        private void cbx_staiton_Init()
        {
            var stepType = cbx_step.SelectedValue?.ToString();
            if (String.IsNullOrWhiteSpace(stepType))
            {
                cbx_staiton.ItemsSource = null;
                return;
            }
            var temp = stepStations.Where(t => t.StepType == stepType).Select(t => t.StationId).ToList();
            if (temp.Count > 1)
            {
                var itemsSource = stations.Where(t => temp.Contains(t.Id) || t.Name == "");
                if (int.TryParse(cbx_Lines.SelectedValue?.ToString(), out int line))
                {
                    itemsSource = itemsSource.Where(t => t.LineId == line || t.Name == "");
                }
                cbx_staiton.ItemsSource = itemsSource;
                cbx_staiton.DisplayMemberPath = "Name";
                cbx_staiton.SelectedValuePath = "Id";
            }
        }

        private void cbx_nextStaiton_Init()
        {
            var nextStepType = cbx_nextStep.SelectedValue?.ToString();
            if (String.IsNullOrWhiteSpace(nextStepType))
            {
                cbx_nextStaiton.ItemsSource = null;
                return;
            }
            var temp = stepStations.Where(t => t.StepType == nextStepType).Select(t => t.StationId).ToList();
            if (temp.Count > 1)
            {
                var itemsSource = stations.Where(t => temp.Contains(t.Id) || t.Name == "");
                if (int.TryParse(cbx_Lines.SelectedValue?.ToString(), out int line))
                {
                    itemsSource = itemsSource.Where(t => t.LineId == line || t.Name == "");
                }
                cbx_nextStaiton.ItemsSource = itemsSource;
                cbx_nextStaiton.DisplayMemberPath = "Name";
                cbx_nextStaiton.SelectedValuePath = "Id";
            }
        }


    }
}
