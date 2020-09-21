using HHECS.Bll;
using HHECS.EquipmentExcute.Robot.Enums;
using HHECS.Model.BllModel;
using HHECS.Model.Common;
using HHECS.Model.Entities;
using HHECS.Model.Enums;
using HHECS.Model.PLCHelper.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using System;

namespace HHECS.EquipmentExcute.Robot
{
    /// <summary>
    /// 组焊机器人
    /// </summary>
    public class RobotForAssembly : RobotExcute
    {
        #region 旧代码
        ///// <summary>
        ///// 组焊机器人的处理
        ///// </summary>
        ///// <param name="robot"></param>
        ///// <param name="allEquipments"></param>
        ///// <param name="plc"></param>
        ///// <returns></returns>
        //public override BllResult ExcuteRequest(List<Equipment> robots, List<Equipment> allEquipments, IPLC plc)
        //{
        //    try
        //    {
        //        //找出 桁车 未完成的任务
        //        var stepTraceList = AppSession.Dal.GetCommonModelByCondition<StepTrace>($"where status < {StepTraceStatus.任务完成.GetIndexInt()}");
        //        foreach (var robot in robots)
        //        {
        //            Excute(robot, allEquipments, stepTraceList.Data, plc);
        //        }
        //        return BllResultFactory.Sucess();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log($"组焊机器人处理过程中出现异常：{ex.Message}", LogLevel.Exception, ex);
        //        return BllResultFactory.Error($"组焊机器人处理过程中出现异常：{ex.Message}");
        //    }
        //}


        //private BllResult Excute(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc)
        //{
        //    ////读取所有地址值
        //    //var readResult = plc.Reads(robot.EquipmentProps.Where(t => t.EquipmentTypeTemplate.PropType == EquipmentPropType.PLC_DelayRead.ToString()).ToList());
        //    //if (!readResult.Success)
        //    //{
        //    //    Logger.Log($"组焊机器人处理过程中出现异常：读取地址错误：{readResult.Msg},请检查网络配置", LogLevel.Error);
        //    //    return BllResultFactory.Error($"组焊机器人处理过程中出现异常：读取地址错误：{readResult.Msg},请检查网络配置");
        //    //}

        //    var ready_OK = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.Ready_OK.ToString());
        //    var start_OK = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.Start_OK.ToString());
        //    //var prg_running = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcInformation.Prg_running.ToString());
        //    var start = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.Start.ToString());

        //    //下料准备完成信号
        //    var Ready_Blank = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.Ready_Blank.ToString());
        //    //ECS回复允许下料信号
        //    var Blank_Ready = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.Blank_Ready.ToString());
        //    //下料请求信号
        //    var Request_Blank = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.Request_Blank.ToString());

        //    if (ready_OK == null || start_OK == null || start == null)
        //    {
        //        return BllResultFactory.Error($"未找到{robot.Name}的任务或属性");
        //    }

        //    //如果 下料准备完成，就直接回复允许
        //    if (Ready_Blank.Value == "True" && Blank_Ready.Value == "False")
        //    {
        //        Blank_Ready.Value = "True";
        //        BllResult plcResult = plc.Write(Blank_Ready);
        //        if (plcResult.Success)
        //        {
        //            Logger.Log($"处理设备【{robot.Name}】下料准备完成 信号成功", LogLevel.Success);
        //            return BllResultFactory.Sucess();
        //        }
        //        else
        //        {
        //            Logger.Log($"处理设备【{robot.Name}】下料准备完成 信号失败，写入PLC失败：原因：{plcResult.Msg}", LogLevel.Error);
        //            return BllResultFactory.Error();
        //        }
        //    }

        //    //// 清除 ECS允许下料信号
        //    //if (Ready_Blank.Value == "False" && Blank_Ready.Value == "True")
        //    //{
        //    //    Blank_Ready.Value = "False";
        //    //    BllResult plcResult = plc.Write(Blank_Ready);
        //    //    if (plcResult.Success)
        //    //    {
        //    //        Logger.Log($"清除设备【{robot.Name}】允许下料信号成功", LogLevel.Success);
        //    //        return BllResultFactory.Sucess();
        //    //    }
        //    //    else
        //    //    {
        //    //        Logger.Log($"清除设备【{robot.Name}】允许下料信号失败，写入PLC失败：原因：{plcResult.Msg}", LogLevel.Error);
        //    //        return BllResultFactory.Error();
        //    //    }
        //    //}

        //    if (ready_OK.Value == "True")
        //    {
        //        if (start_OK.Value == "True")
        //        {
        //            #region 如果设备启动成功，就清除启动信号，清除16个物料缓存区物料信息
        //            //if (start.Value == "True")
        //            //{
        //            //    List<EquipmentProp> equipmentProps = new List<EquipmentProp>();
        //            //    start.Value = "False";
        //            //    var A_Type = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.A_Type.ToString());
        //            //    A_Type.Value = "0";
        //            //    var A_Cache_Area = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.A_Cache_Area.ToString());
        //            //    A_Cache_Area.Value = "0";
        //            //    equipmentProps.Add(start);
        //            //    equipmentProps.Add(A_Type);
        //            //    equipmentProps.Add(A_Cache_Area);
        //            //    for (var i = 1; i < 17; i++)
        //            //    {
        //            //        var A_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Tier" + i.ToString());
        //            //        A_Tier.Value = "0";
        //            //        var A_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Row" + i.ToString());
        //            //        A_Row.Value = "0";
        //            //        var A_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Line" + i.ToString());
        //            //        A_Line.Value = "0";
        //            //        var A_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Num" + i.ToString());
        //            //        A_Num.Value = "0";
        //            //        var B_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Tier" + i.ToString());
        //            //        B_Tier.Value = "0";
        //            //        var B_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Row" + i.ToString());
        //            //        B_Row.Value = "0";
        //            //        var B_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Line" + i.ToString());
        //            //        B_Line.Value = "0";
        //            //        var B_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Num" + i.ToString());
        //            //        B_Num.Value = "0";
        //            //        var C_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Tier" + i.ToString());
        //            //        C_Tier.Value = "0";
        //            //        var C_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Row" + i.ToString());
        //            //        C_Row.Value = "0";
        //            //        var C_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Line" + i.ToString());
        //            //        C_Line.Value = "0";
        //            //        var C_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Num" + i.ToString());
        //            //        C_Num.Value = "0";
        //            //        equipmentProps.Add(A_Tier);
        //            //        equipmentProps.Add(A_Row);
        //            //        equipmentProps.Add(A_Line);
        //            //        equipmentProps.Add(A_Num);
        //            //        equipmentProps.Add(B_Tier);
        //            //        equipmentProps.Add(B_Row);
        //            //        equipmentProps.Add(B_Line);
        //            //        equipmentProps.Add(B_Num);
        //            //        equipmentProps.Add(C_Tier);
        //            //        equipmentProps.Add(C_Row);
        //            //        equipmentProps.Add(C_Line);
        //            //        equipmentProps.Add(C_Num);
        //            //    }
        //            //    var plcResult = plc.Writes(equipmentProps);
        //            //    if (plcResult.Success)
        //            //    {
        //            //        return BllResultFactory.Sucess($"清除设备【{robot.Name}】启动信号成功");
        //            //    }
        //            //    else
        //            //    {
        //            //        Logger.Log($"清除设备【{robot.Name}】启动信号失败", LogLevel.Error);
        //            //        return BllResultFactory.Error($"清除设备【{robot.Name}】启动信号失败");
        //            //    }
        //            //}
        //            #endregion
        //        }

