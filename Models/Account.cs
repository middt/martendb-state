using System;

namespace AccountOpening.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public AccountState State { get; set; }
    }

    public enum AccountState
    {
        New,
        UnderReview,
        Approved,
        Active,
        Rejected
    }

    public enum AccountEvent
    {
        Submit,
        Review,
        Approve,
        Reject,
        Activate
    }

    public class CreateAccountRequest
    {
        public string CustomerName { get; set; }
    }

    public class AccountStateChanged
    {
        public Guid AccountId { get; set; }
        public AccountState OldState { get; set; }
        public AccountState NewState { get; set; }
        public AccountEvent TriggeringEvent { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
