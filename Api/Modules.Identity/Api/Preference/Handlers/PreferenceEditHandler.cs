using Modules.Identity.Api.Preference.Commands;

namespace Modules.Identity.Api.Preference.Handlers;

[Authorize]
[Post("/api/identity/preference/edit")]
public class PreferenceEditHandler(AppDbContext appDb, [FromBody] PreferenceEditCommand command) : CommandHandler
{
    protected Entities.DbSchemas.Preference Preference { get; set; }
    public override Task<IResult> Validate()
    {
        Preference = appDb.Preferences.FirstOrDefault(p => p.UserId == UserId && p.Key == command.Key);
        if (Preference == null)
        {
            Preference = new Entities.DbSchemas.Preference
            {
                UserId = UserId,
                Key = command.Key,
                Value = command.Value,
            };
            //cherryDb.Preferences.Add(Preference);
            appDb.Database.ExecuteSqlRaw("Insert into Preferences (UserId, [Key], [Value]) Values (@UserId, @Key, @Value)", new { UserId, Key = command.Key, Value = command.Value });
        }
        else
        {
            appDb.Database.ExecuteSqlRaw("Update Preferences set [Value]=@Value where [Key]=@Key and UserId=@UserId", new { UserId, Key = command.Key, Value = command.Value });

            Preference.Value = command.Value;
        }
        //cherryDb.SaveChanges();

        return base.Validate();
    }
    public override IResult Response()
    {
        return Ok(Preference);
    }
}