        //        #region AGV上料处理
        //        //找到工站对应的缓存位
        //        var locationResult = AppSession.Dal.GetCommonModelByCondition<Location>($"where srmCode = '{robot.Code}'");
        //        if (locationResult.Success)
        //        {
        //            #region 物料呼叫，工位必须有一个未用完的呼叫，如果没有就新增
        //            var callHeaderResult = AppSession.Dal.GetCommonModelByCondition<MaterialCallHeader>($" where needStation = '{robot.StationCode}' and status != '{MaterialCallStatus.deplete.ToString()}'");
        //            foreach (var item in locationResult.Data)
        //            {
        //                //如果呼叫过，就跳过
        //                if (callHeaderResult.Success)
        //                {
        //                    if (callHeaderResult.Data.Exists(t => t.LocationCode == item.Code))
        //                    {
        //                        continue;
        //                    }
        //                }
        //                //新增一条呼叫
        //                MaterialCallHeader materialCallHeader = new MaterialCallHeader();
        //                materialCallHeader.LineId = robot.LineId;
        //                materialCallHeader.LineCode = robot.LineCode;
        //                materialCallHeader.NeedStation = robot.StationCode;
        //                materialCallHeader.LocationCode = item.Code;
        //                materialCallHeader.CallTime = DateTime.Now;
        //                materialCallHeader.Status = MaterialCallStatus.ready.ToString();
        //                materialCallHeader.Mode = "auto";
        //                materialCallHeader.FromPlatform = "PLC";
        //                materialCallHeader.UserCode = App.User.UserCode;
        //                materialCallHeader.CreateTime = DateTime.Now;
        //                materialCallHeader.CreateBy = App.User.UserCode;
        //                materialCallHeader.UpdateTime = DateTime.Now;
        //                materialCallHeader.UpdateBy = App.User.UserCode;
        //                var result = AppSession.Dal.InsertCommonModel<MaterialCallHeader>(materialCallHeader);
        //                if (result.Success == false)
        //                {
        //                    return BllResultFactory.Error(result.Msg);
        //                }
        //            }
        //            #endregion

        //            #region 将配送完成的物料写入PLC

        //            //var A_Incoming = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.A_Incoming.ToString());
        //            //var B_Incoming = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.B_Incoming.ToString());
        //            //var C_Incoming = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.C_Incoming.ToString());

