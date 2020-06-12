using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Profession.Application.DTO
{
    public class PageSearchDto<TEntityDto>
        where TEntityDto : EntityDto<int>
    {
        public List<TEntityDto> EntityDtos { get; set; }

        public int Total { get; set; }
    }
}
