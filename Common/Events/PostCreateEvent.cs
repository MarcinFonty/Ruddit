using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public struct PostCreateEvent
    {
        public Guid Id {  get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
