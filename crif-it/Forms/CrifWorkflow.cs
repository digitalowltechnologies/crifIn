using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace Crif.It.Forms
{
    public class CrifWorkflow : WorkflowType
    {
        private readonly ILogger<CrifWorkflow> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        [Umbraco.Forms.Core.Attributes.Setting("Fields list", Description = "Fields to trace on GTM", View = "TextField")]
        public string FieldsList { get; set; } = "";


        public CrifWorkflow(ILogger<CrifWorkflow> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;

            Id = new Guid("ccbeb0d5-adaa-4729-8b4c-4bb439dc0202");
            Name = "Trace info on GTM";
            Description = "This workflow allows to trace some info from the form on GTM";
            Icon = "icon-axis-rotation";
            Group = "Services";
        }

        public override WorkflowExecutionStatus Execute(WorkflowExecutionContext context)
        {
            _logger.LogInformation("the IP " + context.Record.IP + " has submitted a record");
            
            foreach (RecordField rf in context.Record.RecordFields.Values)
            {
                if(FieldsList.Contains(rf.Alias))
                {
                    _contextAccessor?.HttpContext?.Session.SetString("form."+rf.Alias, rf.ValuesAsString());
                }
            }

            return WorkflowExecutionStatus.Completed;
        }

        public override List<Exception> ValidateSettings()
        {
            return new List<Exception>();
        }
    }
}
