using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Yyx.BS.Utils
{
    public enum OrderStatusEnum
    {
        [Description("已完成")]
        Complete,
        [Description("已完成")]
        Paid,
        [Description("已完成")]
        ServerSent,
        [Description("已完成")]
        UserCancel,
        [Description("申请取消")]
        TryCancel
    }

    public enum PayStatusEnum
    {
        [Description("已支付")]
        Paid,
        [Description("已退款")]
        Refunded,
        [Description("出错了")]
        Error
    }
}
