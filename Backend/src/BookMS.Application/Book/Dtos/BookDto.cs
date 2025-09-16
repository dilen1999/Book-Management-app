using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMS.Application.DTOs
{
    public record BookDto(
        Guid Id,
        string Title,
        string? Isbn,
        int? PublishedYear
    );

}
