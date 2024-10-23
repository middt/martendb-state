using Marten;
using Microsoft.AspNetCore.Mvc;
using AccountOpening.Models;
using AccountOpening.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Marten
builder.Services.AddMarten(opts =>
{
    opts.Connection("Host=localhost;Database=account_opening;Username=marten_user;Password=marten_password");
    opts.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.All;
});

// Add services
builder.Services.AddSingleton<AccountOpeningStateMachine>();

var app = builder.Build();

app.MapPost("/account", async ([FromBody] CreateAccountRequest request, [FromServices] IDocumentSession session, [FromServices] AccountOpeningStateMachine stateMachine) =>
{
    var account = new Account { Id = Guid.NewGuid(), CustomerName = request.CustomerName, State = AccountState.New };
    session.Store(account);
    await session.SaveChangesAsync();

    return Results.Ok(account);
});

app.MapGet("/account/{id}/event/{accountEvent}", async ([FromRoute] Guid id, [FromRoute] AccountEvent accountEvent, [FromServices] AccountOpeningStateMachine stateMachine) =>
{
    try
    {
        var result = await stateMachine.TransitionAsync(id, accountEvent);
        return Results.Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/account/{id}", async ([FromRoute] Guid id, [FromServices] IQuerySession session) =>
{
    var account = await session.LoadAsync<Account>(id);
    return account != null ? Results.Ok(account) : Results.NotFound();
});

app.MapGet("/account/{id}/state", async ([FromRoute] Guid id, [FromServices] IQuerySession session) =>
{
    var account = await session.LoadAsync<Account>(id);
    return account != null ? Results.Ok(new { state = account.State }) : Results.NotFound();
});

// New endpoint to get event stream
app.MapGet("/account/{id}/events", async ([FromRoute] Guid id, [FromServices] IQuerySession session) =>
{
    var events = await session.Events.FetchStreamAsync(id);

    // Filter or transform events to avoid unsupported types
    var filteredEvents = events.Select(e => new
    {
        // Assuming 'e' has properties you want to keep, adjust as necessary
        e.Sequence,
        e.EventType.Name, // Ensure this is a string or a supported type
        e.Timestamp,
        // Add other properties as needed, avoiding unsupported types
    });

    return Results.Ok(filteredEvents);
});

app.Run();