        //            //if (A_Incoming.Value == "False")
        //            //{
        //            //    var distributeHeader = materialDistribute(locationResult.Data, "A");
        //            //    if (distributeHeader.Success)
        //            //    {
        //            //        //修改配送主表的状态为"投入使用"
        //            //        var excuteResult = AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送投入使用} where id={distributeHeader.Data.Id}");
        //            //        if (excuteResult.Success)
        //            //        {
        //            //            #region 把物料写入料框类型A
        //            //            List<EquipmentProp> equipmentProps = new List<EquipmentProp>();
        //            //            A_Incoming.Value = "True";
        //            //            var A_Type = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.A_Type.ToString());
        //            //            A_Type.Value = distributeHeader.Data.ProductId.ToString();
        //            //            var A_Cache_Area = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.A_Cache_Area.ToString());
        //            //            A_Cache_Area.Value = distributeHeader.Data.LocationCode;
        //            //            equipmentProps.Add(A_Incoming);
        //            //            equipmentProps.Add(A_Type);
        //            //            equipmentProps.Add(A_Cache_Area);
        //            //            for (var i = 0; i < distributeHeader.Data.distributeDetails.Count; i++)
        //            //            {
        //            //                var A_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Tier" + (i + 1).ToString());
        //            //                A_Tier.Value = distributeHeader.Data.distributeDetails[i].Layer.ToString();
        //            //                var A_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Row" + (i + 1).ToString());
        //            //                A_Row.Value = distributeHeader.Data.distributeDetails[i].Row.ToString();
        //            //                var A_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Line" + (i + 1).ToString());
        //            //                A_Line.Value = distributeHeader.Data.distributeDetails[i].Column.ToString();
        //            //                var A_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Num" + (i + 1).ToString());
        //            //                A_Num.Value = distributeHeader.Data.distributeDetails[i].BomNum.ToString();
        //            //                equipmentProps.Add(A_Tier);
        //            //                equipmentProps.Add(A_Row);
        //            //                equipmentProps.Add(A_Line);
        //            //                equipmentProps.Add(A_Num);
        //            //            }
        //            //            var plcResult = plc.Writes(equipmentProps);
        //            //            if (!plcResult.Success)
        //            //            {
        //            //                //如果写入PLC失败，就把状态回滚为"配送完成"
        //            //                AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送完成} where id={distributeHeader.Data.Id}");
        //            //            }
        //            //            #endregion
        //            //        }
        //            //    }
        //            //    else
        //            //    {
        //            //        return BllResultFactory.Error($"处理设备【{robot.Name}】对应物料缓存失败，原因:{distributeHeader.Msg}！");
        //            //    }
        //            //}
        //            //if (B_Incoming.Value == "False")
        //            //{
        //            //    var distributeHeader = materialDistribute(locationResult.Data, "B");
        //            //    if (distributeHeader.Success)
        //            //    {
        //            //        //修改配送主表的状态为"投入使用"
        //            //        var excuteResult = AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送投入使用} where id={distributeHeader.Data.Id}");
        //            //        if (excuteResult.Success)
        //            //        {
        //            //            #region 把物料写入料框类型B
        //            //            List<EquipmentProp> equipmentProps = new List<EquipmentProp>();
        //            //            B_Incoming.Value = "True";
        //            //            var B_Type = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.B_Type.ToString());
        //            //            B_Type.Value = distributeHeader.Data.ProductId.ToString();
        //            //            var B_Cache_Area = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.B_Cache_Area.ToString());
        //            //            B_Cache_Area.Value = distributeHeader.Data.LocationCode;
        //            //            equipmentProps.Add(B_Incoming);
        //            //            equipmentProps.Add(B_Type);
        //            //            equipmentProps.Add(B_Cache_Area);
        //            //            for (var i = 0; i < distributeHeader.Data.distributeDetails.Count; i++)
        //            //            {
        //            //                var B_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Tier" + (i + 1).ToString());
        //            //                B_Tier.Value = distributeHeader.Data.distributeDetails[i].Layer.ToString();
        //            //                var B_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Row" + (i + 1).ToString());
        //            //                B_Row.Value = distributeHeader.Data.distributeDetails[i].Row.ToString();
        //            //                var B_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Line" + (i + 1).ToString());
        //            //                B_Line.Value = distributeHeader.Data.distributeDetails[i].Column.ToString();
        //            //                var B_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Num" + (i + 1).ToString());
        //            //                B_Num.Value = distributeHeader.Data.distributeDetails[i].BomNum.ToString();
        //            //                equipmentProps.Add(B_Tier);
        //            //                equipmentProps.Add(B_Row);
        //            //                equipmentProps.Add(B_Line);
        //            //                equipmentProps.Add(B_Num);
        //            //            }
        //            //            var plcResult = plc.Writes(equipmentProps);
        //            //            if (!plcResult.Success)
        //            //            {
        //            //                //如果写入PLC失败，就把状态回滚为"配送完成"
        //            //                AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送完成} where id={distributeHeader.Data.Id}");
        //            //            }
        //            //            #endregion
        //            //        }
        //            //    }
        //            //    else
        //            //    {
        //            //        return BllResultFactory.Error($"处理设备【{robot.Name}】对应物料缓存失败，原因:{distributeHeader.Msg}！");
        //            //    }
        //            //}
        //            //if (C_Incoming.Value == "False")
        //            //{
        //            //    var distributeHeader = materialDistribute(locationResult.Data, "C");
        //            //    if (distributeHeader.Success)
        //            //    {
        //            //        //修改配送主表的状态为"投入使用"
        //            //        var excuteResult = AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送投入使用} where id={distributeHeader.Data.Id}");
        //            //        if (excuteResult.Success)
        //            //        {
        //            //            #region 把物料写入料框类型C
        //            //            List<EquipmentProp> equipmentProps = new List<EquipmentProp>();
        //            //            C_Incoming.Value = "True";
        //            //            var C_Type = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.C_Type.ToString());
        //            //            C_Type.Value = distributeHeader.Data.ProductId.ToString();
        //            //            var C_Cache_Area = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.C_Cache_Area.ToString());
        //            //            C_Cache_Area.Value = distributeHeader.Data.LocationCode;
        //            //            equipmentProps.Add(C_Incoming);
        //            //            equipmentProps.Add(C_Type);
        //            //            equipmentProps.Add(C_Cache_Area);
        //            //            for (var i = 0; i < distributeHeader.Data.distributeDetails.Count; i++)
        //            //            {
        //            //                var C_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Tier" + (i + 1).ToString());
        //            //                C_Tier.Value = distributeHeader.Data.distributeDetails[i].Layer.ToString();
        //            //                var C_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Row" + (i + 1).ToString());
        //            //                C_Row.Value = distributeHeader.Data.distributeDetails[i].Row.ToString();
        //            //                var C_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Line" + (i + 1).ToString());
        //            //                C_Line.Value = distributeHeader.Data.distributeDetails[i].Column.ToString();
        //            //                var C_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Num" + (i + 1).ToString());
        //            //                C_Num.Value = distributeHeader.Data.distributeDetails[i].BomNum.ToString();
        //            //                equipmentProps.Add(C_Tier);
        //            //                equipmentProps.Add(C_Row);
        //            //                equipmentProps.Add(C_Line);
        //            //                equipmentProps.Add(C_Num);
        //            //            }
        //            //            var plcResult = plc.Writes(equipmentProps);
        //            //            if (!plcResult.Success)
        //            //            {
        //            //                //如果写入PLC失败，就把状态回滚为"配送完成"
        //            //                AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送完成} where id={distributeHeader.Data.Id}");
        //            //            }
        //            //            #endregion
        //            //        }
        //            //    }
        //            //    else
        //            //    {
        //            //        return BllResultFactory.Error($"处理设备【{robot.Name}】对应物料缓存失败，原因:{distributeHeader.Msg}！");
        //            //    }
        //            //}
        //            #endregion

        //            #region 如果A、B、C框都到齐了，就可以生产了

        //            //if (A_Incoming.Value == "True" && B_Incoming.Value == "True" && C_Incoming.Value == "True")
        //            //{
        //            //    start.Value = "True";
        //            //    var result = plc.Write(start);
        //            //    if (result.Success)
        //            //    {
        //            //        return BllResultFactory.Sucess($"写入设备【{robot.Name}】启动信号成功");
        //            //    }
        //            //    else
        //            //    {
        //            //        Logger.Log($"写入设备【{robot.Name}】启动信号失败", LogLevel.Error);
        //            //        return BllResultFactory.Error($"写入设备【{robot.Name}】启动信号失败");
        //            //    }
        //            //}
        //            #endregion
        //        }
        //        #endregion

        //        #region 处理下料请求

