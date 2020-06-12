using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ketchup.Profession.Application.DTO
{
    public abstract class PageDto
    {
        /// <summary>
        /// 当前页
        /// </summary>
        [Range(1, 2147483647, ErrorMessage = "pageIndex的值在1~2147483647之间")]
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 一页数据总数
        /// </summary>
        [Range(1, 2147483647, ErrorMessage = "pageSize的值在1~2147483647之间")]
        public int PageSize { get; set; } = 20;
    }
}
