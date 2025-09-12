using AutoMapper;
using BookMS.Application.DTOs;
using BookMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BookEntity, BookDto>();
        }
    }
}
