using DataAccess;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic;

public class IncidentFormService
{
    private readonly ILogger _logger;
    private readonly MariaDbContext _dbContext;
    
    public IncidentFormService(ILogger<IncidentFormService> logger, MariaDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task SubmitIncidentFormAsync(IncidentFormModel model)
    {
        await _dbContext.SaveIncidentFormAsync(model);  // Lagrer skjemaet i databasen
    }

    public bool ValidateIncidentForm(IncidentFormModel model)
    {
        // Validerer skjemaet
        if (model.Subject == null || model.Subject.Length < 5)
        {
            _logger.LogWarning("Subject is too short");
            return false;
        }
        if (model.Description == null || model.Description.Length < 10)
        {
            _logger.LogWarning("Description is too short");
            return false;
        }
        return true;
    }

}
