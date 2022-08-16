using AutoMapper;
using WhatShouldIWorkOnToday.Server.Models;
using WhatShouldIWorkOnToday.Server.DTOs;

namespace WhatShouldIWorkOnToday.Server;

public class WhatShouldIWorkOnTodayProfile : Profile
{
	public WhatShouldIWorkOnTodayProfile()
	{
		CreateMap<WorkItem, WorkItemDto>();
		CreateMap<WorkItemDto, WorkItem>();
		CreateMap<PinnedWorkItem, PinnedWorkItemDto>();
	}
}
