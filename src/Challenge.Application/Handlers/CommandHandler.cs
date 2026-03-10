namespace Challenge.Application.Handlers;
public class CommandHandler
{

    public CommandHandler()
    {
    }



    public async Task<bool> Commit(CancellationToken cancelationToken)
    {
        //if (_notifications.HasNotifications()) return false;
        //if (await _uow.Commit(cancelationToken)) return true;

        //await _mediator.RaiseEvent(new DomainNotification("Commit", "We had a problem during saving your data."));
        return false;
    }
}