        //        if (Request_Blank.Value == "True")
        //        {
        //            var TYPE_Feedback = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.TYPE_Feedback.ToString()).Value;
        //            var convertResult = int.TryParse(TYPE_Feedback, out int productId);                   
        //            if (!convertResult || productId < 1)
        //            {
        //                Logger.Log($"处理设备【{robot.Name}】下料请求失败，读取工件类型错误，工件类型[{TYPE_Feedback}]不是大于1的整数", LogLevel.Error);
        //                return BllResultFactory.Error();
        //            }
        //            var stepTrace = stepTraceList.FirstOrDefault(t => t.StationId == robot.StationId);
        //            // 如果不存在 本站台的任务，但是还在请求下线，就新增一条任务
        //            if (stepTrace == null)
        //            {
        //                var insertResult = insertStepTrace(robot);
        //                if (insertResult.Success)
        //                {
        //                    Logger.Log($"处理设备[{robot.Name}]的站台[{robot.StationId}]下料请求成功", LogLevel.Error);
        //                }
        //                return insertResult;
        //            }
        //        }

        //        #endregion
        //    }
        //    return BllResultFactory.Sucess();

        //}
        #endregion

        /// <summary>
        /// 处理设备上料准备完成
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="stepTraceList"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public override BllResult ExcuteLoadReady(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc)
        {
            return SendLoadReadyToPlc(true, robot, plc);
            ////找到工站对应的缓存位
            //var locationResult = AppSession.Dal.GetCommonModelByCondition<Location>($"where srmCode = '{robot.Code}' ");
            //if (!locationResult.Success)
            //{
            //    Logger.Log($"处理设备[{robot.Name}]工位[{robot.StationId}]下料准备完成 失败，获取设备对应的上料点位失败", LogLevel.Error);
            //    return BllResultFactory.Error();
            //}

            //#region 如果A、B、C框都到齐了，就可以生产，需要从PLC取值判断，因为C料框是人工配送的

            //var A_Incoming = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.A_Incoming.ToString());
            //var B_Incoming = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.B_Incoming.ToString());
            //var C_Incoming = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.C_Incoming.ToString());

            //if (A_Incoming.Value == "True" && B_Incoming.Value == "True" && C_Incoming.Value == "True")
            //{
            //    #region 如果有"使用中"的呼叫，并且有未完成明细，就根据明细生成任务，然后把任务号和允许抓料信号写入设备，然后返回。
            //    //return SendBlankReadyToPlc(true, robot, plc);
            //    #endregion
            //    #region 如果没有"使用中"的呼叫，或者没有未使用明细，就把呼叫头表状态改为"完成"，并且清除掉写给PLC的料框给定信息。
            //    //return SendBlankReadyToPlc(true, robot, plc);
            //    #endregion
            //}
            //#endregion

            //#region 判断设备是否有4个有效呼叫（AABB，C料框是手动上料），如果没有就新增呼叫头表，保持一个设备有4个有效呼叫
            //var callHeaderResult = AppSession.Dal.GetCommonModelByCondition<MaterialCallHeader>($" where needStation = '{robot.StationCode}' and status < {MaterialCallStatus.物料使用完成.GetIndexString()}");
            //foreach (var item in locationResult.Data)
            //{
            //    //如果呼叫过，就跳过
            //    if (callHeaderResult.Success)
            //    {
            //        if (callHeaderResult.Data.Exists(t => t.LocationCode == item.Code))
            //        {
            //            continue;
            //        }
            //    }
            //    //新增一条呼叫
            //    MaterialCallHeader materialCallHeader = new MaterialCallHeader();
            //    materialCallHeader.LineId = robot.LineId;
            //    materialCallHeader.LineCode = robot.LineCode;
            //    materialCallHeader.NeedStation = robot.StationCode;
            //    materialCallHeader.LocationCode = item.Code;
            //    materialCallHeader.CallTime = DateTime.Now;
            //    materialCallHeader.Status = MaterialCallStatus.ready.ToString();
            //    materialCallHeader.Mode = "auto";
            //    materialCallHeader.FromPlatform = "PLC";
            //    materialCallHeader.UserCode = App.User.UserCode;
            //    materialCallHeader.CreateTime = DateTime.Now;
            //    materialCallHeader.CreateBy = App.User.UserCode;
            //    materialCallHeader.UpdateTime = DateTime.Now;
            //    materialCallHeader.UpdateBy = App.User.UserCode;
            //    var result = AppSession.Dal.InsertCommonModel<MaterialCallHeader>(materialCallHeader);
            //    if (result.Success == false)
            //    {
            //        return BllResultFactory.Error(result.Msg);
            //    }
            //}
            //#endregion

            //#region 如果有"已到达"的呼叫头表，并且料框类型不是"使用中"的料框类型，就把对应的物料信息写入PLC，并且把呼叫头表状态改为"使用中"
            //if (A_Incoming.Value == "False")
            //{
            //    var distributeHeader = materialDistribute(locationResult.Data, "A");
            //    if (distributeHeader.Success)
            //    {
            //        //修改配送主表的状态为"投入使用"
            //        var excuteResult = AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送投入使用} where id={distributeHeader.Data.Id}");
            //        if (excuteResult.Success)
            //        {
            //            #region 把物料写入料框类型A
            //            List<EquipmentProp> equipmentProps = new List<EquipmentProp>();
            //            A_Incoming.Value = "True";
            //            var A_Type = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.A_Type.ToString());
            //            A_Type.Value = distributeHeader.Data.ProductId.ToString();
            //            var A_Cache_Area = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.A_Cache_Area.ToString());
            //            A_Cache_Area.Value = distributeHeader.Data.LocationCode;
            //            equipmentProps.Add(A_Incoming);
            //            equipmentProps.Add(A_Type);
            //            equipmentProps.Add(A_Cache_Area);
            //            for (var i = 0; i < distributeHeader.Data.distributeDetails.Count; i++)
            //            {
            //                var A_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Tier" + (i + 1).ToString());
            //                A_Tier.Value = distributeHeader.Data.distributeDetails[i].Layer.ToString();
            //                var A_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Row" + (i + 1).ToString());
            //                A_Row.Value = distributeHeader.Data.distributeDetails[i].Row.ToString();
            //                var A_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Line" + (i + 1).ToString());
            //                A_Line.Value = distributeHeader.Data.distributeDetails[i].Column.ToString();
            //                var A_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Num" + (i + 1).ToString());
            //                A_Num.Value = distributeHeader.Data.distributeDetails[i].BomNum.ToString();
            //                equipmentProps.Add(A_Tier);
            //                equipmentProps.Add(A_Row);
            //                equipmentProps.Add(A_Line);
            //                equipmentProps.Add(A_Num);
            //            }
            //            var plcResult = plc.Writes(equipmentProps);
            //            if (!plcResult.Success)
            //            {
            //                //如果写入PLC失败，就把状态回滚为"配送完成"
            //                AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送完成} where id={distributeHeader.Data.Id}");
            //            }
            //            #endregion
            //        }
            //    }
            //    else
            //    {
            //        return BllResultFactory.Error($"处理设备【{robot.Name}】对应物料缓存失败，原因:{distributeHeader.Msg}！");
            //    }
            //}
            //if (B_Incoming.Value == "False")
            //{
            //    var distributeHeader = materialDistribute(locationResult.Data, "B");
            //    if (distributeHeader.Success)
            //    {
            //        //修改配送主表的状态为"投入使用"
            //        var excuteResult = AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送投入使用} where id={distributeHeader.Data.Id}");
            //        if (excuteResult.Success)
            //        {
            //            #region 把物料写入料框类型B
            //            List<EquipmentProp> equipmentProps = new List<EquipmentProp>();
            //            B_Incoming.Value = "True";
            //            var B_Type = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.B_Type.ToString());
            //            B_Type.Value = distributeHeader.Data.ProductId.ToString();
            //            var B_Cache_Area = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.B_Cache_Area.ToString());
            //            B_Cache_Area.Value = distributeHeader.Data.LocationCode;
            //            equipmentProps.Add(B_Incoming);
            //            equipmentProps.Add(B_Type);
            //            equipmentProps.Add(B_Cache_Area);
            //            for (var i = 0; i < distributeHeader.Data.distributeDetails.Count; i++)
            //            {
            //                var B_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Tier" + (i + 1).ToString());
            //                B_Tier.Value = distributeHeader.Data.distributeDetails[i].Layer.ToString();
            //                var B_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Row" + (i + 1).ToString());
            //                B_Row.Value = distributeHeader.Data.distributeDetails[i].Row.ToString();
            //                var B_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Line" + (i + 1).ToString());
            //                B_Line.Value = distributeHeader.Data.distributeDetails[i].Column.ToString();
            //                var B_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Num" + (i + 1).ToString());
            //                B_Num.Value = distributeHeader.Data.distributeDetails[i].BomNum.ToString();
            //                equipmentProps.Add(B_Tier);
            //                equipmentProps.Add(B_Row);
            //                equipmentProps.Add(B_Line);
            //                equipmentProps.Add(B_Num);
            //            }
            //            var plcResult = plc.Writes(equipmentProps);
            //            if (!plcResult.Success)
            //            {
            //                //如果写入PLC失败，就把状态回滚为"配送完成"
            //                AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送完成} where id={distributeHeader.Data.Id}");
            //            }
            //            #endregion
            //        }
            //    }
            //    else
            //    {
            //        return BllResultFactory.Error($"处理设备【{robot.Name}】对应物料缓存失败，原因:{distributeHeader.Msg}！");
            //    }
            //}
            //if (C_Incoming.Value == "False")
            //{
            //    var distributeHeader = materialDistribute(locationResult.Data, "C");
            //    if (distributeHeader.Success)
            //    {
            //        //修改配送主表的状态为"投入使用"
            //        var excuteResult = AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送投入使用} where id={distributeHeader.Data.Id}");
            //        if (excuteResult.Success)
            //        {
            //            #region 把物料写入料框类型C
            //            List<EquipmentProp> equipmentProps = new List<EquipmentProp>();
            //            C_Incoming.Value = "True";
            //            var C_Type = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.C_Type.ToString());
            //            C_Type.Value = distributeHeader.Data.ProductId.ToString();
            //            var C_Cache_Area = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.C_Cache_Area.ToString());
            //            C_Cache_Area.Value = distributeHeader.Data.LocationCode;
            //            equipmentProps.Add(C_Incoming);
            //            equipmentProps.Add(C_Type);
            //            equipmentProps.Add(C_Cache_Area);
            //            for (var i = 0; i < distributeHeader.Data.distributeDetails.Count; i++)
            //            {
            //                var C_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Tier" + (i + 1).ToString());
            //                C_Tier.Value = distributeHeader.Data.distributeDetails[i].Layer.ToString();
            //                var C_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Row" + (i + 1).ToString());
            //                C_Row.Value = distributeHeader.Data.distributeDetails[i].Row.ToString();
            //                var C_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Line" + (i + 1).ToString());
            //                C_Line.Value = distributeHeader.Data.distributeDetails[i].Column.ToString();
            //                var C_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Num" + (i + 1).ToString());
            //                C_Num.Value = distributeHeader.Data.distributeDetails[i].BomNum.ToString();
            //                equipmentProps.Add(C_Tier);
            //                equipmentProps.Add(C_Row);
            //                equipmentProps.Add(C_Line);
            //                equipmentProps.Add(C_Num);
            //            }
            //            var plcResult = plc.Writes(equipmentProps);
            //            if (!plcResult.Success)
            //            {
            //                //如果写入PLC失败，就把状态回滚为"配送完成"
            //                AppSession.Dal.ExcuteCommonSqlForInsertOrUpdate($"update material_distribute_task_header set status={DistributeStatus.配送完成} where id={distributeHeader.Data.Id}");
            //            }
            //            #endregion
            //        }
            //    }
            //    else
            //    {
            //        return BllResultFactory.Error($"处理设备【{robot.Name}】对应物料缓存失败，原因:{distributeHeader.Msg}！");
            //    }
            //}
            //#endregion

            //return BllResultFactory.Error();

        }

