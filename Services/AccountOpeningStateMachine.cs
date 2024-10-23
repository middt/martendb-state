using Marten;
using AccountOpening.Models;
using System;
using System.Threading.Tasks;

namespace AccountOpening.Services
{
    public class AccountOpeningStateMachine
    {
        private readonly IDocumentStore _store;

        public AccountOpeningStateMachine(IDocumentStore store)
        {
            _store = store;
        }

        public async Task<Account> TransitionAsync(Guid accountId, AccountEvent accountEvent)
        {
            using var session = _store.LightweightSession();
            var account = await session.LoadAsync<Account>(accountId);

            if (account == null)
            {
                throw new ArgumentException("Account not found", nameof(accountId));
            }

            var oldState = account.State;
            var newState = (account.State, accountEvent) switch
            {
                (AccountState.New, AccountEvent.Submit) => AccountState.UnderReview,
                (AccountState.UnderReview, AccountEvent.Approve) => AccountState.Approved,
                (AccountState.UnderReview, AccountEvent.Reject) => AccountState.Rejected,
                (AccountState.Approved, AccountEvent.Activate) => AccountState.Active,
                _ => throw new InvalidOperationException(GetInvalidTransitionMessage(account.State, accountEvent))
            };

            account.State = newState;
            session.Update(account);

            var stateChangedEvent = new AccountStateChanged
            {
                AccountId = accountId,
                OldState = oldState,
                NewState = newState,
                TriggeringEvent = accountEvent,
                Timestamp = DateTime.UtcNow
            };

            session.Events.Append(accountId, stateChangedEvent);
            await session.SaveChangesAsync();

            return account;
        }

        private string GetInvalidTransitionMessage(AccountState currentState, AccountEvent accountEvent)
        {
            return $"Invalid state transition from {currentState} with event {accountEvent}. " +
                   GetValidTransitionsMessage(currentState);
        }

        private string GetValidTransitionsMessage(AccountState currentState)
        {
            return currentState switch
            {
                AccountState.New => "From New state, you can only Submit.",
                AccountState.UnderReview => "From UnderReview state, you can either Approve or Reject.",
                AccountState.Approved => "From Approved state, you can only Activate.",
                AccountState.Active => "Active is a final state. No further transitions are allowed.",
                AccountState.Rejected => "Rejected is a final state. No further transitions are allowed.",
                _ => "Unknown state."
            };
        }
    }
}
