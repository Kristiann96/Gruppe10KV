﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Models;

namespace Interface
{
    public interface IKartverketAPILogic
    {
        Task<List<Kommune>> GetKommunerAsync();
    }
}


