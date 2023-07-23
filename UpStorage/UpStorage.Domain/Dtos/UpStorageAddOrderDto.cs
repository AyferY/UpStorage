using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpStorage.Domain.Common;
using UpStorage.Domain.Enum;

namespace UpStorage.Domain.Dtos
{
    public class UpStorageAddOrderDto : EntityBase
    {
        public int RequestedAmount { get; set; }
        public string ProductCrawlType { get; set; }

        public UpStorageAddOrderDto(int requestedAmount, string productCrawlType)
        {
            RequestedAmount = requestedAmount;
            ProductCrawlType = productCrawlType;
        }
    }
}
