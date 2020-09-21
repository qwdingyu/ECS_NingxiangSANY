using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHECS.Model.Entities
{
    /// <summary>
    /// 系统字段
    /// </summary>
    public abstract class SysEntity : ICloneable
    {
        /// <summary>
	    /// Id
	    /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int? Id { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 实现ICloneable接口，达到浅表复制。
        /// 浅表复制会复制出一个新对象，新对象的值类型会复制一份新的，但是对象包含的引用类型依然是旧的引用。
        /// </summary>
        /// <returns></returns>
        public Object Clone()  
        {
            return this.MemberwiseClone();
        }
    }
}
