using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatShouldIWorkOnToday.Application.WorkItems.Commands.CreateWorkItem;

public class CreateWorkItemCommandValidator : AbstractValidator<CreateWorkItemCommand>
{
    public CreateWorkItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
