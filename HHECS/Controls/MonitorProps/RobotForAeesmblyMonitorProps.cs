using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHECS.Controls.Model
{
    /// <summary>
    /// 组焊机器人监控属性Props
    /// </summary>
    public enum RobotForAeesmblyMonitorProps
    {
        /// <summary>
        /// 伺服准备就绪_1
        /// </summary>
        System_Ready,
        /// <summary>
        ///  程序运行中_1
        /// </summary>        
        Prg_running,
        /// <summary>
        /// 暂停中_1
        /// </summary>
        Prg_paused,

        /// <summary>
        /// 按下HOLD键暂停中_1
        /// </summary>
        Motion_held,

        /// <summary>
        /// 报警_1
        /// </summary>
        Fault,

        /// <summary>
        /// 安全位置1_1
        /// </summary>
        SafePos_1,
        /// <summary>
        /// 安全位置2_1
        /// </summary>
        SafePos_2,
        /// <summary>
        /// 安全位置3_1
        /// </summary>
        SafePos_3,
        /// <summary>
        /// 反馈的焊接电压_1
        /// </summary>
        V,
        /// <summary>
        /// 反馈的焊接电流_1
        /// </summary>
        I,
        /// <summary>
        /// 伺服准备就绪_2
        /// </summary>
        System_Ready_2,
        /// <summary>
        /// 程序运行中_2
        /// </summary>
        Prg_running_2,
        /// <summary>
        /// 暂停中_2
        /// </summary>
        Prg_paused_2,
        /// <summary>
        /// 按下HOLD键暂停中_2
        /// </summary>
        Motion_held_2,
        /// <summary>
        /// 报警_2
        /// </summary>
        Fault_2,
        /// <summary>
        /// 安全位置1_2
        /// </summary>
        SafePos_1_2,
        /// <summary>
        /// 安全位置2_2
        /// </summary>
        SafePos_2_2,
        /// <summary>
        ///  安全位置3_2
        /// </summary>
        SafePos_3_2,
        /// <summary>
        /// 反馈的焊接电压_2
        /// </summary>
        V_2,
        /// <summary>
        /// 反馈的焊接电流_2
        /// </summary>
        I_2,
        /// <summary>
        /// 启动成功
        /// </summary>
        Start_OK,
        /// <summary>
        /// 焊丝预警
        /// </summary>
        Wire,
        /// <summary>
        /// 错误
        /// </summary>
        Err,
        /// <summary>
        /// 准备完成
        /// </summary>
        Ready_OK,
        /// <summary>
        /// 下料准备完成
        /// </summary>
        Ready_Blank,
        /// <summary>
        /// 请求下料
        /// </summary>
        Request_Blank,
        /// <summary>
        /// 上料准备完成
        /// </summary>
        Ready_Load,
        /// <summary>
        /// 请求上料
        /// </summary>
        Request_Load,
        /// <summary>
        /// 工件型号
        /// </summary>
        TYPE_Feedback,
        /// <summary>
        /// 工件数量
        /// </summary>
        Num,
        /// <summary>
        /// 启动
        /// </summary>
        Start,
        /// <summary>
        /// 停止
        /// </summary>
        Stop,
        /// <summary>
        /// 复位
        /// </summary>
        Reset,
        /// <summary>
        /// 绗架允许上料
        /// </summary>
        Load_Ready,
        /// <summary>
        /// 上料工件型号
        /// </summary>
        TYPE,
        /// <summary>
        ///上料工件数量
        /// </summary>
        Number,
        /// <summary>
        /// 上料完成
        /// </summary>
        Load_Compelte,
        /// <summary>
        /// 工序跟踪ID
        /// </summary>
        Step_Trace_Id,
        /// <summary>
        /// 下料完成
        /// </summary>
        Blank_Complete,
        /// <summary>
        /// CompleteBlank_OK
        /// </summary>
        CompleteBlank_OK,
        /// <summary>
        /// 
        /// </summary>
        Task_OK,
    }
}