        /// <summary>
        /// 处理设备下料准备完成
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="stepTraceList"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public override BllResult ExcuteBlankReady(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc)
        {
            return SendBlankReadyToPlc(true, robot, plc);
        }

        /// <summary>
        /// 处理设备请求下线
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="stepTraceList"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public override BllResult ExcuteRequest(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc)
        {
            var TYPE_Feedback = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.TYPE_Feedback.ToString()).Value;
            var convertResult = int.TryParse(TYPE_Feedback, out int wcsProductType);
            if (!convertResult || wcsProductType < 1)
            {
                Logger.Log($"处理设备【{robot.Name}】下料请求失败，读取工件类型错误，工件类型[{TYPE_Feedback}]不是大于1的整数", LogLevel.Error);
                return BllResultFactory.Error();
            }
            var stepTraceCount = stepTraceList.Count(t => t.StationId == robot.StationId);
            if (stepTraceCount > 1)
            {
                Logger.Log($"处理设备[{robot.Name}]对应的站台[{robot.StationId}]下料请求失败，站台存在{stepTraceCount}个任务", LogLevel.Error);
                return BllResultFactory.Error();
            }
            var stepTrace = stepTraceList.FirstOrDefault(t => t.StationId == robot.StationId);
            // 如果不存在 本站台的任务，但是还在请求下线，就新增一条任务
            if (stepTrace == null)
            {
                var insertResult = insertStepTrace(robot);
                if (insertResult.Success)
                {
                    Logger.Log($"处理设备[{robot.Name}]的站台[{robot.StationId}]下料请求成功", LogLevel.Success);
                }
                return insertResult;
            }
            if (stepTrace.WcsProductType != wcsProductType)
            {
                Logger.Log($"处理设备[{robot.Name}]对应的站台[{robot.StationId}]下料请求失败，站台已存在任务[{stepTrace.Id}]的工件类型[{stepTrace.WcsProductType}]和请求的工件类型[{wcsProductType}]不一致，请人工处理旧任务数据！", LogLevel.Error);
                return BllResultFactory.Error();
            }
            return BllResultFactory.Sucess();
        }

