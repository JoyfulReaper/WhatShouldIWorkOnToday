using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatShouldIWorkOnToday.Application.Common.Interfaces.Services;

namespace WhatShouldIWorkOnToday.Infrastructure.Persistence.Interceptors;
public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public AuditableEntitySaveChangesInterceptor(IDateTimeProvider dateTimeProvider, ICurrentUserService currentUserService)
    {
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }
}
