using Mapster;
using WhatShouldIWorkOnToday.Application.WorkItems.Commands.CreateWorkItem;
using WhatShouldIWorkOnToday.Contracts.WorkItems;

namespace WhatShouldIWorkOnToday.Api.Common.Mapping;

public class WorkItemMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateWorkItemRequest, CreateWorkItemCommand>()
            .Map(dest => dest, src => src);
    }
}
