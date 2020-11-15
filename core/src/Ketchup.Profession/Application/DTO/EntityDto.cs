using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Profession.Application.DTO
{
    public abstract class EntityDto<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }
}
