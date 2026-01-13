//using Modules.Identity.Entities;

//namespace Modules.Identity.Api.Preference.Handlers;

//[Authorize]
//[Get("/api/preference/list")]
//public class PreferenceListHandler(AppDbContext appDb) : CommandHandler
//{
//    public override List<PreferenceItem> Response()
//    {
//        if (string.IsNullOrWhiteSpace(EmployeeCode))
//            return [];

//        return [..appDb.Preferences.Where(p => p.AccountCode == UserCode).Select(p => new PreferenceItem
//        {
//            Key = p.Key,
//            Value = p.Value,
//        })];
//    }
//}

//public class PreferenceItem
//{
//    public string Key { get; set; }
//    public string Value { get; set; }
//}