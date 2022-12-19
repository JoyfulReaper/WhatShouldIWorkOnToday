using Mapster;
using WhatShouldIWorkOnToday.Application.WorkItems.Commands.CreateWorkItem;
using WhatShouldIWorkOnToday.Contracts.WorkItems;
using WhatShouldIWorkOnToday.Domain.WorkItem;
using WhatShouldIWorkOnToday.Domain.WorkItem.Entities;

namespace WhatShouldIWorkOnToday.Api.Common.Mapping;

public class WorkItemMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateWorkItemRequest, CreateWorkItemCommand>()
            .Map(dest => dest, src => src);

        config.NewConfig<WorkItem, WorkItemResponse>()
            .Map(dest => dest, src => src);

        config.NewConfig<Note, NoteResponse>()
            .Map(dest => dest, src => src);

        config.NewConfig<ToDoItem, ToDoItemResponse>()
            .Map(dest => dest, src => src);

        config.NewConfig<WorkItemHistory, WorkItemHistoryResponse>()
            .Map(dest => dest, src => src);
    }
}
