using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Content1
{
    public class TicketRecord : ContentPartRecord
    {
        public string OrderNo { get; set; }

        public string PNR { get; set; }
    }
}