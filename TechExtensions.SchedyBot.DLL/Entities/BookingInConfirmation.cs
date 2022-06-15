using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class BookingInConfirmation : BaseEntity
    {
        public Booking BookingToConfirm { get; set; }
        public BookingCompletionStatusDuringSellerConfirmation Status { get; set; }
        public DateTime? TimeIsBeingChangedFrom { get; set; }
        public DateTime? TimeIsBeingChangedTo { get; set; }
        public string MessageForClient { get; set; }
    }
    public enum BookingCompletionStatusDuringSellerConfirmation
    {
        IsBeingCanceled = 1,
        IsBeingConfirmed = 2,
        TimeIsBeingChanged = 3
    }
}
