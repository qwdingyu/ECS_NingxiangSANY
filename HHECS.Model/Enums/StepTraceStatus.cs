using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHECS.Model.Enums
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum StepTraceStatus
    {
        任务创建 = 1,
        设备开始生产 = 3,
        设备请求下料 = 5,
        //桁车允许下料 = 6,
        //响应设备下料请求 = 7,
        等待桁车执行 = 10,
        下发桁车取货 = 20,//（开始执行“取货”性质任务，去目标库位取出工件）
        响应桁车取货完成 = 25,//（“取货”性质任务完成，此时这个任务对应的工件在桁车上）
        下发桁车放货 = 85,//（此时桁车带着工件去目标库位）
        响应桁车放货完成 = 90,	//（此时桁车已经将工件放入了目标站台）
        任务完成 = 100, //任务完成
        //任务回传失败 = 110, //任务回传失败
        //任务回传成功 = 120, //任务回传成功
        
        异常结束 = 130,//空出与取货错为异常结束
    }
}
