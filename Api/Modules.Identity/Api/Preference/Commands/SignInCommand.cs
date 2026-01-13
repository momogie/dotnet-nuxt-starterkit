namespace Modules.Identity.Api.Preference.Commands;

public class PreferenceEditCommand : ICommand
{
    [Required]
    [MaxLength(100)]
    public string Key { get; set; }
    public string Value { get; set; }
}
