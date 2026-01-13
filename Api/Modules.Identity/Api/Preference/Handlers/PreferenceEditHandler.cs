//using Cherry.Entities.Entities;
//using Microsoft.AspNetCore.Http;
//using Modules.Identity.Api.Preference.Commands;

//namespace Modules.Identity.Api.Preference.Handlers;

//[Authorize]
//[Post("/api/preference/edit")]
//public class PreferenceEditHandler(CherryDbContext cherryDb, [FromBody] PreferenceEditCommand command) : CommandHandler
//{
//    protected Cherry.Entities.Entities.DbSchemas.Preference Preference { get; set; }
//    public override Task<IResult> Validate()
//    {
//        Preference = cherryDb.Preferences.FirstOrDefault(p => p.AccountCode == UserCode && p.Key == command.Key);
//        if(Preference == null)
//        {
//            Preference = new Cherry.Entities.Entities.DbSchemas.Preference
//            {
//                AccountCode = UserCode,
//                Key = command.Key,
//                Value = command.Value,
//            };
//            //cherryDb.Preferences.Add(Preference);
//            cherryDb.Views.Execute("Insert into AppsUserPreferences (AccountCode, [Key], [Value]) Values (@AccountCode, @Key, @Value)", new { AccountCode = UserCode, Key = command.Key, Value = command.Value });
//        }
//        else
//        {
//            cherryDb.Views.Execute("Update AppsUserPreferences set [Value]=@Value where [Key]=@Key and AccountCode=@AccountCode", new { AccountCode = UserCode, Key = command.Key, Value = command.Value });

//            Preference.Value = command.Value;
//        }
//        //cherryDb.SaveChanges();

//        return base.Validate();
//    }
//    public override IResult Response()
//    {
//        return Ok(Preference);
//    }
//}