        /// <summary>
        /// 处理设备上料完成
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="stepTraceList"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public override BllResult ExcuteArrive(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc)
        {
            return BllResultFactory.Error($"设备[{robot.Name}][{robot.Code}]不处理上料完成");
        }

        /// <summary>
        /// 处理翻转请求
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="stepTraceList"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public override BllResult AllowFlip(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc)
        {
            return BllResultFactory.Error($"设备[{robot.Name}][{robot.Code}]不处理翻转");
        }

        /// <summary>
        /// 清除 组焊机器人对应的缓存区物料信息
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="allEquipments"></param>
        /// <param name="stepTraceList"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        private BllResult CacheAreaClear(Equipment robot, List<Equipment> allEquipments, List<StepTrace> stepTraceList, IPLC plc)
        {
            var start = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcRobotStation.Start.ToString());
            if (start.Value == "True")
            {
                List<EquipmentProp> equipmentProps = new List<EquipmentProp>();
                start.Value = "False";
                var A_Type = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.A_Type.ToString());
                A_Type.Value = "0";
                var A_Cache_Area = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == PcPlcAgv.A_Cache_Area.ToString());
                A_Cache_Area.Value = "0";
                equipmentProps.Add(start);
                equipmentProps.Add(A_Type);
                equipmentProps.Add(A_Cache_Area);
                for (var i = 1; i < 17; i++)
                {
                    var A_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Tier" + i.ToString());
                    A_Tier.Value = "0";
                    var A_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Row" + i.ToString());
                    A_Row.Value = "0";
                    var A_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Line" + i.ToString());
                    A_Line.Value = "0";
                    var A_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "A_Num" + i.ToString());
                    A_Num.Value = "0";
                    var B_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Tier" + i.ToString());
                    B_Tier.Value = "0";
                    var B_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Row" + i.ToString());
                    B_Row.Value = "0";
                    var B_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Line" + i.ToString());
                    B_Line.Value = "0";
                    var B_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "B_Num" + i.ToString());
                    B_Num.Value = "0";
                    var C_Tier = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Tier" + i.ToString());
                    C_Tier.Value = "0";
                    var C_Row = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Row" + i.ToString());
                    C_Row.Value = "0";
                    var C_Line = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Line" + i.ToString());
                    C_Line.Value = "0";
                    var C_Num = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == "C_Num" + i.ToString());
                    C_Num.Value = "0";
                    equipmentProps.Add(A_Tier);
                    equipmentProps.Add(A_Row);
                    equipmentProps.Add(A_Line);
                    equipmentProps.Add(A_Num);
                    equipmentProps.Add(B_Tier);
                    equipmentProps.Add(B_Row);
                    equipmentProps.Add(B_Line);
                    equipmentProps.Add(B_Num);
                    equipmentProps.Add(C_Tier);
                    equipmentProps.Add(C_Row);
                    equipmentProps.Add(C_Line);
                    equipmentProps.Add(C_Num);
                }
                var plcResult = plc.Writes(equipmentProps);
                if (plcResult.Success)
                {
                    return BllResultFactory.Sucess($"清除设备【{robot.Name}】启动信号成功");
                }
                else
                {
                    Logger.Log($"清除设备【{robot.Name}】启动信号失败", LogLevel.Error);
                    return BllResultFactory.Error($"清除设备【{robot.Name}】启动信号失败");
                }
            }
            return BllResultFactory.Sucess();
        }

        /// <summary>
        /// 获取指定缓存区的配送到达的物料
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public BllResult<MaterialDistributeTaskHeader> materialDistribute(List<Location> locations, string type)
        {
            string[] locationCode = null;
            try
            {
                locationCode = locations.Where(t => t.Type == type).Select(t => t.Code).ToArray();
                if (locationCode == null)
                {
                    return BllResultFactory<MaterialDistributeTaskHeader>.Error($"没有类型为{type}的缓存区！");
                }
                MaterialDistributeTaskHeader distributeHeader = null;
                IEnumerable<MaterialDistributeTaskDetail> distributeDetails = null;
                IEnumerable<Material> materials = null;
                IEnumerable<MbomDetail> mbomDetails = null;

                var sql = $"select top 1 * from material_distribute_task_header where status=@status and locationCode in @locationCode";
                var param = new { status = DistributeStatus.配送完成, locationCode };

                using (IDbConnection connection = AppSession.Dal.GetConnection())
                {
                    // 找到到达的配送任务
                    distributeHeader = connection.QueryFirst<MaterialDistributeTaskHeader>(sql, param);
                    if (distributeHeader != null)
                    {
                        distributeDetails = connection.GetList<MaterialDistributeTaskDetail>($"where materialDistributeTaskHeaderId={distributeHeader.Id}");
                        materials = connection.GetList<Material>($"where code in @materialCodes", new { materialCodes = distributeDetails.Select(t => t.MaterialCode).ToArray() });
                        mbomDetails = connection.GetList<MbomDetail>($"where productCode = '{distributeHeader.ProductCode}'");
                    }
                    else
                    {
                        return BllResultFactory<MaterialDistributeTaskHeader>.Error("缓存区没有配送完成物料");
                    }
                }
                foreach (var distributeDetail in distributeDetails)
                {
                    var material = materials.FirstOrDefault(t => t.Code == distributeDetail.MaterialCode);
                    if (material != null)
                    {
                        var qty = distributeDetail.Qty;
                        var layer = qty / (material.MaxRow * material.MaxColumn);
                        var column = (decimal)material.MaxColumn;
                        var row = (decimal)material.MaxRow;
                        var remainder = qty % (material.MaxRow * material.MaxColumn);
                        //如果有余数，说明还有不满1层的物料，所以层数要+1
                        if (remainder > 0)
                        {
                            layer = layer + 1;
                            column = remainder / material.MaxRow;
                            remainder = remainder % material.MaxRow;
                            //如果有余数，说明还有不满1列的数量，所以列数要+1
                            if (remainder > 0)
                            {
                                column = column + 1;
                                row = remainder;
                            }
                        }
                        distributeDetail.Layer = (short)layer;
                        distributeDetail.Column = (short)column;
                        distributeDetail.Row = (short)row;
                    }
                    var mbomDetail = mbomDetails.FirstOrDefault(t => t.MaterialCode == distributeDetail.MaterialCode);
                    if (mbomDetail != null)
                    {
                        distributeDetail.BomNum = (short)mbomDetail.BaseQty.Value;
                    }
                    distributeHeader.distributeDetails = distributeDetails.ToList();
                }
                return BllResultFactory<MaterialDistributeTaskHeader>.Sucess(distributeHeader, "");
            }
            catch (Exception ex)
            {
                Logger.Log($"获取缓存区[{String.Join(", ", locationCode)}]送达的物料时候，发生异常：{ex.Message}", LogLevel.Exception, ex);
                return BllResultFactory<MaterialDistributeTaskHeader>.Error(ex.Message);
            }
        }

        /// <summary>
        /// 组焊下线的数据处理，修改呼叫明细状态，插入工序记录
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="connection"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        private BllResult insertStepTrace(Equipment robot)
        {
            try
            {
                ProductHeader productHeader = null;
                //获取电器的工件类型
                var productTypeProp = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.TYPE_Feedback.ToString());
                var result1 = int.TryParse(productTypeProp.Value, out int productType);
                if (productType < 1)
                {
                    Logger.Log($"处理设备[{robot.Name}]的站台[{robot.StationId}]下料请求失败，工件类型[{productType}]小于1", LogLevel.Error);
                    return BllResultFactory.Error();
                }
                //获取产品信息
                var productHeaderResult = AppSession.Dal.GetCommonModelByCondition<ProductHeader>($"where wcsProductType = '{productType}'");
                if (productHeaderResult.Success)
                {
                    productHeader = productHeaderResult.Data[0];
                }
                else
                {
                    Logger.Log($"处理设备[{robot.Name}]的站台[{robot.StationId}]下料请求时候，根据设备工件类型[{productType}]没有检测到对应的产品信息，原因：{productHeaderResult.Msg}，请检查产品基础数据", LogLevel.Error);
                    return BllResultFactory.Error();
                }
                //根据产品找出前两个工序，第一个是组焊工序，第二个就是下个工序
                var stepResult = AppSession.Dal.GetCommonModelByCondition<Step>($"where productCode = '{productHeader.Code}'  order by sequence");
                if (!stepResult.Success)
                {
                    Logger.Log($"处理设备[{robot.Name}]的站台[{robot.StationId}]下料请求时候，根据产品编码[{productHeader.Code}]没有检测到工序信息，原因：{stepResult.Msg}", LogLevel.Error);
                    return BllResultFactory.Error();
                }
                if (stepResult.Data.Count < 2)
                {
                    Logger.Log($"处理设备[{robot.Name}]的站台[{robot.StationId}]下料请求时候，根据产品编码[{productHeader.Code}]没有检测到下一个工序信息，请检查工序基础数据！", LogLevel.Error);
                    return BllResultFactory.Error();
                }
                //向工序表插入一条记录
                StepTrace stepTrace = new StepTrace();
                //stepTrace.WONumber = callDetail.OrderCode;
                /* 由于目前是手动上料，暂用电气的ID，等到自动送料了，再改回数据库的 */
                stepTrace.WcsProductType = productType;
                stepTrace.ProductId = productHeader.Id.Value;
                stepTrace.ProductCode = productHeader.Code;
                //stepTrace.SerialNumber = callDetail.SerialNumber;
                stepTrace.LineId = robot.LineId;
                stepTrace.StepId = stepResult.Data[0].Id.Value;
                stepTrace.StationId = robot.StationId;
                stepTrace.NextStepId = stepResult.Data[1].Id.Value;
                stepTrace.IsNG = false;
                stepTrace.NGcode = "";
                stepTrace.IsInvalid = false;
                stepTrace.Status = StepTraceStatus.设备请求下料.GetIndexInt();
                stepTrace.StationInTime = DateTime.Now;
                stepTrace.LineInTime = DateTime.Now;
                stepTrace.CreateTime = DateTime.Now;
                stepTrace.CreateBy = App.User.UserCode;
                var insertResult = AppSession.Dal.InsertCommonModel<StepTrace>(stepTrace);
                if (insertResult.Success)
                {
                    return BllResultFactory.Sucess();
                }
                else
                {
                    Logger.Log($"处理设备[{robot.Name}]的站台[{robot.StationId}]下料请求时候，插入工序监控表失败：{insertResult.Msg}", LogLevel.Error);
                    return BllResultFactory.Error();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"处理设备[{robot.Name}]的站台[{robot.StationId}]下料请求时候，发生异常：{ex.Message}", LogLevel.Exception, ex);
                return BllResultFactory.Error();
            }

            //using (IDbConnection connection = AppSession.Dal.GetConnection())
            //{
            //    IDbTransaction tran = null;
            //    try
            //    {
            //        connection.Open();
            //        ////配送明细
            //        MaterialCallDetail callDetail = null;
            //        // 找到"投入使用"的配送任务
            //        var distributeHeaderList = connection.GetList<MaterialDistributeTaskHeader>($"where status=@status and needStation = @locationCode", new { status = DistributeStatus.配送投入使用.GetIndexInt(), locationCode = robot.Code });
            //        if (distributeHeaderList.Count() > 0)
            //        {
            //            tran = connection.BeginTransaction();

            //            foreach (var distributeHeader in distributeHeaderList)
            //            {
            //                //根据投入使用的配送任务，找到对应的呼叫，然后把一条 呼叫明细 改为使用。
            //                var sql = $"select top 1 * from material_call_detail where callHeaderId = @callHeaderId and used = @used";
            //                var pram = new { callHeaderId = distributeHeader.MaterialCallId.Value, used = 0 };
            //                callDetail = connection.QuerySingleOrDefault<MaterialCallDetail>(sql, pram, transaction: tran);
            //                if (callDetail != null)
            //                {
            //                    callDetail.Used = true;
            //                    /* 由于现在没送料，是手动上料，于是先注释掉 
            //                    connection.Update(callDetail, transaction: tran);  
            //                    */
            //                }
            //                else
            //                {
            //                    tran?.Rollback();
            //                    Logger.Log($"处理设备【{robot.Name}】下料请求时候，没有未生产的呼叫明细，无法扣减呼叫明细，请核查数据", LogLevel.Error);
            //                    return BllResultFactory.Error();
            //                }
            //                /* 耗尽不再由软件判断，改由机器人给判断，先注释掉
            //                ////如果呼叫明细都是已经使用过的，那么呼叫头标状态就改为 deplete(已耗尽)                       
            //                //var count = connection.RecordCount<MaterialCallDetail>($"where callHeaderId = @callHeaderId ", new { callHeaderId = distributeHeader.MaterialCallId.Value }, tran);
            //                //if (count == 0)
            //                //{
            //                //    distributeHeader.Status = DistributeStatus.配送使用完毕.GetIndexInt();
            //                //    connection.Update(distributeHeader, transaction: tran);
            //                //    connection.Execute($"update material_call_header set status = @status where id=@id ", new { status = MaterialCallStatus.deplete, id = distributeHeader.MaterialCallId.Value }, transaction: tran);
            //                //}
            //                */
            //            }
            //        }
            //        else
            //        {
            //            Logger.Log($"处理设备【{robot.Name}】下料请求的时候，物料配送中没有【{DistributeStatus.配送投入使用}】的物料！", LogLevel.Error);
            //            return BllResultFactory.Error();
            //        }

            //        ProductHeader productHeader = null;
            //        //获取电器的工件类型
            //        var productTypeProp = robot.EquipmentProps.Find(t => t.EquipmentTypeTemplateCode == RobotPcStation.TYPE_Feedback.ToString());
            //        var result1 = int.TryParse(productTypeProp.Value, out int productType);
            //        if (productType < 1)
            //        {
            //            Logger.Log($"处理设备[{robot.Name}]的站台[{robot.StationId}]下料请求失败，工件类型[{productType}]小于1", LogLevel.Error);
            //            return BllResultFactory.Error();
            //        }
            //        //获取产品信息
            //        var productHeaderResult = AppSession.Dal.GetCommonModelByCondition<ProductHeader>($"where wcsProductType = '{productType}'");
            //        if (productHeaderResult.Success)
            //        {
            //            productHeader = productHeaderResult.Data[0];
            //        }
            //        else
            //        {
            //            Logger.Log($"处理设备[{robot.Name}]的站台[{robot.StationId}]下料请求时候，根据设备工件类型[{productType}]没有检测到对应的产品信息，请检查产品基础数据", LogLevel.Error);
            //            return BllResultFactory.Error();
            //        }
            //        //根据产品找出前两个工序，第一个是组焊工序，第二个就是下个工序
            //        var step = connection.Query<Step>($"select top 2 * from step where productCode = @productCode  order by sequence", new { productCode = productHeader.Code }, transaction: tran);
            //        if (step == null && step.Count() != 2)
            //        {
            //            Logger.Log($"没有下一道工序！", LogLevel.Error);
            //            return BllResultFactory.Error();
            //        }
            //        //向工序表插入一条记录
            //        StepTrace stepTrace = new StepTrace();
            //        stepTrace.WONumber = callDetail.OrderCode;
            //        /* 由于目前是手动上料，暂用电气的ID，等到自动送料了，再改回数据库的 */
            //        stepTrace.WcsProductType = productType;
            //        stepTrace.ProductId = productHeader.Id.Value;
            //        stepTrace.ProductCode = productHeader.Code;
            //        stepTrace.SerialNumber = callDetail.SerialNumber;
            //        stepTrace.LineId = robot.LineId;
            //        stepTrace.StepId = step.ElementAt(0).Id.Value;
            //        stepTrace.StationId = robot.StationId;
            //        stepTrace.NextStepId = step.ElementAt(1).Id.Value;
            //        stepTrace.IsNG = false;
            //        stepTrace.NGcode = "";
            //        stepTrace.IsInvalid = false;
            //        stepTrace.Status = StepTraceStatus.设备请求下料.GetIndexInt();
            //        stepTrace.StationInTime = DateTime.Now;
            //        stepTrace.LineInTime = DateTime.Now;
            //        stepTrace.CreateTime = DateTime.Now;
            //        stepTrace.CreateBy = App.User.UserCode;
            //        connection.Insert<StepTrace>(stepTrace, transaction: tran);
            //        tran?.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        tran?.Rollback();
            //        Logger.Log($"处理设备【{robot.Name}】下料请求时候，发生异常：{ex.Message}", LogLevel.Exception, ex);
            //        return BllResultFactory.Error();
            //    }
            //    return BllResultFactory.Sucess();
            //}
        }















    }
}
