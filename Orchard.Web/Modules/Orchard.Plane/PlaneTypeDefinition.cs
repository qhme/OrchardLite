using Orchard.ContentManagement.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Content1
{
    public class PlaneTypeDefinition : ContentTypeDefintion<TicketRecord>
    {
        public override string Description
        {
            get
            {
                return "机票业务";
            }
        }
    }
}