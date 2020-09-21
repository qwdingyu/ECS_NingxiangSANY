using HHECS.Bll;
using HHECS.EquipmentExcute.Robot.Enums;
using HHECS.Model.BllModel;
using HHECS.Model.Common;
using HHECS.Model.Entities;
using HHECS.Model.Enums;
using HHECS.Model.PLCHelper.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace HHECS.EquipmentExcute.Robot
{
    /// <summary>
    /// 机器人顶层抽象类
    /// </summary>
    public abstract class RobotExcute
    {
        /// <summary>
        /// 用于标记设备的类型
        /// </summary>
        public EquipmentType EquipmentType { get; set; }

        /// <summary>
        /// 用于可用存储设备列表
        /// </summary>
        public List<Equipment> Equipments { get; set; }

        /// <summary>
        /// 具体的设备实现逻辑，
        /// 1.上料：机器人先上料准备完成（短信号，ECS回复后消除），ECS回复允许，然后机器人发送上料请求（长信号不消除）
        /// 2.下料：机器人先下料准备完成（短信号，ECS回复后消除），ECS回复允许，然后机器人发送下料请求（长信号不消除）
        /// 3.上料必须用请求，因为用准备完成信号，一旦回复就消失了，如果又把工件放到其他地方去了，这个工位就会一直无法使用，因为再也不会准备完成。
        /// 4.下料可以用准备完成，也能用请求，但是ECS不回复准备完成，机器人是没有请求的，容易卡死。机器人还容易出现反复准备完成的情况，最终改用请求信号。
        /// </summary>
        /// <param name="robots"></param>
        /// <param name="plcs"></param>
        /// <returns></returns>
        public virtual BllResult Excute(List<Equipment> robots, List<Equipment> allEquipments, IPLC plc)
        {
            try
            {
                if (robots.Count == 0)
                {
                    return BllResultFactory.Error($"没有【{this.EquipmentType.Name}】类型的设备，所以不执行处理程序。");
                }
                //找出 未完成的工序任务
                var stepTraceResult = AppSession.Dal.GetCommonModelByConditionWithZero<StepTrace>($"where status < {StepTraceStatus.任务完成.GetIndexInt()}");
                if (!stepTraceResult.Success)
                {
                    Logger.Log($"查询【{this.EquipmentType.Name}】类型的设备的任务出错，原因：{stepTraceResult.Msg}", LogLevel.Error);
                    return BllResultFactory.Error();
                }
                foreach (var robot in robots)
                {
                    //机器人准备完成，才能处理，这个准备完成 是无故障，且在原点。
                    var Ready_OK = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.Ready_OK.ToString());
                    if (Ready_OK.Value == true.ToString())
                    {
                        //处理 上料准备完成
                        var Ready_Load = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.Ready_Load.ToString());
                        var Load_Ready = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.Load_Ready.ToString());
                        if (Ready_Load != null && Load_Ready != null)
                        {
                            //PLC有"上料准备完成"信号，但ECS没有确认信号，则ECS还没有响应
                            if (Ready_Load.Value == "True" && Load_Ready.Value == "False")
                            {
                                ExcuteLoadReady(robot, allEquipments, stepTraceResult.Data, plc);
                            }
                            //PLC有"上料准备完成"信号，但ECS有确认信号， 说明PLC已经清除而WCS没有清除
                            if (Ready_Load.Value == "False" && Load_Ready.Value == "True")
                            {
                                SendLoadReadyToPlc(false, robot, plc);
                            }
                        }

                        // 处理 下料准备完成
                        var Ready_Blank = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.Ready_Blank.ToString());
                        var Blank_Ready = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.Blank_Ready.ToString());
                        if (Ready_Blank != null && Blank_Ready != null)
                        {
                            //PLC有"下料准备完成"信号，但ECS没有确认信号，则ECS还没有响应
                            if (Ready_Blank.Value == "True" && Blank_Ready.Value == "False")
                            {
                                ExcuteBlankReady(robot, allEquipments, stepTraceResult.Data, plc);
                            }
                            //PLC没有"下料准备完成"信号，但ECS有确认信号， 说明PLC已经清除而WCS没有清除
                            if (Ready_Blank.Value == "False" && Blank_Ready.Value == "True")
                            {
                                SendBlankReadyToPlc(false, robot, plc);
                            }
                        }

                        //处理 上料完成
                        var Task_OK = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.Task_OK.ToString());
                        var WCS_Allow_Load = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.WCS_Allow_Load.ToString());
                        if (Task_OK != null && WCS_Allow_Load != null)
                        {
                            //PLC有"上料完成"信号，但ECS没有确认信号，则ECS还没有响应
                            if (Task_OK.Value == "True" && WCS_Allow_Load.Value == "False")
                            {
                                ExcuteArrive(robot, allEquipments, stepTraceResult.Data, plc);
                            }
                            //PLC有"上料完成"信号，但ECS有确认信号， 说明PLC已经清除而WCS没有清除
                            if (Task_OK.Value == "False" && WCS_Allow_Load.Value == "True")
                            {
                                SendArriveToPlc(false, robot, plc);
                            }
                        }

                        //处理 下料
                        var Request_Blank = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.Request_Blank.ToString());
                        if (Request_Blank != null)
                        {
                            //PLC有"下料请求"信号，但ECS没有确认信号，则ECS还没有响应（下料请求不用清除，等工件被取走后，机器人自己清除）
                            if (Request_Blank.Value == "True")
                            {
                                ExcuteRequest(robot, allEquipments, stepTraceResult.Data, plc);
                            }
                        }

                        //处理 翻转
                        var Allow_Flip = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.Allow_Flip.ToString());
                        var WCS_Allow_Flip = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.WCS_Allow_Flip.ToString());
                        if (Allow_Flip != null && WCS_Allow_Flip != null)
                        {
                            // PLC有"翻转请求"信号，但ECS没有确认信号，则ECS还没有响应
                            if (Allow_Flip.Value == "True" && WCS_Allow_Flip.Value == "False")
                            {
                                AllowFlip(robot, allEquipments, stepTraceResult.Data, plc);
                            }
                            // PLC没有"翻转请求"信号，但ECS有确认信号，说明PLC已经清除而WCS没有清除 
                            if (Allow_Flip.Value == "False" && WCS_Allow_Flip.Value == "True")
                            {
                                SendFlipToPlc(false, robot, plc);
                            }
                        }
                    }
                }
                return BllResultFactory.Sucess();
            }
            catch (Exception ex)
            {
                Logger.Log($"机器人理过程中出现异常：{ex.Message}", LogLevel.Exception, ex);
                return BllResultFactory.Error();
            }
        }

        /// <summary>
        /// 处理上料准备完成
        /// 注意：allEquipments引用所有设备，此为共享应用
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public abstract BllResult ExcuteLoadReady(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc);

        /// <summary>
        /// 处理下料准备完成
        /// 注意：allEquipments引用所有设备，此为共享应用
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public abstract BllResult ExcuteBlankReady(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc);

        /// <summary>
        /// 处理下料请求
        /// 注意：allEquipments引用所有设备，此为共享应用
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public abstract BllResult ExcuteRequest(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc);

        /// <summary>
        /// 处理上料完成
        /// 注意：allEquipments引用所有设备，此为共享应用
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public abstract BllResult ExcuteArrive(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc);

        /// <summary>
        /// 执行翻转请求
        /// 注意：allEquipments引用所有设备，此为共享应用
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public abstract BllResult AllowFlip(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc);

        /// <summary>
        /// 写入或清除 ECS允许上料信号，True为写入，False为清除
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="robot"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected BllResult SendLoadReadyToPlc(bool load_Ready, Equipment robot, IPLC plc)
        {
            var operate = load_Ready ? "写入" : "清除";
            var Load_Ready = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.Load_Ready.ToString());
            Load_Ready.Value = load_Ready.ToString();
            BllResult plcResult = plc.Write(Load_Ready);
            if (plcResult.Success)
            {
                Logger.Log($"{operate}设备【{robot.Name}】上料准备完成 信号成功", LogLevel.Success);
            }
            else
            {
                Logger.Log($"{operate}设备【{robot.Name}】上料准备完成 信号失败，写入PLC失败：原因：{plcResult.Msg}", LogLevel.Error);
            }
            return plcResult;
        }

        /// <summary>
        /// 写入或清除 ECS允许下料信号，True为写入，False为清除
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="robot"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected BllResult SendBlankReadyToPlc(bool blank_Ready, Equipment robot, IPLC plc)
        {
            var operate = blank_Ready ? "写入" : "清除";
            var Blank_Ready = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.Blank_Ready.ToString());
            Blank_Ready.Value = blank_Ready.ToString();
            BllResult plcResult = plc.Write(Blank_Ready);
            if (plcResult.Success)
            {
                Logger.Log($"{operate}设备【{robot.Name}】下料准备完成 信号成功", LogLevel.Success);
            }
            else
            {
                Logger.Log($"{operate}设备【{robot.Name}】下料准备完成 信号失败，写入PLC失败：原因：{plcResult.Msg}", LogLevel.Error);
            }
            return plcResult;
        }

        /// <summary>
        /// 写入或清除  ECS响应上料完成 ，True为写入，False为清除
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="robot"></param>
        /// <param name="arrive">是否到达，true为到达,false为清除</param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected BllResult SendArriveToPlc(bool arrive, Equipment robot, IPLC plc)
        {
            var operate = arrive ? "写入" : "清除";
            var WCS_Allow_Load = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.WCS_Allow_Load.ToString());
            WCS_Allow_Load.Value = arrive.ToString();
            BllResult plcResult = plc.Write(WCS_Allow_Load);
            if (plcResult.Success)
            {
                Logger.Log($"{operate}设备【{robot.Name}】 ECS确认上料完成 信号成功", LogLevel.Success);
            }
            else
            {
                Logger.Log($"{operate}设备【{robot.Name}】 ECS确认上料完成 信号失败，写入PLC失败：原因：{plcResult.Msg}", LogLevel.Error);
            }
            return plcResult;
        }

        /// <summary>
        /// 写入或清除  ECS允许焊接机器人翻转，True为写入，False为清除
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="robot">机器人</param>
        /// <param name="allow">是否允许翻转，true为允许,false为清除</param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected BllResult SendFlipToPlc(bool allow, Equipment robot, IPLC plc)
        {
            var operate = allow ? "写入" : "清除";
            var WCS_Allow_Flip = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.WCS_Allow_Flip.ToString());
            WCS_Allow_Flip.Value = allow.ToString();
            BllResult plcResult = plc.Write(WCS_Allow_Flip);
            if (plcResult.Success)
            {
                Logger.Log($"{operate}设备【{robot.Name}】ECS允许翻转信号成功", LogLevel.Success);
            }
            else
            {
                Logger.Log($"{operate}设备【{robot.Name}】ECS允许翻转信号失败，写入PLC失败：原因：{plcResult.Msg}", LogLevel.Error);
            }
            return plcResult;
        }

        /// <summary>
        /// 写入或清除  任务信息
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="plc"></param>
        /// <param name="stepTraceId"></param>
        /// <param name="type"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        protected BllResult SendStepTraceToPlc(string stepTraceId, string type, string number, bool wcsAllowLoad, Equipment robot, IPLC plc)
        {
            var Step_Trace_Id = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.Step_Trace_Id.ToString());
            Step_Trace_Id.Value = stepTraceId;
            var TYPE = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.TYPE.ToString());
            TYPE.Value = type;
            var Number = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.Number.ToString());
            Number.Value = number;
            var WCS_Allow_Load = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.WCS_Allow_Load.ToString());
            WCS_Allow_Load.Value = wcsAllowLoad.ToString();
            var propsToWriter = new List<EquipmentProp> { Step_Trace_Id, TYPE, Number, WCS_Allow_Load };
            return plc.Writes(propsToWriter);
        }

    }
}
