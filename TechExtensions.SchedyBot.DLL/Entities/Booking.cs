using System;
using System.ComponentModel;

namespace TechExtensions.SchedyBot.DLL.Entities
{
    public class Booking : BaseEntity
    {
        public DateTime Date { get; set; }
        public Client Client { get; set; }
        public int? ClientId { get; set; }
        public Client Executor { get; set; }
        public int? ExecutorId { get; set; }
        public BookingCompletionStatus Status { get; set; }
        public ServiceAndSpec ServiceAndSpec { get; set; }
        public int? ServiceAndSpecId { get; set; }
        public string MessageForExecutor { get; set; }
        public string MessageForClient { get; set; }
        public DateTime BookTimeFrom { get; set; }
        public DateTime BookTimeTo { get; set; }
        public BookType BookType { get; set; }
        public string HangFireJobId { get; set; }
    }
    
    public enum BookingCompletionStatus
    {
        New = 0,
        Created = 1,

        AwaitingConfirmation = 4,
        Confirmed = 5,
        Rejected = 6,
        OnConfirm = 7,
        OnReject = 8,
        OnChangeTime = 9,
        OnCancel = 10,
        Canceled = 11
    }

    public enum BookType
    {
        [Description("По работе")]
        ForWork,
        [Description("Выходной")]
        DayOff,
        [Description("Отпуск")]
        Vacation
    }
}
