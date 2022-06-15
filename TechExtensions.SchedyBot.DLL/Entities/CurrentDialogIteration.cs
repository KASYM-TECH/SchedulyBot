using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechExtensions.SchedyBot.DLL.Entities;

public class CurrentDialogIteration
{
    [Key]
    public int Id { get; set; }
    public int TemplateId { get; set; }
    public int BranchId { get; set; }
    public int StepId { get; set; }
    public int CurrentDialogId { get; set; }
    [ForeignKey("CurrentDialogId")]
    public virtual CurrentDialog CurrentDialog { get; set; }
}